using System.Linq;
using Mafia.NET.Extension;
using NUnit.Framework;
using YamlDotNet.RepresentationModel;

namespace Mafia.NET.Tests.Extension;

[TestFixture]
[TestOf(typeof(YamlExtensions))]
public class YamlExtensionsTest
{
    [TestCase("Yes")]
    [TestCase("No")]
    public void AsBoolCatch(string value)
    {
        var node = new YamlScalarNode(value);
        Assert.Catch(() => node.AsBool());
    }

    public enum Enum
    {
        Value,
        Test
    }

    [TestCase("Value", Enum.Value)]
    [TestCase("test", Enum.Test)]
    public void AsEnum(string value, Enum expected)
    {
        var node = new YamlScalarNode(value);
        var returns = node.AsEnum<Enum>();

        Assert.That(returns, Is.EqualTo(expected));
    }

    [TestCase("")]
    [TestCase("key")]
    public void AsEnumCatch(string value)
    {
        var node = new YamlScalarNode(value);

        Assert.Catch(() => node.AsEnum<Enum>());
    }

    [TestCase("key", "test", "value")]
    [TestCase("1", "2", "2", "value", "value")]
    public void AsStringList(params string[] values)
    {
        var list = values.Select(value => new YamlScalarNode(value));
        var node = new YamlSequenceNode(list);

        Assert.AreEqual(node.AsStringList(), values.ToList());
    }

    [Test]
    [TestCase("true", true)]
    [TestCase("True", true)]
    [TestCase("TrUe", true)]
    [TestCase("false", false)]
    [TestCase("False", false)]
    [TestCase("FaLsE", false)]
    public void AsBool(string value, bool expected)
    {
        var node = new YamlScalarNode(value);
        var returns = node.AsBool();

        Assert.That(returns, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("0xFFFFFF", new[] {255, 255, 255})]
    [TestCase("0xA01B56", new[] {160, 27, 86})]
    public void AsColor(string value, int[] expected)
    {
        var node = new YamlScalarNode(value);
        var returns = node.AsColor();

        Assert.That(returns.R, Is.EqualTo(expected[0]));
        Assert.That(returns.G, Is.EqualTo(expected[1]));
        Assert.That(returns.B, Is.EqualTo(expected[2]));
    }

    [Test]
    [TestCase("2", 2)]
    [TestCase("-4", -4)]
    public void AsInt(string value, int expected)
    {
        var node = new YamlScalarNode(value);
        var returns = node.AsInt();

        Assert.That(returns, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("value", "value")]
    [TestCase("5", "5")]
    public void AsString(string value, string expected)
    {
        var node = new YamlScalarNode(value);
        var returns = node.AsString();

        Assert.That(returns, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("Key", true)]
    [TestCase("Value", false)]
    public void Contains(string value, bool expected)
    {
        var node = new YamlMappingNode(new YamlScalarNode("Key"), new YamlScalarNode("Value"));
        var returns = node.Contains(value);

        Assert.That(returns, Is.EqualTo(expected));
    }

    [Test]
    [TestCase("null", true)]
    [TestCase("Null", true)]
    [TestCase("NULL", true)]
    [TestCase("~", true)]
    [TestCase("", false)]
    [TestCase("None", false)]
    [TestCase("0", false)]
    [TestCase("false", false)]
    public void IsNull(string value, bool expected)
    {
        var node = new YamlScalarNode(value);
        var returns = node.IsNull();

        Assert.That(returns, Is.EqualTo(expected));
    }
}