using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Roles.Selectors;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Teams
{
    public interface ITeam : IColorizable, ILocalizable, IRegistrable
    {
        Key Name { get; }
        int Order { get; }

        List<IRoleSelector> Selectors(RoleRegistry roles);
    }

    public class Team : ITeam
    {
        public Team(string id, Color color, int order)
        {
            Id = id;
            Name = new Key($"{id}name");
            Color = color;
            Order = order;
        }

        public string Id { get; }
        public Key Name { get; }
        public Color Color { get; }
        public int Order { get; }

        public List<IRoleSelector> Selectors(RoleRegistry roles)
        {
            var selectors = new List<IRoleSelector>();

            foreach (var category in CategoryRegistry.Default.Entries())
            {
                var selector = new RoleSelector(roles, category);
                selectors.Add(selector);
            }

            var random = new RoleSelector(roles, this);
            selectors.Add(random);

            return selectors;
        }

        public Text Localize(CultureInfo culture = null)
        {
            return Name.Localize(culture);
        }

        public override string ToString()
        {
            return Localize().ToString();
        }
    }
}