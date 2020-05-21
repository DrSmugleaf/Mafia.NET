using Mafia.NET.Localization;
using Mafia.NET.Matches;

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
    public class Citizen : TownAbility<CitizenSetup>, IVest
    {
        public override void Initialize(IMatch match, IPlayer user)
        {
            base.Initialize(match, user);
            Uses = Setup.OneBulletproofVest ? 1 : 0;
        }

        public void Vest()
        {
            if (Uses == 0 || !TargetManager.Try(out _)) return;

            var notification = Entry.Chat(CitizenKey.UsedUpNow);
            User.OnNotification(notification);

            CurrentlyDeathImmune = true;
        }

        protected override void _onNightStart()
        {
            if (Uses == 0)
            {
                User.OnNotification(Entry.Chat(CitizenKey.UsedUp));
                return;
            }

            var usesLeft = Entry.Chat(Uses == 1 ? CitizenKey.UsesLeft : CitizenKey.UsesLeftPlural);
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