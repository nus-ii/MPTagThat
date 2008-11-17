namespace MPTagThat.TagToFileName
{
  partial class TagToFileName
  {
    /// <summary>
    /// Required designer variable.
    /// </summary>
    private System.ComponentModel.IContainer components = null;

    /// <summary>
    /// Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
      if (disposing && (components != null))
      {
        components.Dispose();
      }
      base.Dispose(disposing);
    }

    #region Windows Form Designer generated code

    /// <summary>
    /// Required method for Designer support - do not modify
    /// the contents of this method with the code editor.
    /// </summary>
    private void InitializeComponent()
    {
      this.btRemoveFormat = new MPTagThat.Core.WinControls.MPTButton();
      this.btAddFormat = new MPTagThat.Core.WinControls.MPTButton();
      this.cbFormat = new System.Windows.Forms.ComboBox();
      this.lblFormat = new MPTagThat.Core.WinControls.MPTLabel();
      this.btApply = new MPTagThat.Core.WinControls.MPTButton();
      this.btCancel = new MPTagThat.Core.WinControls.MPTButton();
      this.groupBoxParm = new MPTagThat.Core.WinControls.MPTGroupBox();
      this.lblParmEnumerate = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblModifiedBy = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblBPM = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblSubTitle = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblContentGroup = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblComposer = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblConductor = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmFileName = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmComment = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblAlbumArtist = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmGenre = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmDiscTotal = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmDisc = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmTrackTotal = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmTrack = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmYear = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmAlbum = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmTitle = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblParmArtist = new MPTagThat.Core.WinControls.MPTLabel();
      this.lblStartAt = new MPTagThat.Core.WinControls.MPTLabel();
      this.numericUpDownStartAt = new System.Windows.Forms.NumericUpDown();
      this.numericUpDownNumberDigits = new System.Windows.Forms.NumericUpDown();
      this.lblNumberDigits = new MPTagThat.Core.WinControls.MPTLabel();
      this.groupBoxParm.SuspendLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAt)).BeginInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberDigits)).BeginInit();
      this.SuspendLayout();
      // 
      // btRemoveFormat
      // 
      this.btRemoveFormat.Localisation = "RemoveFormat";
      this.btRemoveFormat.LocalisationContext = "TagAndRename";
      this.btRemoveFormat.Location = new System.Drawing.Point(377, 74);
      this.btRemoveFormat.Name = "btRemoveFormat";
      this.btRemoveFormat.Size = new System.Drawing.Size(235, 23);
      this.btRemoveFormat.TabIndex = 2;
      this.btRemoveFormat.Text = "Remove Format From List";
      this.btRemoveFormat.UseVisualStyleBackColor = true;
      this.btRemoveFormat.Click += new System.EventHandler(this.btRemoveFormat_Click);
      // 
      // btAddFormat
      // 
      this.btAddFormat.Localisation = "AddFormat";
      this.btAddFormat.LocalisationContext = "TagAndRename";
      this.btAddFormat.Location = new System.Drawing.Point(95, 74);
      this.btAddFormat.Name = "btAddFormat";
      this.btAddFormat.Size = new System.Drawing.Size(235, 23);
      this.btAddFormat.TabIndex = 1;
      this.btAddFormat.Text = "Add Format To List";
      this.btAddFormat.UseVisualStyleBackColor = true;
      this.btAddFormat.Click += new System.EventHandler(this.btAddFormat_Click);
      // 
      // cbFormat
      // 
      this.cbFormat.FormattingEnabled = true;
      this.cbFormat.Location = new System.Drawing.Point(95, 37);
      this.cbFormat.Name = "cbFormat";
      this.cbFormat.Size = new System.Drawing.Size(517, 21);
      this.cbFormat.TabIndex = 0;
      this.cbFormat.KeyPress += new System.Windows.Forms.KeyPressEventHandler(this.cbFormat_Keypress);
      // 
      // lblFormat
      // 
      this.lblFormat.AutoSize = true;
      this.lblFormat.Localisation = "Format";
      this.lblFormat.LocalisationContext = "TagAndRename";
      this.lblFormat.Location = new System.Drawing.Point(29, 40);
      this.lblFormat.Name = "lblFormat";
      this.lblFormat.Size = new System.Drawing.Size(42, 13);
      this.lblFormat.TabIndex = 13;
      this.lblFormat.Text = "Format:";
      // 
      // btApply
      // 
      this.btApply.Localisation = "Apply";
      this.btApply.LocalisationContext = "TagAndRename";
      this.btApply.Location = new System.Drawing.Point(200, 409);
      this.btApply.Name = "btApply";
      this.btApply.Size = new System.Drawing.Size(99, 23);
      this.btApply.TabIndex = 3;
      this.btApply.Text = "Apply";
      this.btApply.UseVisualStyleBackColor = true;
      this.btApply.Click += new System.EventHandler(this.btApply_Click);
      // 
      // btCancel
      // 
      this.btCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
      this.btCancel.Localisation = "Cancel";
      this.btCancel.LocalisationContext = "TagAndRename";
      this.btCancel.Location = new System.Drawing.Point(420, 409);
      this.btCancel.Name = "btCancel";
      this.btCancel.Size = new System.Drawing.Size(99, 23);
      this.btCancel.TabIndex = 4;
      this.btCancel.Text = "Cancel";
      this.btCancel.UseVisualStyleBackColor = true;
      this.btCancel.Click += new System.EventHandler(this.btCancel_Click);
      // 
      // groupBoxParm
      // 
      this.groupBoxParm.Controls.Add(this.numericUpDownNumberDigits);
      this.groupBoxParm.Controls.Add(this.lblNumberDigits);
      this.groupBoxParm.Controls.Add(this.numericUpDownStartAt);
      this.groupBoxParm.Controls.Add(this.lblStartAt);
      this.groupBoxParm.Controls.Add(this.lblParmEnumerate);
      this.groupBoxParm.Controls.Add(this.lblModifiedBy);
      this.groupBoxParm.Controls.Add(this.lblBPM);
      this.groupBoxParm.Controls.Add(this.lblSubTitle);
      this.groupBoxParm.Controls.Add(this.lblContentGroup);
      this.groupBoxParm.Controls.Add(this.lblComposer);
      this.groupBoxParm.Controls.Add(this.lblConductor);
      this.groupBoxParm.Controls.Add(this.lblParmFileName);
      this.groupBoxParm.Controls.Add(this.lblParmComment);
      this.groupBoxParm.Controls.Add(this.lblAlbumArtist);
      this.groupBoxParm.Controls.Add(this.lblParmGenre);
      this.groupBoxParm.Controls.Add(this.lblParmDiscTotal);
      this.groupBoxParm.Controls.Add(this.lblParmDisc);
      this.groupBoxParm.Controls.Add(this.lblParmTrackTotal);
      this.groupBoxParm.Controls.Add(this.lblParmTrack);
      this.groupBoxParm.Controls.Add(this.lblParmYear);
      this.groupBoxParm.Controls.Add(this.lblParmAlbum);
      this.groupBoxParm.Controls.Add(this.lblParmTitle);
      this.groupBoxParm.Controls.Add(this.lblParmArtist);
      this.groupBoxParm.Localisation = "GroupBoxParm";
      this.groupBoxParm.LocalisationContext = "TagAndRename";
      this.groupBoxParm.Location = new System.Drawing.Point(18, 122);
      this.groupBoxParm.Name = "groupBoxParm";
      this.groupBoxParm.Size = new System.Drawing.Size(748, 264);
      this.groupBoxParm.TabIndex = 22;
      this.groupBoxParm.TabStop = false;
      this.groupBoxParm.Text = "Parameters (Click to add to the list)";
      // 
      // lblParmEnumerate
      // 
      this.lblParmEnumerate.AutoSize = true;
      this.lblParmEnumerate.Localisation = "Enumerate";
      this.lblParmEnumerate.LocalisationContext = "TagAndRename";
      this.lblParmEnumerate.Location = new System.Drawing.Point(6, 190);
      this.lblParmEnumerate.Name = "lblParmEnumerate";
      this.lblParmEnumerate.Size = new System.Drawing.Size(113, 13);
      this.lblParmEnumerate.TabIndex = 19;
      this.lblParmEnumerate.Text = "<#> = Enumerate Files";
      this.lblParmEnumerate.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblModifiedBy
      // 
      this.lblModifiedBy.AutoSize = true;
      this.lblModifiedBy.Localisation = "ModifiedBy";
      this.lblModifiedBy.LocalisationContext = "TagAndRename";
      this.lblModifiedBy.Location = new System.Drawing.Point(6, 137);
      this.lblModifiedBy.Name = "lblModifiedBy";
      this.lblModifiedBy.Size = new System.Drawing.Size(141, 13);
      this.lblModifiedBy.TabIndex = 18;
      this.lblModifiedBy.Text = "<M> = Modified / remixed by";
      this.lblModifiedBy.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblBPM
      // 
      this.lblBPM.AutoSize = true;
      this.lblBPM.Localisation = "BPM";
      this.lblBPM.LocalisationContext = "TagAndRename";
      this.lblBPM.Location = new System.Drawing.Point(271, 137);
      this.lblBPM.Name = "lblBPM";
      this.lblBPM.Size = new System.Drawing.Size(61, 13);
      this.lblBPM.TabIndex = 17;
      this.lblBPM.Text = "<E> = BPM";
      this.lblBPM.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblSubTitle
      // 
      this.lblSubTitle.AutoSize = true;
      this.lblSubTitle.Localisation = "SubTitle";
      this.lblSubTitle.LocalisationContext = "TagAndRename";
      this.lblSubTitle.Location = new System.Drawing.Point(533, 111);
      this.lblSubTitle.Name = "lblSubTitle";
      this.lblSubTitle.Size = new System.Drawing.Size(77, 13);
      this.lblSubTitle.TabIndex = 16;
      this.lblSubTitle.Text = "<S> = SubTitle";
      this.lblSubTitle.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblContentGroup
      // 
      this.lblContentGroup.AutoSize = true;
      this.lblContentGroup.Localisation = "Group";
      this.lblContentGroup.LocalisationContext = "TagAndRename";
      this.lblContentGroup.Location = new System.Drawing.Point(533, 87);
      this.lblContentGroup.Name = "lblContentGroup";
      this.lblContentGroup.Size = new System.Drawing.Size(108, 13);
      this.lblContentGroup.TabIndex = 15;
      this.lblContentGroup.Text = "<U> = Content Group";
      this.lblContentGroup.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblComposer
      // 
      this.lblComposer.AutoSize = true;
      this.lblComposer.Localisation = "Composer";
      this.lblComposer.LocalisationContext = "TagAndRename";
      this.lblComposer.Location = new System.Drawing.Point(533, 64);
      this.lblComposer.Name = "lblComposer";
      this.lblComposer.Size = new System.Drawing.Size(86, 13);
      this.lblComposer.TabIndex = 14;
      this.lblComposer.Text = "<R> = Composer";
      this.lblComposer.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblConductor
      // 
      this.lblConductor.AutoSize = true;
      this.lblConductor.Localisation = "Conductor";
      this.lblConductor.LocalisationContext = "TagAndRename";
      this.lblConductor.Location = new System.Drawing.Point(271, 64);
      this.lblConductor.Name = "lblConductor";
      this.lblConductor.Size = new System.Drawing.Size(88, 13);
      this.lblConductor.TabIndex = 13;
      this.lblConductor.Text = "<N> = Conductor";
      this.lblConductor.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmFileName
      // 
      this.lblParmFileName.AutoSize = true;
      this.lblParmFileName.Localisation = "Filename";
      this.lblParmFileName.LocalisationContext = "TagAndRename";
      this.lblParmFileName.Location = new System.Drawing.Point(533, 137);
      this.lblParmFileName.Name = "lblParmFileName";
      this.lblParmFileName.Size = new System.Drawing.Size(116, 13);
      this.lblParmFileName.TabIndex = 11;
      this.lblParmFileName.Text = "<F> = Current Filename";
      this.lblParmFileName.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmComment
      // 
      this.lblParmComment.AutoSize = true;
      this.lblParmComment.Localisation = "Comment";
      this.lblParmComment.LocalisationContext = "TagAndRename";
      this.lblParmComment.Location = new System.Drawing.Point(533, 41);
      this.lblParmComment.Name = "lblParmComment";
      this.lblParmComment.Size = new System.Drawing.Size(82, 13);
      this.lblParmComment.TabIndex = 10;
      this.lblParmComment.Text = "<C> = Comment";
      this.lblParmComment.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblAlbumArtist
      // 
      this.lblAlbumArtist.AutoSize = true;
      this.lblAlbumArtist.Localisation = "AlbumArtist";
      this.lblAlbumArtist.LocalisationContext = "TagAndRename";
      this.lblAlbumArtist.Location = new System.Drawing.Point(6, 64);
      this.lblAlbumArtist.Name = "lblAlbumArtist";
      this.lblAlbumArtist.Size = new System.Drawing.Size(151, 13);
      this.lblAlbumArtist.TabIndex = 9;
      this.lblAlbumArtist.Text = "<O> = Orchestra / Album Artist";
      this.lblAlbumArtist.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmGenre
      // 
      this.lblParmGenre.AutoSize = true;
      this.lblParmGenre.Localisation = "Genre";
      this.lblParmGenre.LocalisationContext = "TagAndRename";
      this.lblParmGenre.Location = new System.Drawing.Point(271, 41);
      this.lblParmGenre.Name = "lblParmGenre";
      this.lblParmGenre.Size = new System.Drawing.Size(68, 13);
      this.lblParmGenre.TabIndex = 8;
      this.lblParmGenre.Text = "<G> = Genre";
      this.lblParmGenre.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmDiscTotal
      // 
      this.lblParmDiscTotal.AutoSize = true;
      this.lblParmDiscTotal.Localisation = "DiscTotal";
      this.lblParmDiscTotal.LocalisationContext = "TagAndRename";
      this.lblParmDiscTotal.Location = new System.Drawing.Point(271, 111);
      this.lblParmDiscTotal.Name = "lblParmDiscTotal";
      this.lblParmDiscTotal.Size = new System.Drawing.Size(110, 13);
      this.lblParmDiscTotal.TabIndex = 7;
      this.lblParmDiscTotal.Text = "<d> = Total # of discs";
      this.lblParmDiscTotal.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmDisc
      // 
      this.lblParmDisc.AutoSize = true;
      this.lblParmDisc.Localisation = "Disc";
      this.lblParmDisc.LocalisationContext = "TagAndRename";
      this.lblParmDisc.Location = new System.Drawing.Point(6, 111);
      this.lblParmDisc.Name = "lblParmDisc";
      this.lblParmDisc.Size = new System.Drawing.Size(100, 13);
      this.lblParmDisc.TabIndex = 6;
      this.lblParmDisc.Text = "<D> = Disc Number";
      this.lblParmDisc.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmTrackTotal
      // 
      this.lblParmTrackTotal.AutoSize = true;
      this.lblParmTrackTotal.Localisation = "TrackTotal";
      this.lblParmTrackTotal.LocalisationContext = "TagAndRename";
      this.lblParmTrackTotal.Location = new System.Drawing.Point(271, 87);
      this.lblParmTrackTotal.Name = "lblParmTrackTotal";
      this.lblParmTrackTotal.Size = new System.Drawing.Size(115, 13);
      this.lblParmTrackTotal.TabIndex = 5;
      this.lblParmTrackTotal.Text = "<k> = Total # of tracks";
      this.lblParmTrackTotal.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmTrack
      // 
      this.lblParmTrack.AutoSize = true;
      this.lblParmTrack.Localisation = "Track";
      this.lblParmTrack.LocalisationContext = "TagAndRename";
      this.lblParmTrack.Location = new System.Drawing.Point(6, 87);
      this.lblParmTrack.Name = "lblParmTrack";
      this.lblParmTrack.Size = new System.Drawing.Size(106, 13);
      this.lblParmTrack.TabIndex = 4;
      this.lblParmTrack.Text = "<K> = Track Number";
      this.lblParmTrack.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmYear
      // 
      this.lblParmYear.AutoSize = true;
      this.lblParmYear.Localisation = "Year";
      this.lblParmYear.LocalisationContext = "TagAndRename";
      this.lblParmYear.Location = new System.Drawing.Point(6, 41);
      this.lblParmYear.Name = "lblParmYear";
      this.lblParmYear.Size = new System.Drawing.Size(60, 13);
      this.lblParmYear.TabIndex = 3;
      this.lblParmYear.Text = "<Y> = Year";
      this.lblParmYear.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmAlbum
      // 
      this.lblParmAlbum.AutoSize = true;
      this.lblParmAlbum.Localisation = "Album";
      this.lblParmAlbum.LocalisationContext = "TagAndRename";
      this.lblParmAlbum.Location = new System.Drawing.Point(533, 17);
      this.lblParmAlbum.Name = "lblParmAlbum";
      this.lblParmAlbum.Size = new System.Drawing.Size(67, 13);
      this.lblParmAlbum.TabIndex = 2;
      this.lblParmAlbum.Text = "<B> = Album";
      this.lblParmAlbum.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmTitle
      // 
      this.lblParmTitle.AutoSize = true;
      this.lblParmTitle.Localisation = "Title";
      this.lblParmTitle.LocalisationContext = "TagAndRename";
      this.lblParmTitle.Location = new System.Drawing.Point(271, 16);
      this.lblParmTitle.Name = "lblParmTitle";
      this.lblParmTitle.Size = new System.Drawing.Size(58, 13);
      this.lblParmTitle.TabIndex = 1;
      this.lblParmTitle.Text = "<T> = Title";
      this.lblParmTitle.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblParmArtist
      // 
      this.lblParmArtist.AutoSize = true;
      this.lblParmArtist.Localisation = "Artist";
      this.lblParmArtist.LocalisationContext = "TagAndRename";
      this.lblParmArtist.Location = new System.Drawing.Point(6, 17);
      this.lblParmArtist.Name = "lblParmArtist";
      this.lblParmArtist.Size = new System.Drawing.Size(61, 13);
      this.lblParmArtist.TabIndex = 0;
      this.lblParmArtist.Text = "<A> = Artist";
      this.lblParmArtist.Click += new System.EventHandler(this.lblParm_Click);
      // 
      // lblStartAt
      // 
      this.lblStartAt.AutoSize = true;
      this.lblStartAt.Localisation = "StartAt";
      this.lblStartAt.LocalisationContext = "TagAndRename";
      this.lblStartAt.Location = new System.Drawing.Point(342, 175);
      this.lblStartAt.Name = "lblStartAt";
      this.lblStartAt.Size = new System.Drawing.Size(41, 13);
      this.lblStartAt.TabIndex = 20;
      this.lblStartAt.Text = "Start at";
      // 
      // numericUpDownStartAt
      // 
      this.numericUpDownStartAt.Location = new System.Drawing.Point(274, 173);
      this.numericUpDownStartAt.Maximum = new decimal(new int[] {
            9999,
            0,
            0,
            0});
      this.numericUpDownStartAt.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownStartAt.Name = "numericUpDownStartAt";
      this.numericUpDownStartAt.Size = new System.Drawing.Size(62, 20);
      this.numericUpDownStartAt.TabIndex = 21;
      this.numericUpDownStartAt.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
      // 
      // numericUpDownNumberDigits
      // 
      this.numericUpDownNumberDigits.Location = new System.Drawing.Point(274, 199);
      this.numericUpDownNumberDigits.Maximum = new decimal(new int[] {
            10,
            0,
            0,
            0});
      this.numericUpDownNumberDigits.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
      this.numericUpDownNumberDigits.Name = "numericUpDownNumberDigits";
      this.numericUpDownNumberDigits.Size = new System.Drawing.Size(62, 20);
      this.numericUpDownNumberDigits.TabIndex = 23;
      this.numericUpDownNumberDigits.Value = new decimal(new int[] {
            2,
            0,
            0,
            0});
      // 
      // lblNumberDigits
      // 
      this.lblNumberDigits.AutoSize = true;
      this.lblNumberDigits.Localisation = "NumberDigits";
      this.lblNumberDigits.LocalisationContext = "TagAndRename";
      this.lblNumberDigits.Location = new System.Drawing.Point(342, 201);
      this.lblNumberDigits.Name = "lblNumberDigits";
      this.lblNumberDigits.Size = new System.Drawing.Size(85, 13);
      this.lblNumberDigits.TabIndex = 22;
      this.lblNumberDigits.Text = "Number of Digits";
      // 
      // TagToFileName
      // 
      this.AcceptButton = this.btApply;
      this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
      this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
      this.BackColor = System.Drawing.Color.LightSteelBlue;
      this.CancelButton = this.btCancel;
      this.ClientSize = new System.Drawing.Size(784, 444);
      this.Controls.Add(this.groupBoxParm);
      this.Controls.Add(this.btCancel);
      this.Controls.Add(this.btApply);
      this.Controls.Add(this.btRemoveFormat);
      this.Controls.Add(this.btAddFormat);
      this.Controls.Add(this.cbFormat);
      this.Controls.Add(this.lblFormat);
      this.MaximizeBox = false;
      this.MinimizeBox = false;
      this.Name = "TagToFileName";
      this.StartPosition = System.Windows.Forms.FormStartPosition.Manual;
      this.Text = "TagToFileName";
      this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.OnClose);
      this.groupBoxParm.ResumeLayout(false);
      this.groupBoxParm.PerformLayout();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownStartAt)).EndInit();
      ((System.ComponentModel.ISupportInitialize)(this.numericUpDownNumberDigits)).EndInit();
      this.ResumeLayout(false);
      this.PerformLayout();

    }

    #endregion

    private MPTagThat.Core.WinControls.MPTButton btRemoveFormat;
    private MPTagThat.Core.WinControls.MPTButton btAddFormat;
    private System.Windows.Forms.ComboBox cbFormat;
    private MPTagThat.Core.WinControls.MPTLabel lblFormat;
    private MPTagThat.Core.WinControls.MPTButton btApply;
    private MPTagThat.Core.WinControls.MPTButton btCancel;
    private MPTagThat.Core.WinControls.MPTGroupBox groupBoxParm;
    private MPTagThat.Core.WinControls.MPTLabel lblModifiedBy;
    private MPTagThat.Core.WinControls.MPTLabel lblBPM;
    private MPTagThat.Core.WinControls.MPTLabel lblSubTitle;
    private MPTagThat.Core.WinControls.MPTLabel lblContentGroup;
    private MPTagThat.Core.WinControls.MPTLabel lblComposer;
    private MPTagThat.Core.WinControls.MPTLabel lblConductor;
    private MPTagThat.Core.WinControls.MPTLabel lblParmFileName;
    private MPTagThat.Core.WinControls.MPTLabel lblParmComment;
    private MPTagThat.Core.WinControls.MPTLabel lblAlbumArtist;
    private MPTagThat.Core.WinControls.MPTLabel lblParmGenre;
    private MPTagThat.Core.WinControls.MPTLabel lblParmDiscTotal;
    private MPTagThat.Core.WinControls.MPTLabel lblParmDisc;
    private MPTagThat.Core.WinControls.MPTLabel lblParmTrackTotal;
    private MPTagThat.Core.WinControls.MPTLabel lblParmTrack;
    private MPTagThat.Core.WinControls.MPTLabel lblParmYear;
    private MPTagThat.Core.WinControls.MPTLabel lblParmAlbum;
    private MPTagThat.Core.WinControls.MPTLabel lblParmTitle;
    private MPTagThat.Core.WinControls.MPTLabel lblParmArtist;
    private MPTagThat.Core.WinControls.MPTLabel lblParmEnumerate;
    private System.Windows.Forms.NumericUpDown numericUpDownNumberDigits;
    private MPTagThat.Core.WinControls.MPTLabel lblNumberDigits;
    private System.Windows.Forms.NumericUpDown numericUpDownStartAt;
    private MPTagThat.Core.WinControls.MPTLabel lblStartAt;
  }
}