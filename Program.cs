using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;

namespace OitAntennaKai
{
    internal static class Program
    {
        private static void Main(string[] args)
        {
            var rssList = File.ReadLines("rsslist.txt").ToArray();
            var blogs = rssList.Select(uri => LocalTest.GetBlogFromRssUri(uri)).ToArray();
            var category = new Category("test", blogs, true);
            Console.WriteLine(blogs.SelectMany(blog => blog.Articles).Count());
            Console.WriteLine(category.Bundles.SelectMany(bundle => bundle.Articles).Count());
            using (var writer = new StreamWriter(@"C:\Users\Sinshu\Desktop\test.txt"))
            {
                foreach (var bundle in category.Bundles)
                {
                    if (bundle.Articles.Count == 1)
                    {
                        writer.WriteLine(ToString(bundle.Articles[0]));
                    }
                    else
                    {
                        foreach (var article in bundle.Articles)
                        {
                            writer.WriteLine("■■" + ToString(article));
                        }
                    }
                    writer.WriteLine();
                }
            }
            foreach (var blog in category.OrderByUnko())
            {
                Console.WriteLine(blog.Stats.Score.ToString("0.00") + ", " + blog.Title + "(" + blog.Stats.BundleRatio.ToString("0.00") + "), " + blog.Stats.ArticlesPerDay.ToString("0.00"));
            }
            Console.WriteLine("AA");
        }

        static string ToString(Article article)
        {
            return article.Date + ": " + article.Title + " (" + article.Blog.Title + ")";
        }
    }
}
