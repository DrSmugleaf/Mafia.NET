using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities;

namespace Mafia.NET.Players.Roles
{
    public class RolePriority
    {
        public RolePriority(IMatch match)
        {
            Match = match;
            StartOrder = new List<Action<IAbility>>()
            {
                ability => ability.Chat(),
                ability => ability.Detain()
            };
            
            EndOrder = new List<Action<IAbility>>()
            {
                ability => ability.Vest(),
                ability => ability.Switch(),
                ability => ability.Try(ability.Block),
                ability => ability.Try(ability.Misc),
                ability => ability.Try(ability.Kill),
                ability => ability.Try(ability.Clean),
                ability => ability.Try(ability.Detect),
                ability => ability.Try(ability.Disguise),
                ability => ability.Try(ability.MasonRecruit),
                ability => ability.Try(ability.CultRecruit)
            };
        }
        
        public IMatch Match { get; }
        public IList<Action<IAbility>> StartOrder { get; }
        public IList<Action<IAbility>> EndOrder { get; }

        public IList<IAbility> Abilities()
        {
            var abilities = new List<IAbility>();

            foreach (var player in Match.AllPlayers)
                abilities.Add(player.Role.Ability);

            return abilities.OrderBy(ability => ability.User.Number).ToList();
        }

        public void OnNightStart()
        {
            foreach (var action in StartOrder)
            foreach (var ability in Abilities())
                action(ability);
        }

        public void OnNightEnd()
        {
            foreach (var action in EndOrder)
            foreach (var ability in Abilities())
                action(ability);
        }
    }
}