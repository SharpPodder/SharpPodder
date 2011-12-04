using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using SharpPodder.FileDownloaders;
using SharpPodder.FeedReading;
using Newtonsoft.Json.Converters;
using System.Runtime.Serialization;
using SharpPodder.FeedMerging;

namespace SharpPodder
{
    [JsonObject(MemberSerialization.OptIn)]
    public class Subscription
    {
        private static readonly DateTime sessionTime = DateTime.Now;
        private static int sessionFileCount = 0;

        [JsonProperty]
        public IFeedReader FeedReader { get; set; }

        [JsonProperty]
        public string Name { get; set; }
        [JsonProperty]
        public Uri Uri { get; set; }
        [JsonProperty]
        public string FileNameFormat { get; set; }

        [JsonProperty]
        public List<ISubscriptionFilter> Filters { get; set; }

        [JsonProperty(ObjectCreationHandling=ObjectCreationHandling.Reuse)]
        public SubscriptionState State { get; set; }
        
        public IFileDownloader FileDownloader { get; set; }

        public IFeedMerger FeedMerger { get; set; }

        public void RefreshItems()
        {
            var feed = FeedReader.ReadItems(Uri);
            var merged = FeedMerger.Merge(State.Items, feed.Items);
            foreach (var filter in Filters)
                filter.Filter(merged);
            State.OverrideItems(merged);
            Changed();
        }

        internal Uri GenerateLocalFileName(SubscriptionItem item, Uri uri)
        {
            //TODO: beautify and optimize it
            var nameTemplate = FileNameFormat.Replace('/', Path.DirectorySeparatorChar).Replace('\\', Path.DirectorySeparatorChar);
            nameTemplate = nameTemplate.Replace("{feedName", "{0");
            nameTemplate = nameTemplate.Replace("{timeNow", "{1");
            nameTemplate = nameTemplate.Replace("{timePublish", "{2");
            nameTemplate = nameTemplate.Replace("{timeSession", "{3");
            nameTemplate = nameTemplate.Replace("{fileName", "{4");
            nameTemplate = nameTemplate.Replace("{fileExtension", "{5");
            nameTemplate = nameTemplate.Replace("{fullFileName", "{6");
            nameTemplate = nameTemplate.Replace("{globalFeedNumber", "{7");
            nameTemplate = nameTemplate.Replace("{globalEntryNumber", "{8");
            nameTemplate = nameTemplate.Replace("{globalFileNumber", "{9");
            nameTemplate = nameTemplate.Replace("{feedEntryNumber", "{10");
            nameTemplate = nameTemplate.Replace("{feedFileNumber", "{11");
            nameTemplate = nameTemplate.Replace("{entryFileNumber", "{12");
            nameTemplate = nameTemplate.Replace("{MyDocumentsFolder", "{13");
            nameTemplate = nameTemplate.Replace("{MyMusicFolder", "{14");
            nameTemplate = nameTemplate.Replace("{ApplicationDataFolder", "{15");
            nameTemplate = nameTemplate.Replace("{CommonDocumentsFolder", "{16");
            nameTemplate = nameTemplate.Replace("{CommonMusicFolder", "{17");
            nameTemplate = nameTemplate.Replace("{CommonVideosFolder", "{18");
            nameTemplate = nameTemplate.Replace("{DesktopFolder", "{19");
            nameTemplate = nameTemplate.Replace("{MyVideosFolder", "{20");
            nameTemplate = nameTemplate.Replace("{PersonalFolder", "{21");
            nameTemplate = nameTemplate.Replace("{UserProfileFolder", "{22");
            nameTemplate = nameTemplate.Replace("{TempFolder", "{23");

            var originalFullFileName = uri.Segments.Last();
            var originalFileName = Path.GetFileNameWithoutExtension(originalFullFileName).Replace('/', '_').Replace('\\', '_');
            var originalExtension = Path.GetExtension(originalFullFileName).TrimStart('.');

            var fileName = string.Format(
                nameTemplate,
                Name,                   //0 feedName
                DateTime.Now,           //1 timeNow
                item.PublishDate,      //2 timePublish
                sessionTime,            //3 timeSession
                originalFileName,       //4 fileName
                originalExtension,      //5 fileExtension
                originalFullFileName,   //6 fullFileName
                null, //sessionFeedCount,       //7 globalFeedNumber
                null, //sessionEntryCount,      //8 globalEntryNumber
                ++sessionFileCount,       //9 globalFileNumber
                null, //feedEntriesCount,       //10    feedEntryNumber 
                null, //feedDownloadsCount,     //11    feedFileNumber
                null, //itemFileNumber,     //12    entryFileNumber
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),       //13 MyDocumentsFolder
                Environment.GetFolderPath(Environment.SpecialFolder.MyMusic),           //14 MyMusicFolder
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),   //15 ApplicationDataFolder
                Environment.GetFolderPath(Environment.SpecialFolder.CommonDocuments),   //16 CommonDocumentsFolder
                Environment.GetFolderPath(Environment.SpecialFolder.CommonMusic),       //17 CommonMusicFolder
                Environment.GetFolderPath(Environment.SpecialFolder.CommonVideos),      //18 CommonVideosFolder
                Environment.GetFolderPath(Environment.SpecialFolder.DesktopDirectory),  //19 DesktopFolder
                Environment.GetFolderPath(Environment.SpecialFolder.MyVideos),          //20 MyVideosFolder
                Environment.GetFolderPath(Environment.SpecialFolder.Personal),          //21 PersonalFolder
                Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),       //22 UserProfileFolder
                Path.GetTempPath()                                                     //23 TempFolder
                );

            return new Uri(fileName);
        }

        protected Subscription()
        {
            FileDownloader = new DefaultFileDownloader();
            FeedMerger = new AddNewItemsLastFeedMerger();
            State = new SubscriptionState(this);
            FileNameFormat = "{MyDocumentsFolder}/SharpPodder/{feedName}/{fileName}{timeNow:yyyyMMddhhmmss}.{fileExtension}";
            //MediaTypes = new HashSet<string>() { "audio/mpeg" };
            //RelationshipTypes = new HashSet<string>() { "enclosure", "test1" };
            Filters = new List<ISubscriptionFilter>()
            {
                new StandardPodcastContentFilter(),
                new StandardLimitOnNewSubscriptions()
            };
        }

        public Subscription(string name, Uri feedUrl)
            : this()
        {
            Name = name;
            Uri = feedUrl;
        }

        public IEnumerable<SubscriptionItemLink> Links
        {
            get { return State.Items.SelectMany(x => x.Links); }
        }

        protected virtual internal void Changed()
        {
        }
    }
}