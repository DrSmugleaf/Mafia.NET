using Mafia.NET.Extension;
using Mafia.NET.Players.Teams;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Tests.Resources;

[TestFixture]
public class TeamTest
{
    [Test]
    public void Formatting()
    {
        foreach (var yaml in TeamRegistry.LoadYaml())
        {
            var children = yaml.Children;

            Assert.That(children, Contains.Key((YamlNode) "id"));
            Assert.That(children["id"].AsString().Length, Is.Positive);

            Assert.That(children, Contains.Key((YamlNode) "color"));
            var color = children["color"].AsColor();
            Assert.NotNull(color);

            Assert.That(children, Contains.Key((YamlNode) "order"));
            var order = children["order"].AsInt();
            Assert.NotNull(order);
        }
    }
}