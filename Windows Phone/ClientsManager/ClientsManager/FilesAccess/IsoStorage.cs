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
using System.Windows.Media.Imaging;
using System.IO.IsolatedStorage;
using System.IO;

namespace ClientsManager.FilesAccess
{
    public sealed class IsoStorageHelper
    {

        public static BitmapImage PhotoFromFile (string path)
        {
            BitmapImage result = new BitmapImage();
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (String.IsNullOrEmpty(path) || !storage.FileExists(path)) return null;

                using (IsolatedStorageFileStream isoStream = storage.OpenFile(path, FileMode.Open, FileAccess.Read))
                    result.SetSource(isoStream);
            }

            return result;
        }

        //returns photo path (relative to IsolatedStorage)
        public static string SavePhoto(BitmapImage photo, long clientID, string fileName)
        {
            if (!String.IsNullOrEmpty(fileName) && photo != null)
            {
                string path = "Images\\" + clientID.ToString() + "_" + fileName;

                using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
                {
                    storage.CreateDirectory("Images");
                    using (FileStream fs = storage.OpenFile(path, FileMode.Create))
                        Extensions.SaveJpeg(new WriteableBitmap(photo), fs, photo.PixelWidth, photo.PixelHeight, 0, 100);
                }

                return path;
            }
            else
                return null;
        }

        public static void DeleteFile(string path)
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (String.IsNullOrWhiteSpace(path) || !storage.FileExists(path)) return;
                storage.DeleteFile(path);
            }
        }

        public static IsolatedStorageFileStream GetDatabaseStream()
        {
            using (IsolatedStorageFile storage = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (!storage.FileExists("ClientsManager.sdf"))
                {
                    MessageBox.Show("Database file doesn't exists.");
                    return null;
                }

                IsoStorageHelper.DeleteFile("ClientsManager_backup.sdf");

                //cannot access active database file, so have to make a copy
                storage.CopyFile("ClientsManager.sdf", "ClientsManager_backup.sdf");

                return storage.OpenFile("ClientsManager_backup.sdf", FileMode.Open, FileAccess.Read);
            } 
        }
    }
}
