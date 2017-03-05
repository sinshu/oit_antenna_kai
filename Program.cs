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
            {
                var category = new Category("general");
                category.CreateHtmlFile(true);
            }
            {
                var category = new Category("news");
                category.CreateHtmlFile(true);
            }
            {
                var category = new Category("anime");
                category.CreateHtmlFile(true);
            }
        }
    }
}
