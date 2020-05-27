using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum SheriffKey
    {
        NotSuspicious,
        Mafia,
        Triad,
        Cultist,
        Arsonist,
        MassMurderer,
        SerialKiller
    }

    [RegisterAbility("Sheriff", typeof(SheriffSetup))]
    public class Sheriff : TownAbility<SheriffSetup>
    {
        public override void Detect()
        {
            if (!TargetManager.Try(out var target)) return;
            var message = target.Crimes.Sheriff(Setup).Chat();
            User.OnNotification(message);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<SheriffKey>());
        }
    }

    public class SheriffSetup : ITownSetup, ISheriffSetup
    {
        public bool DetectsMafiaTriad { get; set; } = true;
        public bool DetectsSerialKiller { get; set; } = true;
        public bool DetectsArsonist { get; set; } = true;
        public bool DetectsCult { get; set; } = true;
        public bool DetectsMassMurderer { get; set; } = true;
    }

    public interface ISheriffSetup : IAbilitySetup
    {
        bool DetectsMafiaTriad { get; set; }
        bool DetectsSerialKiller { get; set; }
        bool DetectsArsonist { get; set; }
        bool DetectsCult { get; set; }
        bool DetectsMassMurderer { get; set; }
    }
}