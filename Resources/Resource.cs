using System.Collections.Generic;
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

        public static string GetResourcesDirectory()
        {
            return Path.Combine(Directory.GetCurrentDirectory(), "Resources");
        }

        public static string GetResourcesDirectory(string directory)
        {
            return Path.Combine(GetResourcesDirectory(), directory);
        }

        public static List<Resource> FromDirectory(string directory, string pattern)
        {
            directory = GetResourcesDirectory(directory);
            List<Resource> resources = new List<Resource>();
            var files = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                Resource resource = new Resource(file);
                resources.Add(resource);
            }

            return resources;
        }

        public static explicit operator YamlSequenceNode(Resource resource)
        {
            using var reader = new StreamReader(resource.ResourcePath);
            var yaml = new YamlStream();
            yaml.Load(reader);
            return (YamlSequenceNode)yaml.Documents[0].RootNode;
        }

        public static explicit operator YamlMappingNode(Resource resource) => (YamlMappingNode)((YamlSequenceNode)resource)[0];
    }
}
