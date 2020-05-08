using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Jailor", typeof(JailSetup))]
    public class Jailor : BaseAbility<JailSetup>
    {
        public int Executions { get; set; }

        public Jailor()
        {
            Executions = Setup.Executions;
        }

        protected override void _onDayStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetMessage()
            {
                UserAddMessage = (target) => $"You will jail {target.Name}.",
                UserRemoveMessage = (target) => $"You won't jail anyone.",
                UserChangeMessage = (old, _new) => $"You will instead jail {_new.Name}."
            });
        }

        protected override void _onDayEnd()
        {
            if (Match.Graveyard.LynchedToday())
            {
                TargetManager[0] = null;
                User.OnNotification(Notification.Chat("You are unable to jail anyone due to today's lynch."));
            }

            TargetManager[0]?.Role.Ability.PiercingDisable();
        }

        protected override void _onNightStart()
        {
            if (TargetManager.TryDay(0, out var prisoner))
            {
                var jail = Match.Chat.Open("Jailor", User, prisoner);
                var jailor = jail.Participants[User];
                jailor.Name = "Jailor";

                AddTarget(Executions > 0 ? prisoner : null, new TargetMessage()
                {
                    UserAddMessage = (target) => $"You will execute {target.Name}.",
                    UserRemoveMessage = (target) => $"You changed your mind.",
                    TargetAddMessage = (target) => $"{jailor.Name} will execute {target.Name}.",
                    TargetRemoveMessage = (target) => $"{jailor.Name} changed their mind."
                }); ;

                prisoner.Role.Ability.CurrentlyDeathImmune = true;
                Match.Chat.DisableExcept(prisoner, jail);
            }
        }

        protected override bool _onNightEnd()
        {
            if (TargetManager.Try(0, out var execution) && Executions > 0)
            {
                Executions--;
                PiercingThreaten(execution);
                return true;
            }

            return false;
        }
    }

    public class JailSetup : IAbilitySetup
    {
        public int Executions = 1;
    }
}
