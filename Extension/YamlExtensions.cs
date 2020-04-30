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

        public static string AsString(this YamlNode node) => ((YamlScalarNode)node).Value;

        public static int AsInt(this YamlNode node) => int.Parse(((YamlScalarNode)node).Value);
    }
}
