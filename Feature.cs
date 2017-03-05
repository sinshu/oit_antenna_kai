using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace OitAntennaKai
{
    internal class Feature
    {
        private int length;
        private byte[] histogram;

        internal Feature(Article article)
        {
            var normalized = Normalize(article.Title);
            length = normalized.Length;
            histogram = new byte[256];
            var data = Encoding.UTF8.GetBytes(normalized);
            foreach (var b in data)
            {
                histogram[b]++;
            }
        }

        public double GetDistance(Feature feature)
        {
            if (length <= 6 || feature.length <= 6)
            {
                return 999;
            }
            if (Math.Abs(feature.length - length) >= 10) return 999;
            var count = 0;
            for (var i = 0; i < 256; i++)
            {
                var value1 = histogram[i];
                var value2 = feature.histogram[i];
                if (value1 < value2)
                {
                    count += value2 - value1;
                }
                else
                {
                    count += value1 - value2;
                }
            }
            return (double)count / Math.Max(length, feature.length);
        }

        private string Normalize(string source)
        {
            var sb = new StringBuilder();
            foreach (var ch in source)
            {
                if (ch != 'w' && ch != 'W' && ch != 'ｗ' && ch != 'Ｗ')
                {
                    sb.Append(ch);
                }
            }
            return sb.ToString();
        }
    }
}
