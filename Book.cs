using System;
using System.Collections.Generic;
using System.Linq;

namespace OitAntennaKai
{
    public class Book
    {
        private static readonly int maxBundleCount = 1000;
        private static readonly double threshold = 0.85;

        private List<Blog> blogs;
        private List<Bundle> bundles;

        public Book(IEnumerable<Blog> blogs, bool enableBundle)
        {
            this.blogs = blogs.ToList();
            if (enableBundle)
            {
                bundles = GetBundles(this.blogs);
            }
            else
            {
                bundles = OrderByDate(blogs).Select(article => new Bundle(article)).Take(maxBundleCount).ToList();
            }
            UpdateStats(this.blogs, bundles);
        }

        public IEnumerable<Blog> OrderByScore()
        {
            return blogs.OrderByDescending(blog => blog.RssInfo.Stats.Score);
        }

        private static IEnumerable<Article> OrderByDate(IEnumerable<Blog> blogs)
        {
            // 遠い未来の時刻を設定した記事を作って、その記事をサイトのトップに表示し続けるという手法がある。
            // そのような記事を時系列に表示する意味はないので、明日よりも未来の時刻を設定された記事は無視する。
            var tomorrow = DateTime.Now + TimeSpan.FromDays(1);
            var articles = blogs.SelectMany(blog => blog.Articles.Where(article => article.Date < tomorrow));
            return articles.OrderByDescending(article => article.Date);
        }

        private static bool CanBundle(Bundle bundle, Article article)
        {
            var min = double.MaxValue;
            foreach (var target in bundle.Articles)
            {
                if (target.Blog == article.Blog)
                {
                    return false;
                }
                var distance = article.Feature.GetDistance(target.Feature);
                if (distance < min)
                {
                    min = distance;
                }
            }
            return min < threshold;
        }

        private static List<Bundle> GetBundles(IEnumerable<Blog> blogs)
        {
            var bundles = new List<Bundle>();
            foreach (var article in OrderByDate(blogs))
            {
                Bundle found = null;
                foreach (var bundle in bundles)
                {
                    if (CanBundle(bundle, article))
                    {
                        found = bundle;
                        break;
                    }
                }
                if (found == null)
                {
                    bundles.Add(new Bundle(article));
                }
                else
                {
                    found.Add(article);
                }
                if (bundles.Count >= maxBundleCount)
                {
                    break;
                }
            }
            return bundles;
        }

        private static void UpdateStats(IEnumerable<Blog> blogs, List<Bundle> bundles)
        {
            foreach (var blog in blogs)
            {
                blog.RssInfo.Stats.ResetScore();
            }
            foreach (var bundle in bundles)
            {
                if (bundle.Articles.Count >= 2)
                {
                    foreach (var article in bundle.Articles)
                    {
                        article.Blog.RssInfo.Stats.IncreaseArticleCount();
                        article.Blog.RssInfo.Stats.IncreaseBundleCount();
                    }
                }
                else
                {
                    bundle.Articles[0].Blog.RssInfo.Stats.IncreaseArticleCount();
                }
            }
        }

        public IReadOnlyList<Blog> Blogs
        {
            get
            {
                return blogs;
            }
        }

        public IReadOnlyList<Bundle> Bundles
        {
            get
            {
                return bundles;
            }
        }
    }
}
