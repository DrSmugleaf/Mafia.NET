using System.Drawing;
using Mafia.NET.Extension;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Tests.Resources;

[TestFixture]
public class RoleTest
{
    [Test]
    public void Formatting()
    {
        foreach (var yaml in RoleRegistry.LoadYaml())
        {
            var children = yaml.Children;

            Assert.That(children, Contains.Key((YamlNode) "id"));
            Assert.That(children["id"].AsString().Length, Is.Positive);

            Assert.That(children, Contains.Key((YamlNode) "team"));
            var team = children["team"].AsString();
            Assert.That(team.Length, Is.Positive);
            Assert.NotNull(TeamRegistry.Default[team]);

            Assert.That(children, Contains.Key((YamlNode) "categories"));
            var categoryNames = children["categories"];
            Assert.That(categoryNames.AsStringList().Count, Is.Positive);
            foreach (var category in (YamlSequenceNode) categoryNames)
                Assert.NotNull(CategoryRegistry.Default[category.AsString()]);

            Assert.That(children, Contains.Key((YamlNode) "color"));
            Assert.That(children["color"].AsColor(), Is.Not.EqualTo(Color.Empty));
        }
    }
}