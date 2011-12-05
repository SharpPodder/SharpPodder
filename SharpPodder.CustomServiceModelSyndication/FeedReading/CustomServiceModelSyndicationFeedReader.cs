using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.ServiceModel.Syndication;
using System.Diagnostics.CodeAnalysis;

namespace SharpPodder.FeedReading
{
    public class CustomServiceModelSyndicationFeedReader : IFeedReader
    {
        public Feed ReadItems(Uri uri)
        {
            using (XmlReader reader = XmlReader.Create(uri.ToString()))
            {
                var readedItems = SyndicationFeed.Load(reader).Items.ToList();
                var items = readedItems.Select(x => new FeedItem()
                {
                    Id = x.Id,
                    PublishDate = x.PublishDate,
                    LastUpdatedTime = x.LastUpdatedTime == DateTimeOffset.MinValue ? null : (DateTimeOffset?)x.LastUpdatedTime,
                    Summary = x.Summary == null ? null : x.Summary.Text,
                    Title = x.Title == null ? null : x.Title.Text,
                    Categories = new HashSet<string>(x.Categories == null ? Enumerable.Empty<string>() : x.Categories.Select(y => y.Name)),
                    Links = x.Links
                        .Select(link =>
                            new FeedItemLink()
                            {
                                Uri = link.Uri,
                                MediaType = link.MediaType,
                                RelationshipType = link.RelationshipType,
                                Length = link.Length,
                                Title = link.Title
                            }).ToList()
                }).Reverse().ToList();
                var feed = new Feed() { Items = items };
                return feed;
            }
        }
    }
}