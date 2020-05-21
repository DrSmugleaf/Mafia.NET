using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum CoronerKey
    {
        StillAlive,
        AutopsyRole,
        AutopsyLastWill,
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Coroner", typeof(CoronerSetup))]
    public class Coroner : TownAbility<CoronerSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            if (target.Alive)
            {
                User.OnNotification(Entry.Chat(CoronerKey.StillAlive));
                return;
            }

            var message = new EntryBundle();
            message.Chat(CoronerKey.AutopsyRole, target, target.Role);
            if (Setup.DiscoverLastWill && target.LastWill.Text.Length > 0)
                message.Chat(CoronerKey.AutopsyLastWill, target.LastWill);

            User.OnNotification(message);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Dead(Match), TargetNotification.Enum<CoronerKey>());
        }
    }

    public class CoronerSetup : ITownSetup
    {
        public bool DiscoverAllTargets = true; // TODO: Discover targets
        public bool DiscoverDeathType = true; // TODO: Death type
        public bool DiscoverLastWill = true;
    }
}