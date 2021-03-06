﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace OitAntennaKai
{
    internal class Category
    {
        private string id;
        private bool enableBundle;
        private List<RssInfo> rssInfoList;
        private string outputFilePath;

        public Category(string id, bool enableBundle)
        {
            this.id = id;
            this.enableBundle = enableBundle;
            var rssListPath = Path.Combine(Setting.RssListDirectory, id + ".txt");
            rssInfoList = new List<RssInfo>();
            Console.WriteLine("カテゴリ '" + id + "' を初期化します。");
            foreach (var line in File.ReadLines(rssListPath))
            {
                var rssInfo = new RssInfo(this, line);
                Console.WriteLine(line + " -> " + rssInfo.Message);
                rssInfoList.Add(rssInfo);
            }
            outputFilePath = Path.Combine(Setting.OutputDirectory, id + ".html");
        }

        public void CreateHtmlFile()
        {
            var book = new Book(rssInfoList.Where(rss => rss.Blog != null).Select(rss => rss.Blog), enableBundle);
            HtmlMainPage.CreateMainPage(outputFilePath, book);
            //Console.WriteLine("HTMLが更新されたぜ (" + outputFilePath + ")");
        }

        internal void DumpArticles()
        {
            var book = new Book(rssInfoList.Where(rss => rss.Blog != null).Select(rss => rss.Blog), enableBundle);
            var path = Path.Combine(Setting.OutputDirectory, id + ".txt");
            using (var writer = new StreamWriter(path))
            {
                foreach (var bundle in book.Bundles)
                {
                    if (bundle.Articles.Count == 1)
                    {
                        var article = bundle.Articles[0];
                        writer.WriteLine(article.Date.ToString("yyyy/MM/dd HH:mm:ss ") + article.Title + " (" + article.Blog.Title + ")");
                    }
                    else
                    {
                        foreach (var article in bundle.Articles)
                        {
                            writer.WriteLine("● " + article.Date.ToString("yyyy/MM/dd HH:mm:ss ") + article.Title + " (" + article.Blog.Title + ")");
                        }
                    }
                    writer.WriteLine();
                }
            }
        }

        public string ID
        {
            get
            {
                return id;
            }
        }

        public IReadOnlyList<RssInfo> RssInfoList
        {
            get
            {
                return rssInfoList;
            }
        }
    }
}
