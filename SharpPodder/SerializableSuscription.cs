using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.IO;
using SharpPodder.FeedReading;
using System.Runtime.Serialization;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace SharpPodder
{
    public class SerializableSuscription : Subscription, IDisposable
    {
		private static readonly string[] AllowedExtensions = new[] { ".json" };
        private static readonly string SharpPodderUserFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), ".SharpPodder");
        private static readonly string SubcriptionsUserFolder = Path.Combine(SharpPodderUserFolder, "Subscriptions");
        private static readonly JsonSerializerSettings SerializerSettings = new JsonSerializerSettings()
        {
            Converters = new JsonConverter[] { new MyIsoDateTimeConverter(), new StringEnumConverter() },
            NullValueHandling = NullValueHandling.Ignore,
            TypeNameHandling = TypeNameHandling.Auto,
            ObjectCreationHandling = ObjectCreationHandling.Replace,
            DefaultValueHandling = DefaultValueHandling.Include,
			Error = (object sender, Newtonsoft.Json.Serialization.ErrorEventArgs e) =>
			{
				//In order to ignore all conversion errors
				Console.WriteLine("*** [");
				Console.WriteLine(e.ErrorContext.OriginalObject);
				Console.WriteLine(e.ErrorContext.Member);
				Console.WriteLine(e.ErrorContext.Error);
				Console.WriteLine("] ***");
				e.ErrorContext.Handled = true;
			}
        };

        public bool AutoSave { get; set; }
        public string FileName { get; private set; }
		
		[JsonProperty]
		public DateTime BlockTo { get; private set; }

        private SerializableSuscription(string fileName)
        {
            AutoSave = true;
            FileName = fileName;
        }
		
		private void Block(TimeSpan blockFor)
		{
			BlockTo = DateTime.Now + blockFor;
			Save();
		}
		
		private void Block(TimeSpan? blockFor)
		{
			if (blockFor.HasValue)
				Block(blockFor.Value);
			else
				UnBlock();
		}
		
		private void UnBlock()
		{
			if (BlockTo != DateTime.MinValue)
			{
				BlockTo = DateTime.MinValue;
				Save();
			}
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

        public static SerializableSuscription New(string fileName, string name, Uri url, TimeSpan? blockFor)
        {
            var subscription = new SerializableSuscription(fileName) { Name = name, Uri = url };
            subscription.Save();
			subscription.Block(blockFor);
            return subscription;
        }

        public static SerializableSuscription Open(string fileName, TimeSpan? blockFor)
        {
			if (File.Exists(fileName))
			{
	            var json = File.ReadAllText(fileName);
	            if (json.Length > 0)
				{
					var subscription = new SerializableSuscription(fileName);
	                JsonConvert.PopulateObject(json, subscription, SerializerSettings);
					if (subscription.BlockTo < DateTime.Now)
					{
						subscription.Block(blockFor);
						return subscription;
					}
				}
			}
			//Blocked or not exists
            return null;
        }
        
        public static IEnumerable<SerializableSuscription> OpenFromUserFolder(TimeSpan? blockFor)
        {
            return OpenFromFolder(blockFor, SubcriptionsUserFolder);
        }

        public static IEnumerable<SerializableSuscription> OpenFromFolder(TimeSpan? blockFor, params string[] paths)
        {
            return paths.Where(path => Directory.Exists(path)).SelectMany(path => Directory.EnumerateFiles(path)).Where(x => AllowedExtensions.Any(y => x.EndsWith(y))).Select(x => Open(x, blockFor)).Where(x => x != null);
        }

        protected internal override void Changed()
        {
            base.Changed();
            if (AutoSave)
                Save();
        }
		
		public void Dispose ()
		{
			UnBlock();
		}
	}
}