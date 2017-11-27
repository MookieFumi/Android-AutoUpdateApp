using System;
using Android.Content;
using Android.Util;
using Java.IO;
using Java.Net;

namespace AutoUpdateApp
{
    public class AutoUpdater
    {
        private readonly Context _context;
        private readonly string _url;
        private readonly string _packageName;
        private readonly Version _currentVersion;

        public AutoUpdater(Context context, string url, string packageName, string currentVersionName)
        {
            _currentVersion = new Version(currentVersionName);
            _context = context;
            _packageName = packageName;
            _url = url.EndsWith("/") ? url : url + "/";
        }

        public void Update()
        {
            var availableVersion = GetAvailableVersion();
            if (_currentVersion.CompareTo(availableVersion) < 0)
            {
                LaunchAppUpdate();
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("No app update available");
            }
        }

        private void Update(string path)
        {
            var intent = new Intent();
            intent.SetAction(Intent.ActionView);
            intent.SetDataAndType(Android.Net.Uri.FromFile(new Java.IO.File(path)), "application/vnd.android.package-archive");
            _context.StartActivity(intent);
        }

        private void LaunchAppUpdate()
        {
            var apkFile = GetApkFile();
            Update(apkFile);
        }

        private Version GetAvailableVersion()
        {
            var availableVersionFile = GetAvailableVersionFile();
            var content = System.IO.File.ReadAllText(availableVersionFile);
            return new Version(content);
        }

        private string GetAvailableVersionFile()
        {
            var externalStoragePublicDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var apkName = $"{_packageName}.apk";
            var path = $"/{externalStoragePublicDirectory.AbsolutePath}/{apkName}.txt";
            var url = $"{_url}{apkName}.txt";

            return GetFileFromUrl(url, path);
        }

        private string GetApkFile()
        {
            var externalStoragePublicDirectory = Android.OS.Environment.GetExternalStoragePublicDirectory(Android.OS.Environment.DirectoryDownloads);
            var apkName = $"{_packageName}.apk";
            var path = $"/{externalStoragePublicDirectory.AbsolutePath}/{apkName}";
            var url = $"{_url}{apkName}";

            return GetFileFromUrl(url, path);
        }

        private static string GetFileFromUrl(string fromUrl, string toFile)
        {
            try
            {
                URL url = new URL(fromUrl);
                URLConnection connection = url.OpenConnection();
                connection.Connect();

                //int fileLength = connection.GetContentLength();

                // download the file
                InputStream input = new BufferedInputStream(url.OpenStream());
                OutputStream output = new FileOutputStream(toFile);

                var data = new byte[1024];
                long total = 0;
                int count;
                while ((count = input.Read(data)) != -1)
                {
                    total += count;
                    ////PublishProgress((int)(total * 100 / fileLength));
                    output.Write(data, 0, count);
                }

                output.Flush();
                output.Close();
                input.Close();
            }
            catch (Exception e)
            {
                Log.Error("YourApp", e.Message);
            }
            return toFile;
        }

    }
}