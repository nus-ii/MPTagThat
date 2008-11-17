#region Copyright (C) 2007-2008 Team MediaPortal

/*
    Copyright (C) 2007-2008 Team MediaPortal
    http://www.team-mediaportal.com
 
    This file is part of MediaPortal II

    MediaPortal II is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    MediaPortal II is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with MediaPortal II.  If not, see <http://www.gnu.org/licenses/>.
*/

#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.IO;

namespace MPTagThat.Core.MediaChangeMonitor
{
  public class MediaChangeMonitor : IMediaChangeMonitor
  {
    #region Event delegates
    public event MediaInsertedEvent MediaInserted;
    public event MediaRemovedEvent MediaRemoved;
    #endregion

    #region Variables
    private ILogger Logger;
    private DeviceVolumeMonitor _deviceMonitor;
    #endregion

    #region ctor / dtor
    public MediaChangeMonitor()
    {
      Logger = ServiceScope.Get<ILogger>();
    }

    ~MediaChangeMonitor()
    {
      if (_deviceMonitor != null)
        _deviceMonitor.Dispose();

      _deviceMonitor = null;
    }
    #endregion

    #region IMediaChangeMonitor implementation
    public void StartListening(IntPtr aHandle)
    {
      try
      {
        _deviceMonitor = new DeviceVolumeMonitor(aHandle);
        _deviceMonitor.OnVolumeInserted += new DeviceVolumeAction(VolumeInserted);
        _deviceMonitor.OnVolumeRemoved += new DeviceVolumeAction(VolumeRemoved);
        _deviceMonitor.AsynchronousEvents = true;
        _deviceMonitor.Enabled = true;

        Logger.Info("MediaChangeMonitor: Monitoring System for Media Changes");
      }
      catch (DeviceVolumeMonitorException ex)
      {
        Logger.Error("MediaChangeMonitor: Error enabling MediaChangeMonitor Service. {0}", ex.Message);
      }
    }

    public void StopListening()
    {
      if (_deviceMonitor != null)
        _deviceMonitor.Dispose();

      _deviceMonitor = null;
    }
    #endregion

    #region Events
    /// <summary>
    /// The event that gets triggered whenever a new volume is inserted.
    /// </summary>	
    private void VolumeInserted(int bitMask)
    {
      string driveLetter = _deviceMonitor.MaskToLogicalPaths(bitMask);
      Logger.Info("MediaChangeMonitor: Media inserted in drive {0}", driveLetter);

      if (MediaInserted != null)
        MediaInserted(driveLetter);
    }

    /// <summary>
    /// The event that gets triggered whenever a volume is removed.
    /// </summary>	
    private void VolumeRemoved(int bitMask)
    {
      string driveLetter = _deviceMonitor.MaskToLogicalPaths(bitMask);
      Logger.Info("MediaChangeMonitor: Media removed from drive {0}", driveLetter);

      if (MediaRemoved != null)
        MediaRemoved(driveLetter);
    }
    #endregion
  }
}
