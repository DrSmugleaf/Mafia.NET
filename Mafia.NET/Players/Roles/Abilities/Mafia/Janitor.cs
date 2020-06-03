using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum JanitorKey
    {
        LastWillReveal,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Janitor", typeof(JanitorSetup))]
    public class Janitor : MafiaAbility<JanitorSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var sanitize = new Sanitize(this);
            actions.Add(sanitize);
        }

        protected override void _onNightStart()
        {
            if (Uses == 0) return;

            AddTarget(TargetFilter.Living(Match), TargetNotification.Enum<JanitorKey>());
        }
    }

    public class JanitorSetup : MafiaMinionSetup, IUsesSetup
    {
        public int Uses { get; set; } = 2;
    }
}