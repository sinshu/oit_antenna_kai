using System;
using System.Collections.Generic;
using System.Linq;

namespace OitAntennaKai
{
    public class Bundle
    {
        private List<Article> articles;

        internal Bundle(Article article)
        {
            articles = new List<Article>();
            articles.Add(article);
        }

        public void Add(Article article)
        {
            articles.Add(article);
        }

        public IReadOnlyList<Article> Articles
        {
            get
            {
                return articles;
            }
        }
    }
}
