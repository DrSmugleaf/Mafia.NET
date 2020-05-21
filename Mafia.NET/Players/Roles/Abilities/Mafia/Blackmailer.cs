﻿using Mafia.NET.Localization;

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
    public class Blackmailer : MafiaAbility<BlackmailerSetup>, IMisc
    {
        public void Misc(IPlayer target)
        {
            target.Blackmailed = true;
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Team),
                TargetNotification.Enum<BlackmailerKey>());
        }
    }

    public class BlackmailerSetup : MafiaMinionSetup
    {
        public bool BlackmailedTalkDuringTrial = false;
    }
}