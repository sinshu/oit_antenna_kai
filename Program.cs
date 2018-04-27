using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;

namespace OitAntennaKai
{
    internal static class Program
    {
        private static readonly Random random = new Random();

        private static void Main(string[] args)
        {
            var categories = CreateCategories().ToArray();
            categories.ForEach(category => category.CreateHtmlFile());
            HtmlLogPage.CreateLogPage(categories);
            while (true)
            {
                var rssList = GetAllRssInfo(categories).ToList();
                var interval = TimeSpan.FromSeconds(Setting.AccessInterval.TotalSeconds / rssList.Count);
                Console.WriteLine("ブログ数: " + rssList.Count);
                Console.WriteLine("更新間隔: " + interval.TotalSeconds.ToString("0.0") + "秒");
                var lastUpdateTime = DateTime.Now;
                foreach (var rss in rssList)
                {
                    Thread.Sleep(interval);

                    var now = DateTime.Now;
                    if ((now - lastUpdateTime).TotalHours >= 1)
                    {
                        Console.WriteLine("寝てたっぽい...");
                        break;
                    }
                    lastUpdateTime = now;

                    var newArticles = rss.Update();
                    if (newArticles.Count > 0)
                    {
                        foreach (var article in newArticles)
                        {
                            Console.WriteLine(article.Title + " (" + article.Blog.Title + ")");
                        }
                        rss.Category.CreateHtmlFile();
                        HtmlLogPage.CreateLogPage(categories);
                    }
                }
            }
        }

        private static IEnumerable<Category> CreateCategories()
        {
            yield return new Category("general", true);
            yield return new Category("news", true);
            yield return new Category("anime", true);
            yield return new Category("others", false);
        }

        private static IEnumerable<RssInfo> GetAllRssInfo(IEnumerable<Category> categories)
        {
            // このランダムソート、問題ないん？
            return categories.SelectMany(category => category.RssInfoList).OrderBy(rss => random.NextDouble());
        }
    }
}
