using System;
using System.Collections.Generic;
using System.Linq;

namespace OitAntennaKai
{
    internal class RssInfo
    {
        private Category category;
        private string uri;
        private Blog blog;
        private string message;

        public RssInfo(Category category, string uri)
        {
            this.category = category;
            this.uri = uri;
            Update();
        }

        public IList<Article> Update()
        {
            try
            {
                var newBlog = CreateBlog(uri);
                message = "問題ありません。";
                if (blog == null)
                {
                    blog = newBlog;
                    return newBlog.Articles.ToList();
                }
                else
                {
                    var newArticles = EnumerateNewArticles(blog, newBlog).ToList();
                    blog = newBlog;
                    return newArticles;
                }
            }
            catch (Exception e)
            {
                if (blog != null)
                {
                    blog.Stats.IncreaseAccessFailureCount();
                }
                message = e.Message;
                return new List<Article>();
            }
        }

        private Blog CreateBlog(string uri)
        {
            if (Setting.UseBlogCache)
            {
                return BlogCache.GetBlogFromRssUri(uri);
            }
            else
            {
                return new Blog(uri);
            }
        }

        private static IEnumerable<Article> EnumerateNewArticles(Blog oldBlog, Blog newBlog)
        {
            foreach (var article in newBlog.Articles)
            {
                if (article.Date > oldBlog.Articles[0].Date)
                {
                    yield return article;
                }
                else
                {
                    yield break;
                }
            }
        }

        public Category Category
        {
            get
            {
                return category;
            }
        }

        public string Uri
        {
            get
            {
                return uri;
            }
        }

        public Blog Blog
        {
            get
            {
                return blog;
            }
        }

        public string Message
        {
            get
            {
                return message;
            }
        }
    }
}
