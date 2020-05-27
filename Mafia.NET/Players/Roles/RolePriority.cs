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
            StartOrder = new List<Action<IAbility>>
            {
                ability => ability.Chat(),
                ability => ability.Detain()
            };

            EndOrder = new List<Action<IAbility>>
            {
                ability => ability.Vest(),
                ability => ability.Switch(),
                ability => ability.Block(),
                ability => ability.Misc(),
                ability => ability.Kill(),
                ability => ability.Protect(),
                ability => ability.Clean(),
                ability => ability.Detect(),
                ability => ability.Disguise(),
                ability => ability.MasonRecruit(),
                ability => ability.CultRecruit()
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
                ability.Try(action);

            foreach (var ability in Abilities())
                ability.OnNightStart();
        }

        public void OnNightEnd()
        {
            foreach (var action in EndOrder)
            foreach (var ability in Abilities())
                ability.Try(action);
        }
    }
}