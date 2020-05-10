using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Resources
{
    public class Resource
    {
        public Resource(string path)
        {
            ResourcePath = Path.Combine(Directory.GetCurrentDirectory(), "Resources", path);
        }

        private string ResourcePath { get; }

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
            var resources = new List<Resource>();
            var files = Directory.GetFiles(directory, pattern, SearchOption.AllDirectories);

            foreach (var file in files)
            {
                var resource = new Resource(file);
                resources.Add(resource);
            }

            return resources;
        }

        public static explicit operator YamlSequenceNode(Resource resource)
        {
            using var reader = new StreamReader(resource.ResourcePath);
            var yaml = new YamlStream();
            yaml.Load(reader);
            return (YamlSequenceNode) yaml.Documents[0].RootNode;
        }

        public static explicit operator YamlMappingNode(Resource resource)
        {
            return (YamlMappingNode) ((YamlSequenceNode) resource)[0];
        }
    }
}