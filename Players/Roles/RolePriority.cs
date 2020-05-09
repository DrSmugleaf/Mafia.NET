using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Town;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Players.Roles
{
    public class RolePriority
    {
        private static readonly Lazy<RolePriority> Lazy = new Lazy<RolePriority>(() => new RolePriority());
        public static RolePriority Instance { get => Lazy.Value; }

        public IList<T> Abilities<T>(IEnumerable<IPlayer> players) where T : IAbility
        {
            var abilities = new List<T>();

            foreach (var player in players)
            {
                if (player.Role.Ability is T ability) abilities.Add(ability);
            }

            return abilities;
        }

        public void OnNightStart(IMatch match)
        {
            var living = match.LivingPlayers.Values;

            foreach (var chatter in Abilities<INightChatter>(living)) chatter.Chat();
            foreach (var detainer in Abilities<IDetainer>(living)) detainer.Try(detainer.Detain);
        }

        public void OnNightEnd(IMatch match)
        {
            var living = match.LivingPlayers.Values;

            foreach (var switcher in Abilities<ISwitcher>(living)) switcher.Switch();
            foreach (var blocker in Abilities<IRoleBlocker>(living)) blocker.Try(blocker.Block);
            foreach (var misc in Abilities<IMisc>(living)) misc.Try(misc.Misc);
            foreach (var killer in Abilities<IKiller>(living)) killer.Try(killer.Kill);
            foreach (var cleaner in Abilities<ICleaner>(living)) cleaner.Try(cleaner.Clean);
            foreach (var detector in Abilities<IDetector>(living)) detector.Try(detector.Detect);
            foreach (var disguiser in Abilities<IDisguiser>(living)) disguiser.Try(disguiser.Disguise);
            foreach (var mason in Abilities<IMasonRecruiter>(living)) mason.Try(mason.MasonRecruit);
            foreach (var cult in Abilities<ICultRecruiter>(living)) cult.Try(cult.CultRecruit);
        }
    }
}
