using System.Linq;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Bodyguard", typeof(BodyguardSetup))]
    public class Bodyguard : TownAbility<BodyguardSetup>, ISwitcher // TODO The Bodyguard will stay together with his guarded target. That means he won't die if a Mass Murderer visits his target, if that target visited someone else that night.
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetMessage()
            {
                UserAddMessage = (target) => $"You will guard {target.Name}.",
                UserRemoveMessage = (target) => "You won't guard anyone.",
                UserChangeMessage = (old, _new) => $"You will instead guard {_new.Name}."
            });
        }

        public void Switch()
        {
            if (TargetManager.Try(out var guarded))
            {
                var threats = Match.Graveyard.Threats;

                for (int i = 0; i < threats.Count(); i++)
                {
                    var threat = threats[i];
                    if (threat.Victim != guarded) continue;

                    var newDeath = threat.WithVictim(User);
                    threats[i] = newDeath;

                    if (Setup.IgnoresInvulnerability)
                    {
                        PiercingAttack(threat.Killer);
                    }
                    else
                    {
                        Attack(threat.Killer);
                    }
                }
            }
        }
    }

    public class BodyguardSetup : ITownSetup
    {
        public bool IgnoresInvulnerability = true;
        public bool CanBeHealed = false;
        public bool PreventsCultistConversion = false; // TODO: Prevents conversions
    }
}
