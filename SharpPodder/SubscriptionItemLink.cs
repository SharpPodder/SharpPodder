using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.IO;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class SubscriptionItemLink
    {
        public SubscriptionItem SubscriptionItem { get; internal set; }
        public Subscription Subscription
        {
            get { return SubscriptionItem == null ? null : SubscriptionItem.Subscription; }
        }
        public IFileDownloader FileDownloader
        {
            get { return Subscription == null ? null : Subscription.FileDownloader; }
        }

        [JsonProperty]
        public Uri Uri { get; set; }

        [JsonProperty]
        public string MediaType { get; set; }

        [JsonProperty]
        public string RelationshipType { get; set; }

        [JsonProperty]
        public long Length { get; set; }

        [JsonProperty]
        public string Title { get; set; }

        [JsonProperty]
        public Uri DownloadedFileUri { get; private set; }

        [JsonProperty]
        public Exception Exception { get; private set; }

        [JsonProperty]
        public DateTimeOffset? DownloadDate { get; private set; }

        public bool IsDownloaded 
        { 
            get { return DownloadDate.HasValue; } 
        }

        public bool HasFailed
        {
            get { return Exception != null; }
        }

        private void Changed()
        {
            if (Subscription != null)
                Subscription.Changed();
        }

        public void Download()
        {
            try
            {
                Uri downloadedFileUri = GenerateLocalUri();
                FileDownloader.Download(Uri, downloadedFileUri);
                DownloadedFileUri = downloadedFileUri;
            }
            catch (Exception e)
            {
                Exception = e;
            }
            DownloadDate = DateTimeOffset.Now;
            Changed();
        }

        private Uri GenerateLocalUri()
        {
            return Subscription.GenerateLocalFileName(SubscriptionItem, Uri);
        }

        public void MarkAsNotDownloaded()
        {
            DownloadedFileUri = null;
            DownloadDate = null;
            Exception = null;
            Changed();
        }

        [JsonProperty]
        public bool Deleted { get; set; }

        [JsonProperty]
        public bool Downloadable { get; set; }

        [JsonProperty]
        public bool IgnoreIt { get; set; }

        public bool ExistsDownloadedFile()
        {
            return IsDownloaded && File.Exists(DownloadedFileUri.LocalPath);
        }

        public SubscriptionItemLink(SubscriptionItem subscriptionItem)
        {
            SubscriptionItem = subscriptionItem;
        }
    }
}