using Mafia.NET.Localization;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum VigilanteKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Vigilante", typeof(VigilanteSetup))]
    public class Vigilante : TownAbility<VigilanteSetup>
    {
        public override void Kill()
        {
            if (!TargetManager.Try(out var target)) return;
            Attack(target);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<VeteranKey>());
        }
    }

    public class VigilanteSetup : ITownSetup, IUsesSetup
    {
        public int Uses { get; set; } = 2;
    }
}