using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using SharpPodder.FeedReading;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;

namespace SharpPodder
{
    public class SerializableSuscription : Subscription
    {
        private static readonly string SharpPodderUserFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".SharpPodder");
        private static readonly string SubcriptionsUserFolder = Path.Combine(SharpPodderUserFolder, "Subscriptions");
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            Converters = new JsonConverter[] { new IsoDateTimeConverter(), new StringEnumConverter() },
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            DefaultValueHandling = DefaultValueHandling.Include,
			Error = (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e) =>
			{
				//In order to ignore all errors
				e.ErrorContext.Handled = true;
			}
        };

        public bool AutoSave { get; set; }
        public string FileName { get; private set; }

        private SerializableSuscription(string fileName)
        {
            AutoSave = true;
            FileName = fileName;
        }

        public void Save()
        {
            //TODO: block here
            var folder = Path.GetDirectoryName(RealFileName);
            if (!Directory.Exists(folder))
               Directory.CreateDirectory(folder);
            JsonSerializer jsonSerializer = JsonSerializer.Create(SerializerSettings);
            using (var sw = new StreamWriter(RealFileName))
            using (var jsonWriter = new JsonTextWriter(sw))
            {
                jsonWriter.Formatting = Formatting.Indented;
                jsonWriter.QuoteName = false;
                jsonSerializer.Serialize(jsonWriter, this);
            }
        }

        public string RealFileName
        {
            get
            {
                return Path.GetDirectoryName(FileName) == String.Empty
                    ? Path.Combine(SubcriptionsUserFolder, FileName)
                    : FileName;
            }
        }

        public static SerializableSuscription New(string fileName, string name, Uri url)
        {
            var subscription = new SerializableSuscription(fileName) { Name = name, Uri = url };
            subscription.Save();
            return subscription;
        }

        public static SerializableSuscription Open(string fileName)
        {
            var subscription = new SerializableSuscription(fileName);
            var json = File.ReadAllText(subscription.RealFileName);
            if (json.Length > 0)
                JsonConvert.PopulateObject(json, subscription, SerializerSettings);
            return subscription;
        }
        
        public static IEnumerable<SerializableSuscription> OpenFromUserFolder()
        {
            return OpenFromFolder(SubcriptionsUserFolder);
        }

        public static IEnumerable<SerializableSuscription> OpenFromFolder(params string[] paths)
        {
            return paths.Where(path => Directory.Exists(path)).SelectMany(path => Directory.EnumerateFiles(path)).Select(x => Open(x));
        }

        protected internal override void Changed()
        {
            base.Changed();
            if (AutoSave)
                Save();
        }
    }
}