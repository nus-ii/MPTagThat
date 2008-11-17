using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.IO;
using System.Text;
using System.Reflection;
using System.Threading;
using System.Windows.Forms;

using TagLib;
using MPTagThat.Core;
using MPTagThat.Core.Amazon;
using MPTagThat.Core.MusicBrainz;
using MPTagThat.Dialogues;
using MPTagThat.Player;

namespace MPTagThat.GridView
{
  public partial class GridViewTracks : UserControl
  {
    #region Variables
    private delegate void ThreadSafeGridDelegate();
    private delegate void ThreadSafeAddTracksDelegate(TrackData track);
    private delegate void ThreadSafeAddErrorDelegate(string file, string message);
    private GridViewColumns gridColumns;

    private Main _main;

    private bool _itemsChanged;
    private List<string> _lyrics = new List<string>();

    private SortableBindingList<TrackData> bindingList = new SortableBindingList<TrackData>();

    private ILocalisation localisation = ServiceScope.Get<ILocalisation>();
    private ILogger log = ServiceScope.Get<ILogger>();

    private Progress dlgProgress;

    private Rectangle _dragBoxFromMouseDown;
    private Point _screenOffset;

    private Thread _asyncThread = null;
    #endregion

    #region Properties
    /// <summary>
    /// Returns the GridView
    /// </summary>
    public System.Windows.Forms.DataGridView View
    {
      get { return tracksGrid; }
    }

    /// <summary>
    /// Returns the Selected Track
    /// </summary>
    public TrackData SelectedTrack
    {
      get { return bindingList[tracksGrid.CurrentRow.Index]; }
    }

    /// <summary>
    /// Do we have any changes pending?
    /// </summary>
    public bool Changed
    {
      get { return _itemsChanged; }
      set { _itemsChanged = value; }
    }

    /// <summary>
    /// Returns the Bindinglist with all the Rows
    /// </summary>
    public SortableBindingList<TrackData> TrackList
    {
      get { return bindingList; }
    }
    #endregion

    #region Constructor
    public GridViewTracks()
    {
      InitializeComponent();

      // Setup message queue for receiving Messages
      IMessageQueue queueMessage = ServiceScope.Get<IMessageBroker>().GetOrCreate("message");
      queueMessage.OnMessageReceive += new MessageReceivedHandler(OnMessageReceive);

      // Load the Settings
      gridColumns = new GridViewColumns();

      // Setup Dataview Grid
      tracksGrid.AutoGenerateColumns = false;
      tracksGrid.DataSource = bindingList;
      tracksGrid.ClipboardCopyMode = DataGridViewClipboardCopyMode.Disable; // Handle Copy 

      // Setup Event Handler
      tracksGrid.ColumnWidthChanged += new DataGridViewColumnEventHandler(tracksGrid_ColumnWidthChanged);
      tracksGrid.CurrentCellDirtyStateChanged += new EventHandler(tracksGrid_CurrentCellDirtyStateChanged);
      tracksGrid.DataError += new DataGridViewDataErrorEventHandler(tracksGrid_DataError);
      tracksGrid.CellEndEdit += new DataGridViewCellEventHandler(tracksGrid_CellEndEdit);
      tracksGrid.CellValueChanged += new DataGridViewCellEventHandler(tracksGrid_CellValueChanged);
      tracksGrid.EditingControlShowing += new DataGridViewEditingControlShowingEventHandler(tracksGrid_EditingControlShowing);
      tracksGrid.Sorted += new EventHandler(tracksGrid_Sorted);
      tracksGrid.ColumnHeaderMouseClick += new DataGridViewCellMouseEventHandler(tracksGrid_ColumnHeaderMouseClick);
      tracksGrid.MouseDown += new MouseEventHandler(tracksGrid_MouseDown);
      tracksGrid.MouseUp += new MouseEventHandler(tracksGrid_MouseUp);
      tracksGrid.MouseMove += new MouseEventHandler(tracksGrid_MouseMove);
      tracksGrid.QueryContinueDrag += new QueryContinueDragEventHandler(tracksGrid_QueryContinueDrag);

      // The Color for the Image Cell for the Rating is not handled correctly. so we need to handle it via an event
      tracksGrid.SelectionChanged += new EventHandler(tracksGrid_SelectionChanged);

      // Now Setup the columns, we want to display
      CreateColumns();

      // Create the Context Menu
      CreateContextMenu();
    }
    #endregion

    #region Public Methods
    /// <summary>
    /// Set Main Ref to Main
    /// Can#t do it in constructir due to problems with Designer
    /// </summary>
    /// <param name="main"></param>
    public void SetMainRef(Main main)
    {
      _main = main;
    }

    /// <summary>
    /// Creates an Item in the grid out of the given File
    /// </summary>
    /// <param name="trackPath"></param>
    /// <param name="file"></param>
    public void CreateTracksItem(string trackPath, TagLib.File file)
    {
      AddTrack(new TrackData(file));
    }

    #region Save
    public void Save()
    {
      if (_asyncThread == null)
      {
        _asyncThread = new Thread(new ThreadStart(SaveThread));
        _asyncThread.Name = "Save";
      }

      if (_asyncThread.ThreadState != ThreadState.Running)
      {
        _asyncThread = new Thread(new ThreadStart(SaveThread));
        _asyncThread.Start();
      }
    }

