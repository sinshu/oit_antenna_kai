using System;
using System.Collections.Generic;
using System.Linq;

namespace OitAntennaKai
{
    class Ordering
    {
        public static IEnumerable<Article> OrderByDate(IEnumerable<Blog> blogs)
        {
            var articles = blogs.SelectMany(blog => blog.Articles);
            return articles.OrderByDescending(article => article.Date);
        }
    }
}
