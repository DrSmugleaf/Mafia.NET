using System;
using System.Collections.Generic;
using System.IO;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Resources;

public class Resource
{
    public Resource(string path)
    {
        ResourcePath = path;
    }

    public string ResourcePath { get; }

    public static string GetResourcesDirectory()
    {
        return Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources");
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

    public YamlStream ToYamlStream()
    {
        using var reader = new StreamReader(ResourcePath);
        var yaml = new YamlStream();
        yaml.Load(reader);
        return yaml;
    }

    public static explicit operator YamlSequenceNode(Resource resource)
    {
        var yaml = resource.ToYamlStream();
        return (YamlSequenceNode) yaml.Documents[0].RootNode;
    }

    public static explicit operator YamlMappingNode(Resource resource)
    {
        var yaml = resource.ToYamlStream();
        return (YamlMappingNode) yaml.Documents[0].RootNode;
    }
}