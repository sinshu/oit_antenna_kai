﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace OitAntennaKai
{
    internal static class HtmlMainPage
    {
        private static readonly string daysOfWeek = "日月火水木金土";

        public static void CreateMainPage(string path, Book book)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                BeginHtml(writer);
                WriteHeader(writer, Setting.PageTitle, Path.GetFileNameWithoutExtension(path) + ".css");
                BeginBody(writer);
                WriteTitle(writer);
                WriteMenu(writer);
                WriteMainWindow(writer, book);
                WriteSubWindows(writer, book);
                EndBody(writer);
                EndHtml(writer);
            }
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

        private static void WriteHeader(StreamWriter writer, string title, string cssPath)
        {
            writer.WriteLine("<head>");
            writer.WriteLine("<meta charset=\"UTF-8\">");
            writer.WriteLine("<title>" + title + "</title>");
            writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"" + cssPath + "\">");
            writer.WriteLine("</head>");
        }

        private static void BeginBody(StreamWriter writer)
        {
            writer.WriteLine("<body>");
        }

        private static void EndBody(StreamWriter writer)
        {
            writer.WriteLine("<div class=\"rack\"><a href=\"stats.html\" target=\"_blank\">RSS取得状況</a></div>");
            writer.WriteLine("</body>");
        }

        private static string Escape(string text)
        {
            return text.Replace("&", "&amp;").Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static string CreateLink(string text, string title, string href)
        {
            return "<a href=\"" + Escape(href) + "\" title=\"" + Escape(title) + "\" target=\"_blank\">" + Escape(text) + "</a>";
        }

        private static string CreateLink(string text, string title, string href, string cssClass)
        {
            return "<a class=\"" + cssClass + "\" title=\"" + Escape(title) + "\"  href=\"" + Escape(href) + "\" target=\"_blank\">" + Escape(text) + "</a>";
        }

        private static string CreateLink_NoEscapeForTitle(string text, string title, string href, string cssClass)
        {
            return "<a class=\"" + cssClass + "\" title=\"" + title + "\"  href=\"" + Escape(href) + "\" target=\"_blank\">" + Escape(text) + "</a>";
        }

        private static void WriteTitle(StreamWriter writer)
        {
            writer.WriteLine("<div class=\"rack\">");
            writer.WriteLine("<div>");
            writer.WriteLine("<h1>" + Setting.PageTitle + "</h1>");
            var now = DateTime.Now;
            writer.WriteLine("<div>最終更新日：" + now.Year + "年" + now.Month + "月" + now.Day + "日(" + daysOfWeek[(int)now.DayOfWeek] + ") " + now.ToString("HH:mm") + "</div>");
            writer.WriteLine("</div>");
            writer.WriteLine("</div>");
        }

        private static void WriteMenu(StreamWriter writer)
        {
            writer.WriteLine("<div class=\"rack\">");
            writer.WriteLine("<a href=\"general.html\"><div class=\"menu_general\">一般</div></a>");
            writer.WriteLine("<a href=\"news.html\"><div class=\"menu_news\">ニュース</div></a>");
            writer.WriteLine("<a href=\"anime.html\"><div class=\"menu_anime\">サブカル</div></a>");
            writer.WriteLine("<a href=\"others.html\"><div class=\"menu_others\">その他</div></a>");
            writer.WriteLine("</div>");
        }

        private static void WriteMainWindow(StreamWriter writer, Book book)
        {
            writer.WriteLine("<div class=\"rack\">");
            writer.WriteLine("<div class=\"mainwindow\">");
            writer.WriteLine("<table>");
            var lastDay = 0;
            foreach (var bundle in book.Bundles)
            {
                if (bundle.Articles[0].Date.Day != lastDay)
                {
                    writer.WriteLine("<tr><td class=\"date\" colspan=\"3\">" + GetDayText(bundle.Articles[0].Date) + "</td></tr>");
                }
                WriteMainWindowRow(writer, bundle);
                lastDay = bundle.Articles[0].Date.Day;
            }
            writer.WriteLine("</table>");
            writer.WriteLine("</div>");
            writer.WriteLine("</div>");
        }

        private static string GetDayText(DateTime date)
        {
            return date.Month + "月" + date.Day + "日(" + daysOfWeek[(int)date.DayOfWeek] + ")";
        }

        private static void WriteMainWindowRow(StreamWriter writer, Bundle bundle)
        {
            var sb = new StringBuilder();
            sb.Append(CreateLink(TrimKusa(bundle.Articles[0].Title), bundle.Articles[0].Title, bundle.Articles[0].Uri, "normallink"));
            foreach (var article in bundle.Articles.Skip(1))
            {
                sb.Append(" ");
                sb.Append(CreateLink("●", article.Blog.Title, article.Uri, "minorlink"));
            }
            writer.Write("<tr>");
            writer.Write("<td>" + bundle.Articles[0].Date.ToString("HH:mm") + "</td>");
            writer.Write("<td>" + sb + "</td>");
            writer.Write("<td>" + CreateLink_NoEscapeForTitle(bundle.Articles[0].Blog.Title, GetBlogStatsText(bundle.Articles[0].Blog), bundle.Articles[0].Blog.Uri, "normallink") + "</td>");
            writer.WriteLine("</tr>");
        }

        private static void WriteSubWindows(StreamWriter writer, Book book)
        {
            foreach (var pair in book.OrderByScore().Take(20).Buffer(2))
            {
                writer.WriteLine("<div class=\"rack\">");
                foreach (var blog in pair)
                {
                    writer.WriteLine("<div class=\"subwindow\">");
                    writer.WriteLine("<a title=\"" + GetBlogStatsText(blog) + "\" href=\"" + blog.Uri + "\"><div class=\"blogtitle\">" + blog.Title + "</div></a>");
                    writer.WriteLine("<table>");
                    var lastDay = 0;
                    foreach (var article in blog.Articles.Take(10))
                    {
                        if (article.Date.Day != lastDay)
                        {
                            writer.WriteLine("<tr><td class=\"date\" colspan=\"2\">" + GetDayText(article.Date) + "</td></tr>");
                        }
                        WriteSubWindowRow(writer, article);
                        lastDay = article.Date.Day;
                    }
                    writer.WriteLine("</table>");
                    writer.WriteLine("</div>");
                }
                writer.WriteLine("</div>");
            }
        }

        private static void WriteSubWindowRow(StreamWriter writer, Article article)
        {
            writer.Write("<tr>");
            writer.Write("<td>" + article.Date.ToString("HH:mm") + "</td>");
            writer.Write("<td>" + CreateLink(TrimKusa(article.Title), article.Title, article.Uri, "normallink") + "</td>");
            writer.WriteLine("</tr>");
        }

        private static string GetBlogStatsText(Blog blog)
        {
            var sb = new StringBuilder();
            sb.Append("記事数: ");
            sb.Append(blog.RssInfo.Stats.ArticlesPerDay.ToString("0.0"));
            sb.Append(" (件/日)&#13;&#10;");
            sb.Append("重複率: ");
            sb.Append((100 * blog.RssInfo.Stats.BundleRatio).ToString("0.0"));
            sb.Append(" (%)&#13;&#10;");
            sb.Append("スコア: ");
            sb.Append((100 * blog.RssInfo.Stats.Score).ToString("0.00"));
            return sb.ToString();
        }

        // 草を生やしまくってる記事タイトルがあるとレイアウトが崩れる。
        // これを回避するために長すぎる草を短くする。
        private static string TrimKusa(string value)
        {
            var count = new Dictionary<char, int>();
            foreach (var ch in value)
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
            foreach (var ch in value)
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
