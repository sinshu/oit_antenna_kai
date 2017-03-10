using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace OitAntennaKai
{
    public class Blog
    {
        private string uri;
        private string title;
        private List<Article> articles;

        public Blog(Stream stream)
        {
            var document = new XmlDocument();
            document.Load(stream);
            Initialize(document);
        }

        public Blog(TextReader reader)
        {
            var document = new XmlDocument();
            document.Load(reader);
            Initialize(document);
        }

        public Blog(string rssUri)
        {
            var document = new XmlDocument();
            document.Load(rssUri);
            Initialize(document);
        }

        private void Initialize(XmlDocument document)
        {
            uri = GetUri(document);
            title = GetTitle(document);
            articles = EnumerateArticles(this, document).OrderByDescending(article => article.Date).ToList();
            if (articles.Count == 0)
            {
                throw new Exception("記事の取得に失敗しました。");
            }
        }

        private static string GetUri(XmlDocument document)
        {
            return document.GetElementsByTagName("link")[0].InnerText;
        }

        private static string GetTitle(XmlDocument document)
        {
            return document.GetElementsByTagName("title")[0].InnerText;
        }

        private static string GetDateNodeTagName(XmlDocument document)
        {
            if (document.DocumentElement.Name == "rss")
            {
                return "pubDate";
            }
            else
            {
                return "dc:date";
            }
        }

        private static IEnumerable<Article> EnumerateArticles(Blog blog, XmlDocument document)
        {
            var dateNodeTagName = GetDateNodeTagName(document);
            var items = document.GetElementsByTagName("item");
            foreach (XmlElement item in items)
            {
                var uri = item.GetElementsByTagName("link")[0].InnerText;
                var date = DateTime.Parse(item.GetElementsByTagName(dateNodeTagName)[0].InnerText);
                var title = item.GetElementsByTagName("title")[0].InnerText;
                yield return new Article(blog, uri, date, title);
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        public IReadOnlyList<Article> Articles
        {
            get
            {
                return articles;
            }
        }

        // キモいが許せ…
        internal RssInfo RssInfo
        {
            get;
            set;
        }
    }
}
