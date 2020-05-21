using System;
using System.Drawing;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using Mafia.NET.Extension;

namespace Mafia.NET.Localization
{
    public class Parser
    {
        public readonly Regex BackgroundColor = new Regex(@"(<bc=(#[0-9a-fA-F]{6})>)");
        public readonly Regex Color = new Regex(@"({(\d+)c})");
        public readonly Regex NewLine = new Regex(@"(\\n)");

        public Text Parse(string str, NotificationLocation location, params object[] args)
        {
            var text = new Text(location);
            var argsList = args.ToList();

            foreach (Match match in Color.Matches(str))
            {
                var substr = match.Groups[1].Value;
                var index = int.Parse(match.Groups[2].Value);
                var color = ((IColorizable) args[index]).Color.HexRgb();
                str = str.Replace(substr, color);
            }

            foreach (Match match in BackgroundColor.Matches(str))
            {
                var substr = match.Groups[1].Value;
                var color = ColorTranslator.FromHtml(match.Groups[2].Value);
                str = str.Replace(substr, "");
                text.Background = color;
            }

            NewLine.Replace(str, Environment.NewLine);

            var doc = new HtmlDocument();
            doc.LoadHtml(str);

            foreach (var node in doc.DocumentNode.ChildNodes)
            {
                var c = node.GetAttributeValue("c", null);
                var color = ColorTranslator.FromHtml(c);
                var scale = node.GetAttributeValue("s", 1.0);
                var inner = node.InnerText;
                inner = string.Format(inner, args);

                text.Add(inner, color, scale);
            }

            return text;
        }
    }
}