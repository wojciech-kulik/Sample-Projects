using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Live;
using System.Collections.Generic;
using Microsoft.Live.Controls;
using System.IO;
namespace ClientsManager.ViewModels
{
    public class SkyDriveManager
    {
        private object _syncRoot = new object();
        #region fields

        public bool IsWorking {get;private set;}
        public bool Logged = false;
        Action<SkyDriveResult<List<SkyDriveFolder>>> _getFolderCallback;
        Action<SkyDriveResult<bool>> _uploadCallback;


        public LiveConnectClient LiveClient;

        #endregion

        #region FolderID - current folder

        public bool GetFolderAsync(string folderId, Action<SkyDriveResult<List<SkyDriveFolder>>> callback)
        {
            if (!String.IsNullOrEmpty(folderId) && Logged)
            {
                lock (_syncRoot)
                {
                    if (!IsWorking)
                    {
                        _getFolderCallback= callback;
                        IsWorking = true;
                        LiveClient.GetAsync(folderId + "/files?filter=folders,albums");
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region upload

        public bool Upload(string currentFolderID, string remoteFileName ,Stream fileStream, OverwriteOption overwriteOption,Action<SkyDriveResult<bool>> callback)
        {
            lock (_syncRoot)
            {
                if (!IsWorking && Logged)
                {
                    IsWorking = true;
                    _uploadCallback = callback;
                    LiveClient.UploadAsync(currentFolderID, remoteFileName, fileStream, overwriteOption);
                    return true;
                }
            }
            return false;
        }

        void LiveClient_UploadCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
            Action<SkyDriveResult<bool>> callback;
            lock(_syncRoot)
            {
                IsWorking=false;
                callback = _uploadCallback;
            }
            callback(new SkyDriveResult<bool>() { Exception=e.Error});
        }

        #endregion

        #region OnSessionChanged

        public bool RegisterSessionChange(object sender, LiveConnectSessionChangedEventArgs e)
        {
            lock (_syncRoot)
            {
                if (e.Status == LiveConnectSessionStatus.Connected)
                {
                    LiveClient = new LiveConnectClient(e.Session);
                    LiveClient.GetCompleted += LiveClient_GetCompleted;
                    LiveClient.UploadCompleted += new EventHandler<LiveOperationCompletedEventArgs>(LiveClient_UploadCompleted);

                    Logged = true;

                    
                }
                else
                {
                    Logged = false;
                }
            }
            return Logged;
        }

   

        #endregion

        #region When downloading list of files is completed

        private void LiveClient_GetCompleted(object sender, LiveOperationCompletedEventArgs e)
        {
                List<object> data = (List<object>)e.Result["data"];
                List<SkyDriveFolder> result = new List<SkyDriveFolder>();
                foreach (IDictionary<string, object> content in data)
                {
                    result.Add(new SkyDriveFolder() { Name = (string)content["name"], ID = (string)content["id"] });
                }
                Action<SkyDriveResult<List<SkyDriveFolder>>> callback;
                lock (_syncRoot)
                {
                    callback = _getFolderCallback;
                    IsWorking = false;
                }
                callback(new SkyDriveResult<List<SkyDriveFolder>>() { Exception = e.Error, Result = result });
        }

        #endregion
    }
}
