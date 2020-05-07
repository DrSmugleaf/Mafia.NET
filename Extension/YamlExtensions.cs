using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Extension
{
    public static class YamlExtensions
    {
        public static YamlNode Get(this YamlMappingNode node, string key)
        {
            var keyNode = new YamlScalarNode(key);
            return node[keyNode];
        }

        public static bool IsNull(this YamlNode node)
        {
            var value = node.AsString();
            switch (value)
            {
                case "null":
                case "Null":
                case "NULL":
                case "~":
                    return true;
                default:
                    return false;
            }
        }

        public static bool Contains(this YamlMappingNode node, string key) => node.Children.ContainsKey(key);

        public static string AsString(this YamlNode node) => ((YamlScalarNode)node).Value;

        public static int AsInt(this YamlNode node) => int.Parse(((YamlScalarNode)node).Value);

        public static Color AsColor(this YamlNode node) => ColorTranslator.FromHtml(node.AsString());

        public static bool AsBool(this YamlNode node)
        {
            bool.TryParse(node.AsString(), out bool result);
            return result;
        }

        public static T AsEnum<T>(this YamlNode node) => (T)Enum.Parse(typeof(T), node.AsString(), true);

        public static List<string> AsStringList(this YamlNode node) => node.AllNodes.Select(value => value.AsString()).ToList();
    }
}
