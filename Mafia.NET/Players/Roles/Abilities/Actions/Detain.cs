using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Roles.Abilities.Actions;

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

public class Detain : NightStartAbility
{
    public Detain()
    {
        Priority = -2;
    }

    public Release Release { get; set; } = null!;
    public Execute Execute { get; set; } = null!;

    public override void Initialize(AbilitySetupEntry setup, IPlayer user)
    {
        if (Initialized) return;

        base.Initialize(setup, user);

        Release = Get<Release>();

        Execute = Get<Execute>();
        Execute.Detain = this;
        Execute.HasUses = HasUses;
        Execute.Uses = Uses;
    }

    public override void DayEnd(in IList<IAbility> abilities)
    {
        if (Match.Graveyard.AnyLynchesToday())
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
        return !Match.Graveyard.AnyLynchesToday() && base.TryUse();
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

public class Detain<T> : Detain where T : IAbilitySetup
{
    public new T Setup { get; set; } = default!;

    public override void Initialize(AbilitySetupEntry setup, IPlayer user)
    {
        if (Initialized) return;

        base.Initialize(setup, user);
        Setup = (T) base.Setup;
    }
}