using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace OitAntennaKai
{
    public class Article
    {
        private static readonly Regex regNumber = new Regex(@"[0-9０-９]+\s?話");

        private Blog blog;
        private string uri;
        private DateTime date;
        private string title;

        private Feature feature;

        internal Article(Blog blog, string uri, DateTime date, string title)
        {
            this.blog = blog;
            this.uri = uri;
            this.date = date;
            this.title = CheckTitle(title);

            this.feature = new Feature(this);
        }

        private static string CheckTitle(string title)
        {
            if (title.Length > 0)
            {
                foreach (var netabare in Setting.NetabareWarningList)
                {
                    if (title.Contains(netabare))
                    {
                        var number = regNumber.Match(title);
                        if (number.Success)
                        {
                            return "[ネタバレ] " + netabare + " " + number.Value;
                        }
                    }
                }

                return title;
            }
            else
            {
                return "無題";
            }
        }

        public Blog Blog
        {
            get
            {
                return blog;
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public DateTime Date
        {
            get
            {
                return date;
            }
        }

        public string Title
        {
            get
            {
                return title;
            }
        }

        internal Feature Feature
        {
            get
            {
                return feature;
            }
        }
    }
}
