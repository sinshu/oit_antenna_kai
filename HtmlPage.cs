﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace OitAntennaKai
{
    internal static class HtmlPage
    {
        private static readonly string daysOfWeek = "日月火水木金土";

        public static void CreateMainPage(string path, Category category)
        {
            using (var writer = new StreamWriter(path, false, Encoding.UTF8))
            {
                BeginHtml(writer);
                WriteHeader(writer, Setting.PageTitle);
                BeginBody(writer);
                WriteTitle(writer);
                WriteMainWindow(writer, category);
                WriteSubWindows(writer, category);
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

        private static void WriteHeader(StreamWriter writer, string title)
        {
            writer.WriteLine("<head>");
            writer.WriteLine("<meta charset=\"UTF-8\">");
            writer.WriteLine("<title>" + title + "</title>");
            writer.WriteLine("<link rel=\"stylesheet\" type=\"text/css\" href=\"style.css\">");
            writer.WriteLine("</head>");
        }

        private static void BeginBody(StreamWriter writer)
        {
            writer.WriteLine("<body>");
        }

        private static void EndBody(StreamWriter writer)
        {
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

        private static void WriteMainWindow(StreamWriter writer, Category category)
        {
            writer.WriteLine("<div class=\"rack\">");
            writer.WriteLine("<div class=\"mainwindow\">");
            writer.WriteLine("<table>");
            var lastDay = 0;
            foreach (var bundle in category.Bundles)
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
            sb.Append(CreateLink(bundle.Articles[0].Title, bundle.Articles[0].Blog.Title, bundle.Articles[0].Uri, "normallink"));
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

        private static void WriteSubWindows(StreamWriter writer, Category category)
        {
            foreach (var pair in category.OrderByUnko().Take(20).Buffer(2))
            {
                writer.WriteLine("<div class=\"rack\">");
                foreach (var blog in pair)
                {
                    writer.WriteLine("<div class=\"subwindow\">");
                    writer.WriteLine("<div class=\"blogtitle\">" + CreateLink_NoEscapeForTitle(blog.Title, GetBlogStatsText(blog), blog.Uri, "blogtitle") + "</div>");
                    writer.WriteLine("<table>");
                    foreach (var articles in blog.Articles.Take(10))
                    {
                        writer.WriteLine("<tr><td>" + CreateLink(articles.Title, blog.Title, articles.Uri, "normallink") + "</td></tr>");
                    }
                    writer.WriteLine("</table>");
                    writer.WriteLine("</div>");
                }
                writer.WriteLine("</div>");
            }
        }

        private static string GetBlogStatsText(Blog blog)
        {
            var sb = new StringBuilder();
            sb.Append("記事数: ");
            sb.Append(blog.Stats.ArticlesPerDay.ToString("0.0"));
            sb.Append(" (件/日)&#13;&#10;");
            sb.Append("重複率: ");
            sb.Append((100 * blog.Stats.BundleRatio).ToString("0.0"));
            sb.Append(" (%)&#13;&#10;");
            sb.Append("スコア: ");
            sb.Append((100 * blog.Stats.Score).ToString("0.00"));
            return sb.ToString();
        }
    }
}