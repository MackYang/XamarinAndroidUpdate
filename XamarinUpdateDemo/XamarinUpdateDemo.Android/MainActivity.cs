using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using System.Collections.Generic;
using Android;
using Android.Support.V4.Content;
using Android.Support.V4.App;
using Plugin.CurrentActivity;

namespace XamarinUpdateDemo.Droid
{
    [Activity(Label = "XamarinUpdateDemo", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity
    {

        public static MainActivity Instance { get; set; }

        protected override void OnCreate(Bundle savedInstanceState)
        {
            Instance = this;
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            LoadApplication(new App());
            RequestPromissions();
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);
            
            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        protected override void OnDestroy()
        {
            base.OnDestroy();
            Instance = null;
        }
        /// <summary>
        /// 请求权限
        /// </summary>
        private void RequestPromissions()
        {
            var requiredPermissions = new List<string>
            {
                Manifest.Permission.AccessNetworkState,
                Manifest.Permission.WriteExternalStorage,
                Manifest.Permission.ReadExternalStorage,
                Manifest.Permission.MountUnmountFilesystems,
                Manifest.Permission.InstallPackages
            };


            var requestPermissions = new List<string>();


            requiredPermissions.ForEach(x =>
            {
                if (ContextCompat.CheckSelfPermission(this, x) != Permission.Granted)
                {
                    requestPermissions.Add(x);
                }
            });

            if (requestPermissions.Count > 0)
            {
                ActivityCompat.RequestPermissions(this, requestPermissions.ToArray(), requestCode: 1);
            }
        }
    }
}