using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Town;
using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Kidnapper", typeof(KidnapperSetup))]
    public class Kidnapper : MafiaAbility<KidnapperSetup>
    {
        public Kidnapper()
        {
            Charges = Match.Abilities.Setup<JailorSetup>().Charges;
        }

        protected override void _onDayStart()
        {
            TargetFilter filter = TargetFilter.Living(Match);
            if (!Setup.CanKidnapMafiaMembers) filter = filter.Except(User.Role.Affiliation);

            AddTarget(filter, new TargetMessage()
            {
                UserAddMessage = (target) => $"You will jail {target.Name}.",
                UserRemoveMessage = (target) => "You won't jail anyone.",
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
                User.Crimes.Add("Kidnapping");

                var jail = Match.Chat.Open("Jailor", User, prisoner);
                var allies = Match.LivingPlayers.Values.Where(player => player.Role.Affiliation == User.Role.Affiliation && player != User);
                jail.Add(allies, true, false);
                var jailor = jail.Participants[User];
                jailor.Name = "Jailor";

                AddTarget(prisoner, new TargetMessage()
                {
                    UserAddMessage = (target) => $"You will execute {target.Name}.",
                    UserRemoveMessage = (target) => "You changed your mind.",
                    TargetAddMessage = (target) => $"{jailor.Name} will execute {target.Name}.",
                    TargetRemoveMessage = (target) => $"{jailor.Name} changed their mind."
                });

                prisoner.Role.Ability.CurrentlyDeathImmune = true;

                if (prisoner.Role.Affiliation != User.Role.Affiliation)
                {
                    Match.Chat.DisableExcept(prisoner, jail);
                }
            }
        }

        protected override bool _afterNightEnd()
        {
            if (TargetManager.Try(0, out var execution) && Charges > 0)
            {
                Charges--;
                PiercingThreaten(execution);
                return true;
            }

            return false;
        }
    }

    public class KidnapperSetup : MafiaMinionSetup
    {
        public bool CanKidnapMafiaMembers = false;
    }
}
