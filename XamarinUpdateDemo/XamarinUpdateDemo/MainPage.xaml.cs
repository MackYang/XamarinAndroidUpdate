using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace XamarinUpdateDemo
{
    // Learn more about making custom code visible in the Xamarin.Forms previewer
    // by visiting https://aka.ms/xamarinforms-previewer
    [DesignTimeVisible(false)]
    public partial class MainPage : ContentPage
    {

        INativeInterface native;
        public MainPage()
        {
            InitializeComponent();
            var version = VersionTracking.CurrentVersion;

            var fileName = $"test.apk";
            var localFile = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), fileName);
            native = DependencyService.Get<INativeInterface>();

            Task.Run(() =>
            {
                DownLoadFile("http://static.loc-mall.com/download/webview.apk", localFile);
            });
        }

        public bool DownLoadFile(string serverFile, string localFile)
        {
            try {

                WebRequest request = WebRequest.Create(serverFile);
                WebResponse response = request.GetResponse();
                long fileLength = response.ContentLength;
                //剩余下载长度
                long leftLength = fileLength;
                int maxReadCount = 10240;
                long totalRead = 0;

                using (Stream stream = response.GetResponseStream()) {
                    using (FileStream fs = new FileStream(localFile, FileMode.Create, FileAccess.Write)) {
                        while (leftLength > 0) {
                            maxReadCount = maxReadCount > leftLength ? (int)leftLength : maxReadCount;
                            var buffer = new byte[maxReadCount];
                            int downByte = stream.Read(buffer, 0, maxReadCount);
                            fs.Write(buffer, 0, downByte);
                            if (downByte <= 0) { break; }
                            leftLength -= downByte;
                            totalRead += downByte;
                            var rate = (double)totalRead / (double)fileLength;
                            Console.WriteLine("Rate=" + rate);
                        }
                    }
                    native.InstallAPK(localFile);
                }
                return true;
            }
            catch (Exception ex) {
                return false;
            }
        }
    }
}
