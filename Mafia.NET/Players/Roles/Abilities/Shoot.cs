using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum ShootKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetImmune,
        FirstNight
    }

    [RegisterAbility("Shoot", 5, typeof(ShootSetup))]
    public class Shoot : NightEndAbility<ShootSetup>
    {
        public AttackStrength Strength { get; set; } = AttackStrength.Base;

        public TargetNotification UserMessage()
        {
            return Match.Phase.Day == 1
                ? new TargetNotification
                {
                    UserAddMessage = target =>
                    {
                        Targets.ForceSet(null);
                        return Notification.Chat(Role, ShootKey.FirstNight);
                    }
                }
                : TargetNotification.Enum<ShootKey>(this);
        }

        public override void NightStart(in IList<IAbility> abilities)
        {
            var filter = TargetFilter.Living(Match).Except(User);
            var notification = UserMessage();
            SetupTargets(abilities, filter, notification);
        }

        public override bool Active()
        {
            return base.Active() && Uses > 0;
        }

        public override bool Use(IPlayer victim)
        {
            Uses--;

            var attack = Attack(Strength);
            if (attack.Use(victim)) return true;

            var notification = Notification.Chat(Role, ShootKey.TargetImmune);
            User.OnNotification(notification);

            return true;
        }
    }

    public class ShootSetup : IUsesSetup
    {
        public int Uses { get; set; } = 2;
    }
}