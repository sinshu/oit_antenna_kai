using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace OitAntennaKai
{
    public static class Setting
    {
        public static readonly bool UseBlogCache = true;

        public static readonly string PageTitle = "OITあんてな(改)";

        public static readonly string RootDirectory;
        public static readonly string RssListDirectory;
        public static readonly string OutputDirectory;

        static Setting()
        {
            var exePath = Assembly.GetEntryAssembly().Location;
            RootDirectory = Path.GetDirectoryName(exePath);
            RssListDirectory = Path.Combine(RootDirectory, "rsslist");
            OutputDirectory = Path.Combine(RootDirectory, "files");
        }
    }
}
