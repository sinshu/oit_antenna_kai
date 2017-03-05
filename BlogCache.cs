using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;

namespace OitAntennaKai
{
    internal class BlogCache
    {
        private static readonly char[] uriSeparator = new[] { '/' };

        private static readonly string cacheDirectory;

        static BlogCache()
        {
            var assembly = Assembly.GetEntryAssembly();
            var directory = Path.GetDirectoryName(assembly.Location);
            cacheDirectory = Path.Combine(directory, "cache");
        }

        public static Blog GetBlogFromRssUri(string rssUri)
        {
            var name = GetShortNameFromUri(rssUri);
            var cache = Path.Combine(cacheDirectory, name + ".xml");
            if (!File.Exists(cache))
            {
                CacheBlog(rssUri);
            }
            using (var reader = new StreamReader(cache))
            {
                return new Blog(reader);
            }
        }

        private static string GetShortNameFromUri(string uri)
        {
            var splitted = uri.Split(uriSeparator, StringSplitOptions.RemoveEmptyEntries);
            return splitted[splitted.Length - 2];
        }

        private static void CacheBlog(string rssUri)
        {
            var name = GetShortNameFromUri(rssUri);
            var cache = Path.Combine(cacheDirectory, name + ".xml");
            if (!Directory.Exists(cacheDirectory))
            {
                Directory.CreateDirectory(cacheDirectory);
            }
            var request = WebRequest.Create(rssUri);
            using (var response = request.GetResponse())
            using (var stream = response.GetResponseStream())
            using (var reader = new StreamReader(stream))
            using (var writer = new StreamWriter(cache))
            {
                for (var line = reader.ReadLine(); line != null; line = reader.ReadLine())
                {
                    writer.WriteLine(line);
                }
            }
        }
    }
}
