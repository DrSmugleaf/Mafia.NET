using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum InvestigateKey
    {
        ExactDetect,
        Detect
    }

    public class Investigate : AbilityAction<IInvestigativeSetup>
    {
        public Investigate(IAbility<IInvestigativeSetup> ability, int priority = 9, bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);

            // TODO: Target switch
            var message = Setup.DetectsExactRole
                ? Notification.Chat(Ability, InvestigateKey.ExactDetect, target, target.Crimes.RoleName())
                : target.Crimes.Crime(Ability, InvestigateKey.Detect);

            User.OnNotification(message);

            return true;
        }
    }

    public interface IInvestigativeSetup : IAbilitySetup
    {
        public bool DetectsExactRole { get; set; }
    }
}