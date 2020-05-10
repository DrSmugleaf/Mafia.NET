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
            return node[new YamlScalarNode(key)];
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

        public static bool Contains(this YamlMappingNode node, string key)
        {
            return node.Children.ContainsKey(key);
        }

        public static bool Try(this YamlNode root, string key, out YamlNode node)
        {
            node = null;
            var mapping = root as YamlMappingNode;
            if (mapping == null || !mapping.Contains(key)) return false;

            node = mapping[key];
            return true;
        }

        public static string AsString(this YamlNode node)
        {
            return ((YamlScalarNode) node).Value;
        }

        public static int AsInt(this YamlNode node)
        {
            return int.Parse(((YamlScalarNode) node).Value);
        }

        public static Color AsColor(this YamlNode node)
        {
            return ColorTranslator.FromHtml(node.AsString());
        }

        public static bool AsBool(this YamlNode node)
        {
            if (bool.TryParse(node.AsString(), out var result)) return result;
            throw new ArgumentException($"{node.AsString()} isn't a valid boolean value.");
        }

        public static T AsEnum<T>(this YamlNode node)
        {
            return (T) Enum.Parse(typeof(T), node.AsString(), true);
        }

        public static List<string> AsStringList(this YamlSequenceNode node)
        {
            return node.Children.Select(value => value.AsString()).ToList();
        }
    }
}