using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum WatchKey
    {
        SomeoneVisitedTarget,
        NoneVisitedTarget
    }

    public class Watch : AbilityAction<IDetectSetup>
    {
        public Watch(
            IAbility<IDetectSetup> ability,
            int priority = 9,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            var foreignVisits = new EntryBundle();
            foreach (var other in Match.LivingPlayers)
                if (other != User &&
                    other.Role.Ability.DetectTarget(out var foreignTarget, Setup) &&
                    foreignTarget == target)
                    foreignVisits.Chat(Ability, WatchKey.SomeoneVisitedTarget, other);

            if (foreignVisits.Entries.Count == 0)
                foreignVisits.Chat(Ability, WatchKey.NoneVisitedTarget);

            User.OnNotification(foreignVisits);

            return true;
        }
    }
}