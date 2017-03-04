using System;
using System.Collections.Generic;
using System.Linq;

namespace OitAntennaKai
{
    internal class Stats
    {
        private Blog blog;
        private double articlesPerDay;
        private int articleCount;
        private int bundleCount;

        internal Stats(Blog blog)
        {
            this.blog = blog;
            Reset();
        }

        public void Reset()
        {
            var index = Math.Min(10, blog.Articles.Count - 1);
            var count = index + 1;
            articlesPerDay = count / (DateTime.Now - blog.Articles[index].Date).TotalDays;
            articleCount = 0;
            bundleCount = 0;
        }

        public void IncreaseArticleCount()
        {
            articleCount++;
        }

        public void IncreaseBundleCount()
        {
            bundleCount++;
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
                return Math.Min(articlesPerDay, 3) / 3 - BundleRatio;
            }
        }
    }
}
