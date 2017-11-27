using System;
using Android.App;
using Android.Widget;
using Android.OS;
using System.Threading.Tasks;
using Android.Support.V7.App;

namespace AutoUpdateApp
{
    [Activity(Label = "AutoUpdateApp", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        private Button _showCurrentVersionButton;
        private Button _forceDownloadButton;

        public Button ShowCurrentVersionButton => _showCurrentVersionButton ?? (_showCurrentVersionButton = FindViewById<Button>(Resource.Id.showCurrentVersionButton));
        public Button ForceDownloadButton => _forceDownloadButton ?? (_forceDownloadButton = FindViewById<Button>(Resource.Id.forceDownloadButton));

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            SetContentView(Resource.Layout.Main);

            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);
            SupportActionBar.SetIcon(Resource.Mipmap.ic_launcher);
            SupportActionBar.SetDisplayUseLogoEnabled(true);

            ShowCurrentVersionButton.Click += ShowCurrentVersionButtonOnClick;

            ForceDownloadButton.Click += async (sender, args) =>
            {
                await Task.Run(() =>
                {
                    var currentVersionName = PackageManager.GetPackageInfo(PackageName, 0).VersionName;
                    var autoUpdater = new AutoUpdater(this, url: "http://192.168.1.190:8080", packageName: PackageName, currentVersionName: currentVersionName);
                    autoUpdater.Update();
                });
            };
        }

        private void ShowCurrentVersionButtonOnClick(object o, EventArgs eventArgs)
        {
            var info = PackageManager.GetPackageInfo(PackageName, 0);
            var currentVersion = $"Current version {info.VersionName} ({info.VersionCode})";
            Toast.MakeText(this, currentVersion, ToastLength.Long).Show();
        }
    }
}

