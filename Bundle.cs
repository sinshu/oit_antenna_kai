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
            articles.Sort((a1, a2) => DateTime.Compare(a2.Date, a1.Date));
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
