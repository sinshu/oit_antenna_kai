using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

namespace OitAntennaKai
{
    public static class Setting
    {
        public static readonly bool UseBlogCache = false;

        public static readonly string PageTitle = UseBlogCache ? "デバッグ中" : "OITあんてな(改)";

        public static readonly string RootDirectory;
        public static readonly string RssListDirectory;
        public static readonly string BlogCacheDirectory;
        public static readonly string OutputDirectory;

        public static readonly IReadOnlyList<string> NetabareWarningList;

        public static readonly TimeSpan AccessInterval = TimeSpan.FromMinutes(15);

        static Setting()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            RootDirectory = Path.GetDirectoryName(exePath);
            RssListDirectory = Path.Combine(RootDirectory, "rsslist");
            BlogCacheDirectory = Path.Combine(RootDirectory, "cache");
            OutputDirectory = Path.Combine(RootDirectory, "files");

            var netabareListPath = Path.Combine(RootDirectory, "netabare.txt");
            NetabareWarningList = File.ReadAllLines(netabareListPath, Encoding.UTF8);
        }
    }
}
