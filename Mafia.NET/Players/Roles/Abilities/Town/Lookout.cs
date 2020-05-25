using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum LookoutKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        SomeoneVisitedTarget,
        NoneVisitedTarget
    }
    
    [RegisterAbility("Lookout", typeof(LookoutSetup))]
    public class Lookout : TownAbility<LookoutSetup>
    {
        public override void Detect(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);
            
            var foreignVisits = new EntryBundle();
            foreach (var other in Match.LivingPlayers)
            {
                if (other.Role.Ability.DetectTarget(out var foreignTarget) &&
                    foreignTarget == target) 
                    foreignVisits.Chat(LookoutKey.SomeoneVisitedTarget, other);
            }

            if (foreignVisits.Entries.Count == 0) foreignVisits.Chat(LookoutKey.NoneVisitedTarget);

            User.OnNotification(foreignVisits);
        }

        protected override void _onNightStart()
        {
            var filter = TargetFilter.Living(Match);
            if (!Setup.CanTargetSelf)
                filter = filter.Except(User);
            
            AddTarget(filter, TargetNotification.Enum<LookoutKey>());
        }
    }

    public class LookoutSetup : ITownSetup, IIgnoresDetectionImmunity
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
        public bool CanTargetSelf = false;
    }
}