using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Citizen", typeof(CitizenSetup))]
    public class Citizen : TownAbility<CitizenSetup>, IVest
    {
        public override void Initialize(IMatch match)
        {
            base.Initialize(match);
            Uses = Setup.OneBulletproofVest ? 1 : 0;
        }

        public void Vest()
        {
            if (Uses == 0 || !TargetManager.Try(out _)) return;

            var notification = Notification.Chat("Your bulletproof vest is now used up.");
            User.OnNotification(notification);

            CurrentlyDeathImmune = true;
        }

        protected override void _onNightStart()
        {
            if (Uses == 0)
            {
                User.OnNotification(Notification.Chat("Your bulletproof vest is used up."));
                return;
            }

            var notification =
                Notification.Chat($"Your bulletproof vest has {Uses} {(Uses == 1 ? "use" : "uses")} left.");
            User.OnNotification(notification);

            AddTarget(TargetFilter.Only(User), new TargetNotification
            {
                UserAddMessage = target => "You put on your bulletproof vest.",
                UserRemoveMessage = target => "You take off your bulletproof vest."
            });
        }
    }

    public class CitizenSetup : ITownSetup
    {
        public bool OneBulletproofVest = true;
        public bool WinTiesAgainstMafia = true; // TODO 1:1s and tiebreakers
    }
}