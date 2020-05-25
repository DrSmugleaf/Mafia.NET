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
    public class Bodyguard : TownAbility<BodyguardSetup>
    {
        // TODO The Bodyguard will stay together with his guarded target. That means he won't die if a Mass Murderer visits his target, if that target visited someone else that night.
        public override bool HealedBy(IPlayer healer)
        {
            if (Setup.CanBeHealed) return base.HealedBy(healer);
            return Match.Graveyard.ThreatsOn(User).Count > 0;
        }

        public override void Protect(IPlayer target)
        {
            if (TargetManager.Try(out var guarded))
            {
                var threats = Match.Graveyard.ThreatsOn(guarded);
                if (threats.Count > 0)
                {
                    var threat = threats[0];
                    threat.WithVictim(User);

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