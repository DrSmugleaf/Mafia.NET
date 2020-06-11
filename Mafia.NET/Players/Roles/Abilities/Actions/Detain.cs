using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum DetainKey
    {
        DayUserAddMessage,
        DayUserRemoveMessage,
        DayUserChangeMessage,
        UnableToJail,
        UserAddMessage,
        UserRemoveMessage,
        TargetAddMessage,
        TargetRemoveMessage,
        Nickname
    }

    public class Detain : NightStartAbility<DetainSetup>
    {
        public Detain()
        {
            Priority = -2;
        }

        public Release Release { get; set; }
        public Execute Execute { get; set; }

        public override void Initialize(AbilityEntry entry, IPlayer player)
        {
            if (Initialized) return;

            base.Initialize(entry, player);
            Release = Get<Release>();
            Execute = Get<Execute>();
            Execute.Detain = this;
        }

        public override void DayEnd(in IList<IAbility> abilities)
        {
            if (Match.Graveyard.LynchedToday())
            {
                Targets.ForceSet(null);
                User.OnNotification(Notification.Chat(Role, DetainKey.UnableToJail));
            }

            Targets[0]?.Perks.RoleBlock(User, true);
        }

        public override void NightEnd(in IList<IAbility> abilities)
        {
            abilities.Add(Release);
            abilities.Add(Execute); // TODO: Uses
        }

        public override bool TryUse(IPlayer target)
        {
            return !Match.Graveyard.LynchedToday() && base.TryUse();
        }

        public override bool ResolveUse()
        {
            Targets.TryDay(out var first);
            Targets.TryDay(1, out var second);

            if (first == null && second == null) return Use();
            if (first != null && second != null) return Use(first, second);
            if (first != null) return Use(first);
            return false;
        }

        public override bool Use(IPlayer prisoner)
        {
            User.Crimes.Add(CrimeKey.Kidnapping);

            var chatId = $"Jailor-{prisoner.Number}";
            var chat = new NightChat {ChatId = chatId};
            chat.FromParent(this);
            chat.Use();

            var jail = Match.Chat.Open(chatId);
            jail.Add(prisoner);

            prisoner.Perks.CurrentDefense = AttackStrength.Base;
            prisoner.Perks.RoleBlock(User, true);

            return true;
        }
    }

    public class Detain<T> : Detain where T : DetainSetup
    {
        public new T Setup { get; set; }

        public override void Initialize(AbilityEntry entry, IPlayer user)
        {
            if (Initialized) return;

            base.Initialize(entry, user);
            Setup = (T) base.Setup;
        }
    }

    public class DetainSetup : IUsesSetup
    {
        public int Uses { get; set; } = 1;
    }
}