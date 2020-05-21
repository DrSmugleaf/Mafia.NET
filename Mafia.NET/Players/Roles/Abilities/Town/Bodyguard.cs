using System.Linq;
using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum BodyguardKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Bodyguard", typeof(BodyguardSetup))]
    public class
        Bodyguard : TownAbility<BodyguardSetup>,
            ISwitcher // TODO The Bodyguard will stay together with his guarded target. That means he won't die if a Mass Murderer visits his target, if that target visited someone else that night.
    {
        public void Switch()
        {
            if (TargetManager.Try(out var guarded))
            {
                var threats = Match.Graveyard.Threats;

                for (var i = 0; i < threats.Count(); i++)
                {
                    var threat = threats[i];
                    if (threat.Victim != guarded) continue;

                    var newDeath = threat.WithVictim(User);
                    threats[i] = newDeath;

                    if (Setup.IgnoresInvulnerability)
                        PiercingAttack(threat.Killer);
                    else
                        Attack(threat.Killer);
                }
            }
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<BodyguardKey>());
        }
    }

    public class BodyguardSetup : ITownSetup
    {
        public bool CanBeHealed = false;
        public bool IgnoresInvulnerability = true;
        public bool PreventsCultistConversion = false; // TODO: Prevents conversions
    }
}