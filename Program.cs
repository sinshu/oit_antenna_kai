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
            var rssList = File.ReadLines("rsslist\\anime.txt").ToArray();
            var blogs = rssList.Select(uri => LocalTest.GetBlogFromRssUri(uri)).ToArray();
            var category = new Category("test", blogs, true);
            HtmlPage.CreateMainPage(@"C:\Users\Sinshu\Desktop\test.html", category);
        }
    }
}
