using System.IO;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Resources
{
    class Resource
    {
        private string ResourcePath { get; }

        public Resource(string path)
        {
            ResourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", path);
        }

        public static implicit operator YamlSequenceNode(Resource resource)
        {
            using var reader = new StreamReader(resource.ResourcePath);
            var yaml = new YamlStream();
            yaml.Load(reader);
            return (YamlSequenceNode)yaml.Documents[0].RootNode;
        }
    }
}
