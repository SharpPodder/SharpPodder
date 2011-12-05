using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Net;

namespace SharpPodder.FileDownloaders
{
    public class DefaultFileDownloader : IFileDownloader
    {
        public void Download(Uri remoteUri, Uri localUri)
        {
            var localPath = localUri.LocalPath;
            var tmp = Path.GetTempFileName();
            var wc = new WebClient();
            wc.DownloadFile(remoteUri, tmp);
            var localFolder = Path.GetDirectoryName(localPath);
            if (!Directory.Exists(localFolder))
                Directory.CreateDirectory(localFolder);
            if (File.Exists(localPath))
                File.Delete(localPath);
            File.Move(tmp, localPath);
        }
    }
}