using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OitAntennaKai
{
    internal class Category
    {
        private string id;
        private List<RssInfo> rssInfoList;
        private string outputFilePath;

        public Category(string id)
        {
            this.id = id;
            var rssListPath = Path.Combine(Setting.RssListDirectory, id + ".txt");
            rssInfoList = new List<RssInfo>();
            foreach (var line in File.ReadLines(rssListPath))
            {
                var rssInfo = new RssInfo(line);
                Console.WriteLine(line + " -> " + rssInfo.Message);
                rssInfoList.Add(rssInfo);
            }
            outputFilePath = Path.Combine(Setting.OutputDirectory, id + ".html");
        }

        public void CreateHtmlFile(bool enableBundle)
        {
            var book = new Book(rssInfoList.Where(rss => rss.Blog != null).Select(rss => rss.Blog), enableBundle);
            HtmlPage.CreateMainPage(outputFilePath, book);
        }
    }
}
