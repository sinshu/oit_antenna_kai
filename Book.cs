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
                bundles = blogs.SelectMany(blog => blog.Articles.Select(article => new Bundle(article))).ToList();
                bundles.Sort((x, y) => DateTime.Compare(y.Articles[0].Date, x.Articles[0].Date));
            }
            UpdateStats(this.blogs, bundles);
        }

        public IEnumerable<Blog> OrderByUnko()
        {
            return blogs.OrderByDescending(blog => blog.Stats.Score);
        }

        private static IEnumerable<Article> OrderByDate(IEnumerable<Blog> blogs)
        {
            var articles = blogs.SelectMany(blog => blog.Articles);
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
            bundles.Sort((x, y) => DateTime.Compare(y.Articles[0].Date, x.Articles[0].Date));
            return bundles;
        }

        private static void UpdateStats(IEnumerable<Blog> blogs, List<Bundle> bundles)
        {
            foreach (var blog in blogs)
            {
                blog.Stats.Reset();
            }
            foreach (var bundle in bundles)
            {
                if (bundle.Articles.Count >= 2)
                {
                    foreach (var article in bundle.Articles)
                    {
                        article.Blog.Stats.IncreaseArticleCount();
                        article.Blog.Stats.IncreaseBundleCount();
                    }
                }
                else
                {
                    bundle.Articles[0].Blog.Stats.IncreaseArticleCount();
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