    /// <summary>
    /// Save the Selected files only
    /// </summary>
    private void SaveThread()
    {
      Util.EnterMethod(Util.GetCallingMethod());
      //Make calls to Tracksgrid Threadsafe
      if (tracksGrid.InvokeRequired)
      {
        ThreadSafeGridDelegate d = new ThreadSafeGridDelegate(SaveThread);
        tracksGrid.Invoke(d, new object[] { });
        return;
      }

      dlgProgress = new Progress();
      dlgProgress.Text = localisation.ToString("progress", "SavingHeader");
      ShowForm(dlgProgress);

      int count = 0;
      int trackCount = tracksGrid.SelectedRows.Count;
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        row.Cells[1].Value = "";

        if (!row.Selected)
          continue;

        count++;
        try
        {
          Application.DoEvents();
          dlgProgress.UpdateProgress(ProgressBarStyle.Blocks, string.Format(localisation.ToString("progress", "Saving"), count, trackCount), count, trackCount, true);
          if (dlgProgress.IsCancelled)
          {
            dlgProgress.Close();
            return;
          }
          TrackData track = bindingList[row.Index];
          if (track.Changed)
          {
            if (Options.MainSettings.CopyArtist && track.AlbumArtist == "")
              track.AlbumArtist = track.Artist;

            // Save the file 
            track.File = Util.FormatID3Tag(track.File);
            track.File.Save();

            if (RenameFile(track))
            {
              // rename was ok, so get the new file into the binding list
              string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(track.File.Name), track.FileName);
              TagLib.ByteVector.UseBrokenLatin1Behavior = true;
              TagLib.File file = TagLib.File.Create(newFileName);
              track.File = file;
            }

            row.Cells[1].Value = localisation.ToString("message", "Ok");
            if (row.Index % 2 == 0)
              row.DefaultCellStyle.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.DefaultBackColor;
            else
              row.DefaultCellStyle.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.AlternatingRowBackColor;

            track.Changed = false;
          }
        }
        catch (Exception ex)
        {
          row.Cells[1].Value = localisation.ToString("message", "Error");
          AddErrorMessage(bindingList[row.Index].File.Name, ex.Message);
        }
      }

      dlgProgress.Close();

      _itemsChanged = false;
      // check, if we still have changed items in the list
      foreach (TrackData track in bindingList)
      {
        if (track.Changed)
          _itemsChanged = true;
      }

