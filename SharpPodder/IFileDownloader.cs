using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpPodder
{
    public interface IFileDownloader
    {
        //TODO: add progress indicatior events
        void Download(Uri remoteUri, Uri localUri);
    }
}