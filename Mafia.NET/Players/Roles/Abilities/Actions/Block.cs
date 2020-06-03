using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum BlockKey
    {
        TargetBlockImmune
    }

    public class Block : AbilityAction<IBlockSetup>
    {
        public Block(
            IAbility<IBlockSetup> ability,
            int priority = 3,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Soliciting);

            if (target.Role.Team.Id == "Town") User.Crimes.Add(CrimeKey.DisturbingThePeace);

            var blocked = target.Ability.BlockedBy(User);

            if (blocked) return true;
            if (Setup.DetectsBlockImmune)
            {
                var notification = Notification.Chat(Ability, BlockKey.TargetBlockImmune);
                User.OnNotification(notification);
                return false;
            }

            return false;
        }
    }

    public interface IBlockSetup : IAbilitySetup
    {
        public bool DetectsBlockImmune { get; set; }
    }
}