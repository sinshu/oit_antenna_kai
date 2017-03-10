using System;
using System.Collections.Generic;
using System.Linq;

namespace OitAntennaKai
{
    internal class Stats
    {
        private RssInfo rssInfo;
        private int accessFailureCount;
        private double articlesPerDay;
        private int articleCount;
        private int bundleCount;

        internal Stats(RssInfo rssInfo)
        {
            this.rssInfo = rssInfo;
            ResetAccessFailureCount();
            ResetScore();
        }

        public void ResetAccessFailureCount()
        {
            accessFailureCount = 0;
        }

        public void ResetScore()
        {
            if (rssInfo.Blog != null)
            {
                var count = Math.Min(10, rssInfo.Blog.Articles.Count);
                var index = count - 1;
                articlesPerDay = count / (DateTime.Now - rssInfo.Blog.Articles[index].Date).TotalDays;
            }
            else
            {
                articlesPerDay = 0;
            }
            articleCount = 0;
            bundleCount = 0;
        }

        public void IncreaseAccessFailureCount()
        {
            accessFailureCount++;
        }

        public void IncreaseArticleCount()
        {
            articleCount++;
        }

        public void IncreaseBundleCount()
        {
            bundleCount++;
        }

        public int AccessFailureCount
        {
            get
            {
                return accessFailureCount;
            }
        }

        public double ArticlesPerDay
        {
            get
            {
                return articlesPerDay;
            }
        }

        public double BundleRatio
        {
            get
            {
                if (articleCount > 0)
                {
                    return (double)bundleCount / articleCount;
                }
                else
                {
                    return 0;
                }
            }
        }

        public double Score
        {
            get
            {
                const int n = 3;
                var omake = Math.Max(articlesPerDay - n, 0) / 10000;
                return Math.Min(articlesPerDay, n) / n - BundleRatio + omake;
            }
        }
    }
}
