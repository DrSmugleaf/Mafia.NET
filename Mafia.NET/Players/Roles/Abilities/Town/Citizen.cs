using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum CitizenKey
    {
        UsesLeft,
        UsesLeftPlural,
        UsedUp,
        UsedUpNow,
        UserAddMessage,
        UserRemoveMessage
    }

    [RegisterAbility("Citizen", typeof(CitizenSetup))]
    public class Citizen : TownAbility<CitizenSetup>
    {
        public override void Initialize(IPlayer user)
        {
            base.Initialize(user);
            Uses = Setup.OneBulletproofVest ? 1 : 0;
        }

        public override void Vest()
        {
            if (Uses == 0 || !TargetManager.Try(out _)) return;

            var notification = Notification.Chat(CitizenKey.UsedUpNow);
            User.OnNotification(notification);

            CurrentlyDeathImmune = true;
        }

        protected override void _onNightStart()
        {
            if (Uses == 0)
            {
                User.OnNotification(Notification.Chat(CitizenKey.UsedUp));
                return;
            }

            var usesLeft = Notification.Chat(Uses == 1 ? CitizenKey.UsesLeft : CitizenKey.UsesLeftPlural);
            User.OnNotification(usesLeft);

            AddTarget(TargetFilter.Only(User), TargetNotification.Enum<CitizenKey>());
        }
    }

    public class CitizenSetup : ITownSetup
    {
        public bool OneBulletproofVest = true;
        public bool WinTiesAgainstMafia = true; // TODO 1:1s and tiebreakers
    }
}