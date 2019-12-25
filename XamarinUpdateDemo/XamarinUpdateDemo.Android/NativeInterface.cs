using Android.App;
using Android.Content;
using Android.Net;
using Android.OS;
using Android.Support.V4.Content;
using Java.IO;
using System;
using XamarinUpdateDemo.Droid;
using Uri = Android.Net.Uri;

[assembly: Xamarin.Forms.Dependency(typeof(NativeInterface))]
namespace XamarinUpdateDemo.Droid
{
    public class NativeInterface : INativeInterface
    {
        public void InstallAPK(string apk)
        {
            MyFragment fragment = new MyFragment(apk);
            MainActivity.Instance.SupportFragmentManager.BeginTransaction().Add(fragment, "MyFragment").CommitAllowingStateLoss();
        }
    }

    public class MyFragment : Android.Support.V4.App.Fragment
    {
        private string _filePath;
        public MyFragment(string filePath)
        {
            _filePath = filePath;
        }

        public override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            InstallAPK(_filePath);
        }

        public void InstallAPK(String fileUri)
        {
            // 核心是下面几句代码
            if (null != fileUri) {
                try {
                    Intent intent = new Intent(Intent.ActionView);
                    File apkFile = new File(fileUri);
                    //兼容7.0
                    if (Build.VERSION.SdkInt >= BuildVersionCodes.N) {
                        intent.SetFlags(ActivityFlags.GrantReadUriPermission);
                        Uri contentUri = FileProvider.GetUriForFile(Activity, Activity.PackageName + ".fileprovider", apkFile);
                        intent.SetDataAndType(contentUri, "application/vnd.android.package-archive");
                        //兼容8.0
                        if (Build.VERSION.SdkInt >= BuildVersionCodes.O) {
                            var hasInstallPermission = Activity.PackageManager.CanRequestPackageInstalls();
                            if (!hasInstallPermission) {
                                StartInstallPermissionSettingActivity();
                                return;
                            }
                        }
                    }
                    else {
                        intent.SetDataAndType(Uri.FromFile(apkFile), "application/vnd.android.package-archive");
                        intent.SetFlags(ActivityFlags.NewTask);
                    }
                    if (Activity.PackageManager.QueryIntentActivities(intent, 0).Count > 0) {
                        StartActivity(intent);
                    }
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.Fail(ex.Message);
                }
            }
        }

        /**
         * 跳转到设置-允许安装未知来源-页面
         */
        private void StartInstallPermissionSettingActivity()
        {
            Uri packageURI = Uri.Parse("package:" + Activity.PackageName);
            Intent intent = new Intent(Android.Provider.Settings.ActionManageUnknownAppSources, packageURI);
            intent.AddFlags(ActivityFlags.NewTask);
            StartActivityForResult(intent, 1);
        }

        public override void OnActivityResult(int requestCode, int resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            InstallAPK(_filePath);
        }
    }

}