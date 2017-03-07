using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OitAntennaKai
{
    internal static class HtmlLogPage
    {
        private static readonly string daysOfWeek = "日月火水木金土";

        private static readonly string outputPath = Path.Combine(Setting.OutputDirectory, "stats.html");

        public static void CreateLogPage(IEnumerable<Category> categories)
        {
            using (var writer = new StreamWriter(outputPath, false, Encoding.UTF8))
            {
                BeginHtml(writer);
                WriteHeader(writer, "RSS取得状況");
                BeginBody(writer);
                WriteTable(writer, categories);
                EndBody(writer);
                EndHtml(writer);
            }
            //Console.WriteLine("ログが更新されたぜ (" + outputPath + ")");
        }

        private static void BeginHtml(StreamWriter writer)
        {
            writer.WriteLine("<!DOCTYPE html>");
            writer.WriteLine("<html lang=\"ja\">");
        }

        private static void EndHtml(StreamWriter writer)
        {
            writer.WriteLine("</html>");
        }

        private static void WriteHeader(StreamWriter writer, string title)
        {
            writer.WriteLine("<head>");
            writer.WriteLine("<meta charset=\"UTF-8\">");
            writer.WriteLine("<title>" + title + "</title>");
            writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"stats.css\">");
            writer.WriteLine("</head>");
        }

        private static void BeginBody(StreamWriter writer)
        {
            writer.WriteLine("<body>");
            writer.WriteLine("<h1>RSS取得状況</h1>");
            var now = DateTime.Now;
            writer.WriteLine("<div>最終更新日：" + now.Year + "年" + now.Month + "月" + now.Day + "日(" + daysOfWeek[(int)now.DayOfWeek] + ") " + now.ToString("HH:mm") + "</div>");
        }

        private static void EndBody(StreamWriter writer)
        {
            writer.WriteLine("</body>");
        }

        private static void WriteTable(StreamWriter writer, IEnumerable<Category> categories)
        {
            writer.WriteLine("<table>");
            writer.WriteLine("<tr class=\"header\"><td>カテゴリ</td><td>RSS</td><td>状態</td><td>ブログ名</td><td>最終更新日</td><td>エラー<br>回数</td><td>記事数<br>(件/日)</td><td>重複率</td><td>スコア</td></tr>");
            foreach (var category in categories)
            {
                foreach (var rss in category.RssInfoList)
                {
                    WriteRow(writer, category, rss);
                }
            }
            writer.WriteLine("</table>");
        }

        private static void WriteRow(StreamWriter writer, Category category, RssInfo rss)
        {
            writer.Write("<tr>");
            writer.Write("<td>" + category.ID + "</td>");
            writer.Write("<td>" + rss.Uri + "</td>");
            writer.Write("<td>" + rss.Message + "</td>");
            if (rss.Blog != null)
            {
                writer.Write("<td>" + rss.Blog.Title + "</td>");
                writer.Write("<td>" + GetDayText(rss.Blog.Articles[0].Date) + "</td>");
                writer.Write("<td class=\"number\">" + rss.Blog.Stats.AccessFailureCount + "</td>");
                writer.Write("<td class=\"number\">" + rss.Blog.Stats.ArticlesPerDay.ToString("0.0") + "</td>");
                writer.Write("<td class=\"number\">" + (100 * rss.Blog.Stats.BundleRatio).ToString("0.0") + "</td>");
                writer.Write("<td class=\"number\">" + (100 * rss.Blog.Stats.Score).ToString("0.00") + "</td>");
            }
            else
            {
                writer.Write("<td colspan=\"6\">ブログにアクセスできません。</td>");
            }
            writer.WriteLine("</tr>");
        }

        private static string GetDayText(DateTime date)
        {
            return date.ToString("yyyy/MM/dd HH:mm:ss");
        }
    }
}
