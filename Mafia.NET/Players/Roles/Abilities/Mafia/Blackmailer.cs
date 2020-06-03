using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterKey]
    public enum BlackmailerKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Blackmailer", typeof(BlackmailerSetup))]
    public class Blackmailer : MafiaAbility<BlackmailerSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var blackmail = new Blackmail(this);
            actions.Add(blackmail);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team),
                TargetNotification.Enum<BlackmailerKey>());
        }
    }

    public class BlackmailerSetup : MafiaMinionSetup, IBlackmailSetup
    {
        public bool BlackmailedTalkDuringTrial { get; set; } = false;
    }
}