      Util.LeaveMethod(Util.GetCallingMethod());

    }

    public void SaveAll()
    {
      if (_asyncThread == null)
      {
        _asyncThread = new Thread(new ThreadStart(SaveAllThread));
        _asyncThread.Name = "SaveAll";
      }

      if (_asyncThread.ThreadState != ThreadState.Running)
      {
        _asyncThread = new Thread(new ThreadStart(SaveAllThread));
        _asyncThread.Start();
      }
    }

    /// <summary>
    /// Save All changed files, regardless, if they are selected or not
    /// </summary>
    private void SaveAllThread()
    {
      Util.EnterMethod(Util.GetCallingMethod());
      //Make calls to Tracksgrid Threadsafe
      if (tracksGrid.InvokeRequired)
      {
        ThreadSafeGridDelegate d = new ThreadSafeGridDelegate(SaveAllThread);
        tracksGrid.Invoke(d, new object[] { });
        return;
      }

      bool bErrors = false;
      int i = 0;

      dlgProgress = new Progress();
      dlgProgress.Text = localisation.ToString("progress", "SavingHeader");
      ShowForm(dlgProgress);

      int trackCount = bindingList.Count;
      foreach (TrackData track in bindingList)
      {
        Application.DoEvents();
        dlgProgress.UpdateProgress(ProgressBarStyle.Blocks, string.Format(localisation.ToString("progress", "Saving"), i + 1, trackCount), i + 1, trackCount, true);
        if (dlgProgress.IsCancelled)
        {
          dlgProgress.Close();
          return;
        }
        try
        {
          tracksGrid.Rows[i].Cells[1].Value = "";
          if (track.Changed)
          {
            if (Options.MainSettings.CopyArtist && track.AlbumArtist == "")
              track.AlbumArtist = track.Artist;

            // Save the file 
            track.File = Util.FormatID3Tag(track.File);
            track.File.Save();

            if (RenameFile(track))
            {
              // rename was ok, so get the new file into the binding list
              string ext = System.IO.Path.GetExtension(track.File.Name);
              string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(track.File.Name), String.Format("{0}{1}", track.FileName, ext));
              TagLib.ByteVector.UseBrokenLatin1Behavior = true;
              TagLib.File file = TagLib.File.Create(newFileName);
              track.File = file;
            }

            tracksGrid.Rows[i].Cells[1].Value = localisation.ToString("message", "Ok");
            if (i % 2 == 0)
              tracksGrid.Rows[i].DefaultCellStyle.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.DefaultBackColor;
            else
              tracksGrid.Rows[i].DefaultCellStyle.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.AlternatingRowBackColor;

            track.Changed = false;
          }
        }
        catch (Exception ex)
        {
          tracksGrid.Rows[i].Cells[1].Value = localisation.ToString("message", "Error");
          AddErrorMessage(track.File.Name, ex.Message);
          bErrors = true;
        }
        i++;
      }

      dlgProgress.Close();
      _itemsChanged = bErrors;

      Util.LeaveMethod(Util.GetCallingMethod());
    }

    /// <summary>
    /// Rename the file if necessary
    /// Called by Save and SaveAll
    /// </summary>
    /// <param name="track"></param>
    private bool RenameFile(TrackData track)
    {
      string originalFileName = System.IO.Path.GetFileName(track.File.Name);
      if (originalFileName != track.FileName)
      {
        string ext = System.IO.Path.GetExtension(track.File.Name);
        string newFileName = System.IO.Path.Combine(System.IO.Path.GetDirectoryName(track.File.Name), String.Format("{0}{1}", System.IO.Path.GetFileNameWithoutExtension(track.FileName), ext));
        System.IO.File.Move(track.File.Name, newFileName);
        return true;
      }
      return false;
    }
    #endregion

    #region Identify File
    public void TagTracksFromInternet()
    {
      if (_asyncThread == null)
      {
        _asyncThread = new Thread(new ThreadStart(TagTracksFromInternetThread));
        _asyncThread.Name = "TagFromInternet";
      }

      if (_asyncThread.ThreadState != ThreadState.Running)
      {
        _asyncThread = new Thread(new ThreadStart(TagTracksFromInternetThread));
        _asyncThread.Start();
      }
    }

    /// <summary>
    /// Tag the the Selected files from Internet
    /// </summary>
    private void TagTracksFromInternetThread()
    {
      Util.EnterMethod(Util.GetCallingMethod());
      //Make calls to Tracksgrid Threadsafe
      if (tracksGrid.InvokeRequired)
      {
        ThreadSafeGridDelegate d = new ThreadSafeGridDelegate(TagTracksFromInternetThread);
        tracksGrid.Invoke(d, new object[] { });
        return;
      }

      dlgProgress = new Progress();
      dlgProgress.Text = localisation.ToString("progress", "InternetHeader");
      ShowForm(dlgProgress);

      int count = 0;
      int trackCount = tracksGrid.SelectedRows.Count;
      MusicBrainzAlbum musicBrainzAlbum = new MusicBrainzAlbum();

      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        row.Cells[1].Value = "";

        if (!row.Selected)
          continue;

        count++;
        try
        {
          Application.DoEvents();
          dlgProgress.UpdateProgress(ProgressBarStyle.Blocks, string.Format(localisation.ToString("progress", "Internet"), count, trackCount), count, trackCount, true);
          if (dlgProgress.IsCancelled)
          {
            dlgProgress.Close();
            return;
          }
          TrackData track = bindingList[row.Index];

          using (MusicBrainzTrackInfo trackinfo = new MusicBrainzTrackInfo())
          {
            dlgProgress.StatusLabel2 = localisation.ToString("progress", "InternetMusicBrainz");
            List<MusicBrainzTrack> musicBrainzTracks = trackinfo.GetMusicBrainzTrack(track.FullFileName);
            if (musicBrainzTracks.Count > 0)
            {
              MusicBrainzTrack musicBrainzTrack = null;
              if (musicBrainzTracks.Count == 1)
                musicBrainzTrack = musicBrainzTracks[0];
              else
              {
                // Skip the Album selection, if the album been selected already for a previous track
                bool albumFound = false;
                foreach (MusicBrainzTrack mbtrack in musicBrainzTracks)
                {
                  if (mbtrack.AlbumID == musicBrainzAlbum.Id)
                  {
                    albumFound = true;
                    musicBrainzTrack = mbtrack;
                    break;
                  }
                }

                if (!albumFound)
                {
                  MusicBrainzAlbumResults dlgAlbumResults = new MusicBrainzAlbumResults(musicBrainzTracks);
                  if (_main.ShowForm(dlgAlbumResults) == DialogResult.OK)
                  {
                    if (dlgAlbumResults.SelectedListItem > -1)
                      musicBrainzTrack = musicBrainzTracks[dlgAlbumResults.SelectedListItem];
                    else
                      musicBrainzTrack = musicBrainzTracks[0];
                  }
                  dlgAlbumResults.Dispose();
                }
              }

              // We didn't get a track
              if (musicBrainzTrack == null)
                continue;

              // Are we still at the same album?
              // if not, get the album, so that we have the release date
              if (musicBrainzAlbum.Id != musicBrainzTrack.AlbumID)
              {
                using (MusicBrainzAlbumInfo albumInfo = new MusicBrainzAlbumInfo())
                {
                  dlgProgress.StatusLabel2 = localisation.ToString("progress", "InternetAlbum");
                  Application.DoEvents();
                  if (dlgProgress.IsCancelled)
                  {
                    dlgProgress.Close();
                    return;
                  }
                  musicBrainzAlbum = albumInfo.GetMusicBrainzAlbumById(musicBrainzTrack.AlbumID.ToString());
                }
              }

              track.Title = musicBrainzTrack.Title;
              track.Artist = musicBrainzTrack.Artist;
              track.Album = musicBrainzTrack.Album;
              track.Track = musicBrainzTrack.Number.ToString();

              if (musicBrainzAlbum.Year != null && musicBrainzAlbum.Year.Length >= 4)
                track.Year = Convert.ToInt32(musicBrainzAlbum.Year.Substring(0, 4));

              // Do we have a valid Amazon Album?
              if (musicBrainzAlbum.Amazon != null)
              {
                // Get the picture
                ByteVector vector = musicBrainzAlbum.Amazon.AlbumImage;
                if (vector != null)
                {
                  Picture pic = new Picture(vector);
                  pic.MimeType = "image/jpg";
                  pic.Description = "";
                  pic.Type = PictureType.FrontCover;
                  track.Pictures = new TagLib.IPicture[] { pic };
                }
              }

              SetBackgroundColorChanged(row.Index);
              track.Changed = true;
              _itemsChanged = true;
              row.Cells[1].Value = localisation.ToString("message", "Ok");
            }
          }
        }
        catch (Exception ex)
        {
          row.Cells[1].Value = localisation.ToString("message", "Error");
          AddErrorMessage(bindingList[row.Index].File.Name, ex.Message);
        }
      }

      tracksGrid.Refresh();
      tracksGrid.Parent.Refresh();

      dlgProgress.Close();

      Util.LeaveMethod(Util.GetCallingMethod());
    }
    #endregion

    #region Cover Art
    public void GetCoverArt()
    {
      if (_asyncThread == null)
      {
        _asyncThread = new Thread(new ThreadStart(GetCoverArtThread));
        _asyncThread.Name = "GetCoverArt";
      }

      if (_asyncThread.ThreadState != ThreadState.Running)
      {
        _asyncThread = new Thread(new ThreadStart(GetCoverArtThread));
        _asyncThread.Start();
      }
    }

    /// <summary>
    /// Get Cover Art via Amazon Webservice
    /// </summary>
    private void GetCoverArtThread()
    {
      Util.EnterMethod(Util.GetCallingMethod());
      //Make calls to Tracksgrid Threadsafe
      if (tracksGrid.InvokeRequired)
      {
        ThreadSafeGridDelegate d = new ThreadSafeGridDelegate(GetCoverArtThread);
        tracksGrid.Invoke(d, new object[] { });
        return;
      }

      dlgProgress = new Progress();
      dlgProgress.Text = localisation.ToString("progress", "CoverArtHeader");
      ShowForm(dlgProgress);

      int count = 0;
      int trackCount = tracksGrid.SelectedRows.Count;
      AmazonAlbum amazonAlbum = null;

      string savedArtist = "";
      string savedAlbum = "";

      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        row.Cells[1].Value = "";

        if (!row.Selected)
          continue;

        count++;
        try
        {
          Application.DoEvents();
          dlgProgress.UpdateProgress(ProgressBarStyle.Blocks, string.Format(localisation.ToString("progress", "CoverArt"), count, trackCount), count, trackCount, true);
          if (dlgProgress.IsCancelled)
          {
            dlgProgress.Close();
            return;
          }
          TrackData track = bindingList[row.Index];

          if (track.Album == "" || track.Artist == "")
            continue;

          // Only retrieve the Cover Art, if we don't have it yet
          if (track.Album != savedAlbum || track.Artist != savedArtist || amazonAlbum == null)
          {
            savedArtist = track.Artist;
            savedAlbum = track.Album;

            List<AmazonAlbum> albums = new List<AmazonAlbum>();
            using (AmazonAlbumInfo amazonInfo = new AmazonAlbumInfo())
            {
              albums = amazonInfo.AmazonAlbumSearch(track.Artist, track.Album);
            }

            amazonAlbum = null;
            if (albums.Count > 0)
            {
              if (albums.Count == 1)
              {
                amazonAlbum = albums[0];
              }
              else
              {
                AmazonAlbumSearchResults dlgAlbumResults = new AmazonAlbumSearchResults(albums);
                if (_main.ShowForm(dlgAlbumResults) == DialogResult.OK)
                {
                  if (dlgAlbumResults.SelectedListItem > -1)
                    amazonAlbum = albums[dlgAlbumResults.SelectedListItem];
                  else
                    amazonAlbum = albums[0];
                }
                dlgAlbumResults.Dispose();
              }

              if (amazonAlbum == null)
                continue;

            }
          }

          // Now update the Cover Art
          if (amazonAlbum != null)
          {
            ByteVector vector = amazonAlbum.AlbumImage;
            if (vector != null)
            {
              // Get the availbe Covers
              List<IPicture> pics = new List<IPicture>();
              pics = new List<IPicture>(track.Pictures);

              Picture pic = new Picture(vector);
              pics.Add(pic);

              track.Pictures = pics.ToArray();
            }


            SetBackgroundColorChanged(row.Index);
            track.Changed = true;
            _itemsChanged = true;
            row.Cells[1].Value = localisation.ToString("message", "Ok");
            _main.FillInfoPanel();
          }

        }
        catch (Exception ex)
        {
          row.Cells[1].Value = localisation.ToString("message", "Error");
          AddErrorMessage(bindingList[row.Index].File.Name, ex.Message);
        }
      }

      tracksGrid.Refresh();
      tracksGrid.Parent.Refresh();

      dlgProgress.Close();

      Util.LeaveMethod(Util.GetCallingMethod());
    }
    #endregion

    #region Lyrics
    public void GetLyrics()
    {
      if (_asyncThread == null)
      {
        _asyncThread = new Thread(new ThreadStart(GetLyricsThread));
        _asyncThread.Name = "GetLyrics";
      }

      if (_asyncThread.ThreadState != ThreadState.Running)
      {
        _asyncThread = new Thread(new ThreadStart(GetLyricsThread));
        _asyncThread.Start();
      }
    }

    /// <summary>
    /// Get Lyrics for selected Rows
    /// </summary>
    private void GetLyricsThread()
    {
      Util.EnterMethod(Util.GetCallingMethod());
      //Make calls to Tracksgrid Threadsafe
      if (tracksGrid.InvokeRequired)
      {
        ThreadSafeGridDelegate d = new ThreadSafeGridDelegate(GetLyricsThread);
        tracksGrid.Invoke(d, new object[] { });
        return;
      }

      dlgProgress = new Progress();
      dlgProgress.Text = localisation.ToString("progress", "LyricsHeader");
      ShowForm(dlgProgress);

      int count = 0;
      int trackCount = tracksGrid.SelectedRows.Count;

      List<TrackData> tracks = new List<TrackData>();
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (!row.Selected)
          continue;

        count++;
        Application.DoEvents();
        dlgProgress.UpdateProgress(ProgressBarStyle.Blocks, string.Format(localisation.ToString("progress", "Lyrics"), count, trackCount), count, trackCount, true);
        if (dlgProgress.IsCancelled)
        {
          dlgProgress.Close();
          return;
        }
        tracks.Add(bindingList[row.Index]);
      }

      dlgProgress.Close();

      if (tracks.Count > 0)
      {
        try
        {
          LyricsSearch lyricssearch = new LyricsSearch(tracks);
          if (_main.ShowForm(lyricssearch) == DialogResult.OK)
          {
            DataGridView lyricsResult = lyricssearch.GridView;
            foreach (DataGridViewRow lyricsRow in lyricsResult.Rows)
            {
              if (lyricsRow.Cells[0].Value == System.DBNull.Value || lyricsRow.Cells[0].Value == null)
                continue;

              if ((bool)lyricsRow.Cells[0].Value != true)
                continue;

              foreach (DataGridViewRow row in tracksGrid.Rows)
              {
                TrackData lyricsTrack = tracks[lyricsRow.Index];
                TrackData track = bindingList[row.Index];
                if (lyricsTrack.FullFileName == track.FullFileName)
                {
                  track.Lyrics = (string)lyricsRow.Cells[5].Value;
                  SetBackgroundColorChanged(row.Index);
                  track.Changed = true;
                  _itemsChanged = true;
                  break;
                }
              }
            }
          }
        }
        catch (Exception ex)
        {
          log.Error("Error in Lyricssearch: {0}", ex.Message);
        }

      }

      tracksGrid.Refresh();
      tracksGrid.Parent.Refresh();

      Util.LeaveMethod(Util.GetCallingMethod());
    }
    #endregion

    /// <summary>
    /// Discards any changes
    /// </summary>
    public void DiscardChanges()
    {
      _itemsChanged = false;
    }

    /// <summary>
    /// Remove the Tags
    /// </summary>
    /// <param name="type"></param>
    public void DeleteTags(TagTypes type)
    {
      Util.EnterMethod(Util.GetCallingMethod());
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (!row.Selected)
          continue;

        TrackData track = bindingList[row.Index];
        try
        {
          track.File.RemoveTags(type);

          SetBackgroundColorChanged(row.Index);
          track.Changed = true;
          _itemsChanged = true;
        }
        catch (Exception ex)
        {
          log.Error("Error while Removing Tags: {0} stack: {1}", ex.Message, ex.StackTrace);
          row.Cells[1].Value = localisation.ToString("message", "Error");
          AddErrorMessage(track.File.Name, ex.Message);

        }
      }

      tracksGrid.Refresh();
      tracksGrid.Parent.Refresh();

      Util.LeaveMethod(Util.GetCallingMethod());
    }

    /// <summary>
    /// The Del key has been pressed. Send the selected files to the recycle bin
    /// </summary>
    public void DeleteTracks()
    {
      Util.EnterMethod(Util.GetCallingMethod());
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (!row.Selected)
          continue;

        TrackData track = bindingList[row.Index];
        try
        {
          Util.SHFILEOPSTRUCT shf = new Util.SHFILEOPSTRUCT();
          shf.wFunc = Util.FO_DELETE;
          shf.fFlags = Util.FOF_ALLOWUNDO | Util.FOF_NOCONFIRMATION;
          shf.pFrom = track.File.Name;
          Util.SHFileOperation(ref shf);

          // Remove the file from the binding list
          bindingList.RemoveAt(row.Index);
        }
        catch (Exception ex)
        {
          log.Error("Error applying changes from MultiTagedit: {0} stack: {1}", ex.Message, ex.StackTrace);
          row.Cells[1].Value = localisation.ToString("message", "Error");
          AddErrorMessage(track.File.Name, ex.Message);
        }
      }

      tracksGrid.Refresh();
      tracksGrid.Parent.Refresh();

      Util.LeaveMethod(Util.GetCallingMethod());
    }


    /// <summary>
    /// Checks, if we have something selected
    /// </summary>
    /// <returns></returns>
    public bool CheckSelections()
    {
      bool selected = false;

      // Check for at least one row selected
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (row.Selected)
        {
          selected = true;
          break;
        }
      }

      // display a message box, when nothing is selected
      if (!selected)
        MessageBox.Show(localisation.ToString("message", "NoSelection"), localisation.ToString("message", "NoSelectionHeader"), MessageBoxButtons.OK, MessageBoxIcon.Exclamation);

      return selected;
    }

    /// <summary>
    /// Sets the Background Color for changed Items
    /// </summary>
    /// <param name="index"></param>
    public void SetBackgroundColorChanged(int index)
    {
      tracksGrid.Rows[index].DefaultCellStyle.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.ChangedBackColor;
      tracksGrid.Rows[index].DefaultCellStyle.ForeColor = ServiceScope.Get<IThemeManager>().CurrentTheme.ChangedForeColor;
    }

    /// <summary>
    /// Adds an Error Message to the Message Grid
    /// </summary>
    /// <param name="file"></param>
    /// <param name="message"></param>
    public void AddErrorMessage(string file, string message)
    {
      if (_main.ErrorGridView.InvokeRequired)
      {
        ThreadSafeAddErrorDelegate d = new ThreadSafeAddErrorDelegate(AddErrorMessage);
        _main.ErrorGridView.Invoke(d, new object[] { file, message });
        return;
      }

      _main.ErrorGridView.Rows.Add(file, message);
    }

    #region Script Handling
    /// <summary>
    /// Executes a script on all selected rows
    /// </summary>
    /// <param name="scriptFile"></param>
    public void ExecuteScript(string scriptFile)
    {
      Util.EnterMethod(Util.GetCallingMethod());
      Assembly assembly = ServiceScope.Get<IScriptManager>().Load(scriptFile);

      try
      {
        if (assembly != null)
        {
          IScript script = (IScript)assembly.CreateInstance("Script");
          int i = 0;
          foreach (TrackData track in bindingList)
          {
            if (tracksGrid.Rows[i].Selected)
            {
              script.Invoke(track);
              track.Changed = true;
              SetBackgroundColorChanged(i);
            }
            i++;
          }
          _itemsChanged = true;
        }
      }
      catch (Exception ex)
      {
        log.Error("Script Execution failed: {0}", ex.Message);
        MessageBox.Show(localisation.ToString("message", "Script_Compile_Failed"), localisation.ToString("message", "Error_Title"), MessageBoxButtons.OK);
      }
      tracksGrid.Refresh();
      tracksGrid.Parent.Refresh();

      Util.LeaveMethod(Util.GetCallingMethod());
    }
    #endregion
    #endregion

    #region Private Methods
    #region Localisation
    /// <summary>
    /// Language Change event has been fired. Apply the new language
    /// </summary>
    private void LanguageChanged()
    {
      LocaliseScreen();
    }

    private void LocaliseScreen()
    {
      // Update the column Headings
      foreach (DataGridViewColumn col in tracksGrid.Columns)
      {
        col.HeaderText = localisation.ToString("column_header", col.Name);
      }
    }
    #endregion

    /// <summary>
    /// Create the Columns of the Grid based on the users setting
    /// </summary>
    private void CreateColumns()
    {
      Util.EnterMethod(Util.GetCallingMethod());

      // Now create the columns 
      foreach (GridViewColumn column in gridColumns.Settings.Columns)
      {
        tracksGrid.Columns.Add(Util.FormatGridColumn(column));
      }

      // Add a dummy column and set the property of the last column to fill
      DataGridViewColumn col = new DataGridViewTextBoxColumn();
      col.Name = "dummy";
      col.HeaderText = "";
      col.ReadOnly = true;
      col.Visible = true;
      col.Width = 5;
      tracksGrid.Columns.Add(col);
      tracksGrid.Columns[tracksGrid.Columns.Count - 1].AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;

      Util.LeaveMethod(Util.GetCallingMethod());
    }

    /// <summary>
    /// Save the settings
    /// </summary>
    private void SaveSettings()
    {
      // Save the Width of the Columns
      int i = 0;
      foreach (DataGridViewColumn column in tracksGrid.Columns)
      {
        // Don't save the dummy column
        if (i == tracksGrid.Columns.Count - 1)
          break;

        gridColumns.SaveColumnSettings(column, i);
        i++;
      }
      gridColumns.SaveSettings();
    }

    /// <summary>
    /// Adds a Track to the data grid
    /// </summary>
    /// <param name="track"></param>
    private void AddTrack(TrackData track)
    {
      if (track == null)
        return;


      if (tracksGrid.InvokeRequired)
      {
        ThreadSafeAddTracksDelegate d = new ThreadSafeAddTracksDelegate(AddTrack);
        tracksGrid.Invoke(d, new object[] { track });
        return;
      }

      bindingList.Add(track);
    }

    /// <summary>
    /// Create Context Menu
    /// </summary>
    private void CreateContextMenu()
    {
      // Build the Context Menu for the Grid
      MenuItem[] rmitems = new MenuItem[3];
      rmitems[0] = new MenuItem();
      rmitems[0].Text = localisation.ToString("contextmenu", "AddBurner");
      rmitems[0].Click += new System.EventHandler(tracksGrid_AddToBurner);
      rmitems[0].DefaultItem = true;
      rmitems[1] = new MenuItem();
      rmitems[1].Text = localisation.ToString("contextmenu", "AddConverter");
      rmitems[1].Click += new System.EventHandler(tracksGrid_AddToConvert);
      rmitems[2] = new MenuItem();
      rmitems[2].Text = localisation.ToString("contextmenu", "AddPlaylist");
      rmitems[2].Click += new System.EventHandler(tracksGrid_AddToPlayList);
      this.tracksGrid.ContextMenu = new ContextMenu(rmitems);
    }

    private void ShowForm(Form f)
    {
      int x = _main.ClientSize.Width / 2 - f.Width / 2;
      int y = _main.ClientSize.Height / 2 - f.Height / 2;
      Point clientLocation = _main.Location;
      x += clientLocation.X;
      y += clientLocation.Y;
      f.Location = new Point(x, y);
      f.Show();
    }
    #endregion

    #region EventHandler
    /// <summary>
    /// Handle Messages
    /// </summary>
    /// <param name="message"></param>
    private void OnMessageReceive(QueueMessage message)
    {
      string action = message.MessageData["action"] as string;

      switch (action.ToLower())
      {
        case "languagechanged":
          LanguageChanged();
          this.Refresh();
          break;
      }
    }

    /// <summary>
    /// Handles changes in the column width
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_ColumnWidthChanged(object sender, DataGridViewColumnEventArgs e)
    {
      // On startup we get sometimes an exception
      try
      {
        if (tracksGrid.Rows.Count > 0)
          tracksGrid.InvalidateRow(e.Column.Index);
      }
      catch (Exception)
      { }
    }

    /// <summary>
    /// Handles editing of data columns
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_CurrentCellDirtyStateChanged(object sender, EventArgs e)
    {
      // For combo box and check box cells, commit any value change as soon
      // as it is made rather than waiting for the focus to leave the cell.
      if (!tracksGrid.CurrentCell.OwningColumn.GetType().Equals(typeof(DataGridViewTextBoxColumn)))
      {
        tracksGrid.CommitEdit(DataGridViewDataErrorContexts.Commit);

      }
    }

    /// <summary>
    /// Only allow valid values to be entered.
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
      if (e.Exception == null) return;

      // If the user-specified value is invalid, cancel the change 
      // and display the error icon in the row header.
      if ((e.Context & DataGridViewDataErrorContexts.Commit) != 0 &&
          (typeof(FormatException).IsAssignableFrom(e.Exception.GetType()) ||
          typeof(ArgumentException).IsAssignableFrom(e.Exception.GetType())))
      {
        tracksGrid.Rows[e.RowIndex].ErrorText = localisation.ToString("message", "DataEntryError");
        e.Cancel = true;
      }
      else
      {
        // Rethrow any exceptions that aren't related to the user input.
        e.ThrowException = true;
      }
    }

    /// <summary>
    /// We're leaving a Cell after edit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {
      // Ensure that the error icon in the row header is hidden.
      tracksGrid.Rows[e.RowIndex].ErrorText = "";
    }

    /// <summary>
    /// Value of Cell has changed
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_CellValueChanged(object sender, DataGridViewCellEventArgs e)
    {
      // When changing the Status or the Header Text, ignore the Cell Changed event
      if (e.ColumnIndex == 1 || e.RowIndex == -1)
        return;

      _itemsChanged = true;
      SetBackgroundColorChanged(e.RowIndex);
      TrackData track = (TrackData)bindingList[e.RowIndex];
      track.Changed = true;
    }

    /// <summary>
    /// Clicking on the Column Header sorts by this column
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_Sorted(object sender, EventArgs e)
    {
      int i = 0;
      // Set the Color for changed rows again
      foreach (TrackData track in bindingList)
      {
        if (track.Changed)
        {
          SetBackgroundColorChanged(i);
        }
        i++;
      }
    }

    /// <summary>
    /// Handle Right Mouse Click to show Column Config Dialogue
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_ColumnHeaderMouseClick(object sender, DataGridViewCellMouseEventArgs e)
    {
      // only hndle right click on Header to show config dialogue
      if (e.Button == MouseButtons.Right)
      {
        MPTagThat.Dialogues.ColumnSelect dialog = new MPTagThat.Dialogues.ColumnSelect(this);
        dialog.Location = this.PointToScreen(new Point(20, 50));
        dialog.ShowDialog();
      }
    }

    /// <summary>
    /// We want to get Control, when editing a Cell, so that we can control the input
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_EditingControlShowing(object sender, DataGridViewEditingControlShowingEventArgs e)
    {
      DataGridView view = sender as DataGridView;
      string colName = view.CurrentCell.OwningColumn.Name;
      if (colName == "Track" || colName == "Disc")
      {
        TextBox txtbox = e.Control as TextBox;
        if (txtbox != null)
        {
          txtbox.KeyPress += new KeyPressEventHandler(txtbox_KeyPress);
        }
      }
    }

    /// <summary>
    /// Allow only Digits and the slash when enetering data for Track or Disc
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void txtbox_KeyPress(object sender, KeyPressEventArgs e)
    {
      char keyChar = e.KeyChar;

      if (!Char.IsDigit(keyChar)      // 0 - 9
         &&
         keyChar != 8               // backspace
         &&
         keyChar != 13              // enter
         &&
         keyChar != '/'
         )
      {
        //  Do not display the keystroke
        e.Handled = true;
      }
    }

    /// <summary>
    /// Handle Right Mouse Click to open the context Menu in the Grid
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_MouseClick(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Right)
      {
        Point mouse = tracksGrid.PointToClient(Cursor.Position);
        System.Windows.Forms.DataGridView.HitTestInfo selectedRow = tracksGrid.HitTest(mouse.X, mouse.Y);

        if (selectedRow.Type != DataGridViewHitTestType.ColumnHeader)
          this.tracksGrid.ContextMenu.Show(tracksGrid, new Point(e.X, e.Y));
      }
    }

    /// <summary>
    /// Double Click on a Row.
    /// Start Single Edit
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    private void tracksGrid_MouseDoubleClick(object sender, MouseEventArgs e)
    {
      MPTagThat.TagEdit.SingleTagEdit dlgSingleTagedit = new MPTagThat.TagEdit.SingleTagEdit(_main);
      Form f = (Form)dlgSingleTagedit;
      int x = (_main.ClientSize.Width / 2) - (f.Width / 2);
      int y = (_main.ClientSize.Height / 2) - (f.Height / 2);
      Point clientLocation = _main.Location;
      x += clientLocation.X;
      y += clientLocation.Y;

      f.Location = new Point(x, y);
      f.ShowDialog();
    }

    /// <summary>
    /// Add to Burner
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    private void tracksGrid_AddToBurner(object o, System.EventArgs e)
    {
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (!row.Selected)
          continue;

        TrackData track = bindingList[row.Index];
        _main.BurnGridView.AddToBurner(track);
      }
    }

    /// <summary>
    /// Add to Converter Grid
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    private void tracksGrid_AddToConvert(object o, System.EventArgs e)
    {
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (!row.Selected)
          continue;

        TrackData track = bindingList[row.Index];
        _main.ConvertGridView.AddToConvert(track);
      }
    }

    /// <summary>
    /// Add to Playlist
    /// </summary>
    /// <param name="o"></param>
    /// <param name="e"></param>
    private void tracksGrid_AddToPlayList(object o, System.EventArgs e)
    {
      foreach (DataGridViewRow row in tracksGrid.Rows)
      {
        if (!row.Selected)
          continue;

        TrackData track = bindingList[row.Index];
        PlayListData playListItem = new PlayListData();
        playListItem.FileName = track.FullFileName;
        playListItem.Title = string.Format("{0} - {1}", track.Artist, track.Title);
        playListItem.Duration = track.Duration.Substring(3, 5);  // Just get Minutes and seconds
        _main.Player.PlayList.Add(playListItem);
      }
    }

    /// <summary>
    /// Handle the Background Color for the Rating Image Cell
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_SelectionChanged(object sender, EventArgs e)
    {
      for (int i = 0; i < tracksGrid.Rows.Count; i++)
      {
        if (tracksGrid.Rows[i].Selected)
          tracksGrid.Rows[i].Cells[10].Style.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.SelectionBackColor;
        else
        {
          if (bindingList[i].Changed)
          {
            tracksGrid.Rows[i].Cells[10].Style.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.ChangedBackColor;
          }
          else
          {
            if (i % 2 == 0)
              tracksGrid.Rows[i].Cells[10].Style.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.DefaultBackColor;
            else
              tracksGrid.Rows[i].Cells[10].Style.BackColor = ServiceScope.Get<IThemeManager>().CurrentTheme.AlternatingRowBackColor;
          }
        }
      }
    }

    /// <summary>
    /// Handle Drag and Drop Operation
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_MouseDown(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left && tracksGrid.SelectedRows.Count > 0)
      {
        // Remember the point where the mouse down occurred. The DragSize indicates
        // the size that the mouse can move before a drag event should be started.                
        Size dragSize = SystemInformation.DragSize;

        // Create a rectangle using the DragSize, with the mouse position being
        // at the center of the rectangle.
        _dragBoxFromMouseDown = new Rectangle(new Point(e.X - (dragSize.Width / 2), e.Y - (dragSize.Height / 2)), dragSize);
      }
      else
        _dragBoxFromMouseDown = Rectangle.Empty;
    }

    /// <summary>
    /// The mouse moves. Do a Drag & Drop if necessary
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_MouseMove(object sender, MouseEventArgs e)
    {
      if (e.Button == MouseButtons.Left)
      {
        // If the mouse moves outside the rectangle, start the drag.
        if (_dragBoxFromMouseDown != Rectangle.Empty &&
            !_dragBoxFromMouseDown.Contains(e.X, e.Y))
        {

          // The screenOffset is used to account for any desktop bands 
          // that may be at the top or left side of the screen when 
          // determining when to cancel the drag drop operation.
          _screenOffset = SystemInformation.WorkingArea.Location;

          List<PlayListData> selectedRows = new List<PlayListData>();
          foreach (DataGridViewRow row in tracksGrid.Rows)
          {
            if (!row.Selected)
              continue;

            TrackData track = bindingList[row.Index];
            PlayListData playListItem = new PlayListData();
            playListItem.FileName = track.FullFileName;
            playListItem.Title = string.Format("{0} - {1}", track.Artist, track.Title);
            playListItem.Duration = track.Duration.Substring(3, 5);  // Just get Minutes and seconds
            selectedRows.Add(playListItem);
          }
          tracksGrid.DoDragDrop(selectedRows, DragDropEffects.Copy);
        }
      }
    }

    /// <summary>
    /// The Mouse has been released
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_MouseUp(object sender, MouseEventArgs e)
    {
      // Reset the drag rectangle when the mouse button is raised.
      _dragBoxFromMouseDown = Rectangle.Empty;
    }

    /// <summary>
    /// Determines, if Drag and drop should continue
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    void tracksGrid_QueryContinueDrag(object sender, QueryContinueDragEventArgs e)
    {
      DataGridView dg = sender as DataGridView;

      if (dg != null)
      {
        Form f = dg.FindForm();
        // Cancel the drag if the mouse moves off the form. The screenOffset
        // takes into account any desktop bands that may be at the top or left
        // side of the screen.
        if (((Control.MousePosition.X - _screenOffset.X) < f.DesktopBounds.Left) ||
            ((Control.MousePosition.X - _screenOffset.X) > f.DesktopBounds.Right) ||
            ((Control.MousePosition.Y - _screenOffset.Y) < f.DesktopBounds.Top) ||
            ((Control.MousePosition.Y - _screenOffset.Y) > f.DesktopBounds.Bottom))
        {

          e.Action = DragAction.Cancel;
        }
      }
    }
    #endregion
  }
}
