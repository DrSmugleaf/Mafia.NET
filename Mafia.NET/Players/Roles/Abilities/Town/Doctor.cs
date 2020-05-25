using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum DoctorKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetAttacked
    }
    
    [RegisterAbility("Doctor", typeof(DoctorSetup))]
    public class Doctor : TownAbility<DoctorSetup>
    {
        public override void Protect(IPlayer target)
        {
            if (TargetManager.Try(out var healed))
            {
                if (healed.Role.Ability.Heal(User) && Setup.KnowsIfTargetAttacked)
                {
                    var notification = Notification.Chat(DoctorKey.TargetAttacked);
                    User.OnNotification(notification);
                }
            }
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<DoctorKey>());
        }
    }

    public class DoctorSetup : ITownSetup
    {
        public bool KnowsIfTargetAttacked = true;
        public bool PreventsCultistConversion = false; // TODO
        public bool KnowsIfTargetConverted = false;
        public bool WitchDoctorWhenConverted = false;
    }
}