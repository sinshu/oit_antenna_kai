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
            var categories = new[]
            {
                new Category("general"),
                new Category("news"),
                new Category("anime"),
                new Category("other")
            };
            categories.ForEach(category => category.CreateHtmlFile(category.ID != "other"));
            HtmlLogPage.CreateLogPage(Path.Combine(Setting.OutputDirectory, "stats.html"), categories);
        }
    }
}
