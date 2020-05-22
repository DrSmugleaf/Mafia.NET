using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Categories;
using Mafia.NET.Players.Teams;

namespace Mafia.NET.Players.Roles.Selectors
{
    public class SelectorGroup : IColorizable, ILocalizable
    {
        public SelectorGroup(string id, Key name, List<RoleSelector> selectors, Color color)
        {
            Id = id;
            Name = name;
            Selectors = selectors;
            Color = color;
        }

        public string Id { get; }
        public Key Name { get; }
        public List<RoleSelector> Selectors { get; }
        public Color Color { get; }

        public static List<SelectorGroup> Default(RoleRegistry roles)
        {
            var groups = new List<SelectorGroup>();
            
            var randomSelectors = new List<RoleSelector>();
            var anyRandom = new RoleSelector(roles);
            randomSelectors.Add(anyRandom);

            var teams = Team.All;
            foreach (var team in teams)
            {
                var selectors = roles.Team(team)
                    .Where(role => role.Natural)
                    .Select(role => new RoleSelector(role)).ToList();
                
                var group = new SelectorGroup(team.Id, team.Name, selectors, team.Color);
                groups.Add(group);
                
                var teamRandom = new RoleSelector(roles, team);
                randomSelectors.Add(teamRandom);
            }

            foreach (var category in Category.Categories.Values)
            {
                var selector = new RoleSelector(roles, category);
                randomSelectors.Add(selector);
            }
            
            var randomGroup = new SelectorGroup("Random", SelectorKey.Random, randomSelectors, ColorTranslator.FromHtml("#00CCFF"));
            groups.Add(randomGroup);

            return groups;
        }
        
        public static List<SelectorGroup> Default()
        {
            return Default(RoleRegistry.Default);
        }

        public List<IRoleSelector> Get(params string[] ids)
        {
            return Selectors.Where(selector => ids.Contains(selector.Id)).Cast<IRoleSelector>().ToList();
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