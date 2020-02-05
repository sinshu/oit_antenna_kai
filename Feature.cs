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

        public static string Normalize(string source)
        {
            source = StringEx.HankakuToZenkaku(source);

            var count = new Dictionary<char, int>();
            foreach (var ch in source)
            {
                if (count.ContainsKey(ch))
                {
                    count[ch]++;
                }
                else
                {
                    count.Add(ch, 1);
                }
            }

            var sb = new StringBuilder();
            var buffer = new List<char>();
            foreach (var ch in source)
            {
                if (count[ch] <= 3)
                {
                    if (buffer.Count > 0)
                    {
                        sb.Append(buffer.ToArray());
                        buffer.Clear();
                    }
                    sb.Append(ch);
                }
                else
                {
                    if (buffer.Count < 5)
                    {
                        buffer.Add(ch);
                    }
                }
            }
            if (buffer.Count > 0)
            {
                sb.Append(buffer.ToArray());
            }
            return sb.ToString();
        }
    }
}
