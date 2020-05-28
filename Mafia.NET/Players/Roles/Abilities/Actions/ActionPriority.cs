using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class ActionPriority
    {
        public ActionPriority(IMatch match)
        {
            Match = match;
            StartOrder = new List<Action<IAbilityAction>>
            {
                ability => ability.Chat(),
                ability => ability.Detain()
            };

            EndOrder = new List<Action<IAbilityAction>>
            {
                ability => ability.Vest(),
                ability => ability.Switch(),
                ability => ability.Block(),
                ability => ability.Misc(),
                ability => ability.Kill(),
                ability => ability.Revenge(),
                ability => ability.Protect(),
                ability => ability.Clean(),
                ability => ability.Detect(),
                ability => ability.Disguise(),
                ability => ability.MasonRecruit(),
                ability => ability.CultRecruit()
            };
        }

        public IMatch Match { get; }
        public IList<Action<IAbilityAction>> StartOrder { get; }
        public IList<Action<IAbilityAction>> EndOrder { get; }

        public IList<IPlayer> Players()
        {
            var abilities = new List<IPlayer>();

            foreach (var player in Match.AllPlayers)
                abilities.Add(player);

            return abilities.OrderBy(player => player.Number).ToList();
        }

        public void OnDayStart()
        {
            foreach (var player in Players())
                player.Role.Ability.OnDayStart();
        }

        public void OnDayEnd()
        {
            foreach (var player in Players())
                player.Role.Ability.OnDayEnd();
        }

        public void OnNightStart()
        {
            foreach (var action in StartOrder)
            foreach (var player in Players())
                player.Role.Ability.Try(action);

            foreach (var player in Players())
                player.Role.Ability.OnNightStart();
        }

        public void BeforeNightEnd()
        {
            foreach (var player in Players())
                player.Role.Ability.BeforeNightEnd();
        }

        public void OnNightEnd()
        {
            foreach (var action in EndOrder)
            foreach (var player in Players())
                player.Role.Ability.Try(action);

            foreach (var player in Players())
                player.Role.Ability.OnNightEnd();
        }
    }
}