using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;
using Mafia.NET.Players.Roles.Perks;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterKey]
public enum CultSuggestKey
{
    UserAddMessage,
    UserRemoveMessage,
    UserChangeMessage,
    TeamAddMessage,
    TeamRemoveMessage,
    TeamChangeMessage
}

[RegisterAbility("Cult Suggest", 13, typeof(CultSuggestSetup))]
public class CultSuggest : NightEndAbility<ICultSuggestSetup>
{
    public bool IsMafia(IPlayer player)
    {
        var team = player.Role.Team.Id;
        return team == "Mafia" || team == "Triad"; // TODO
    }

    public int CultistsAllowed()
    {
        return Match.AllPlayers.Count(player => !IsMafia(player)) / 3;
    }

    public List<IPlayer> CultistsAlive()
    {
        return Match.LivingPlayers
            .Where(IsTeammate)
            .OrderBy(player => player.Number)
            .ToList();
    }

    public int ConversionsAllowed()
    {
        return CultistsAllowed() - CultistsAlive().Count;
    }

    public bool CanConvert()
    {
        return ConversionsAllowed() > 0;
    }

    public bool Vulnerable(IPlayer player)
    {
        return !Setup.ImmunityPreventsConversion ||
               player.Perks.CurrentDefense == AttackStrength.None;
    }

    public override bool Active()
    {
        return base.Active() && CanConvert() && Cooldown == 0;
    }

    public override void NightStart(in IList<IAbility> abilities)
    {
        var filter = TargetFilter.Living(Match).Where(player => !IsTeammate(player));
        SetupTargets<CultSuggestKey>(abilities, filter);
    }

    public override bool IsTeammate(IPlayer player)
    {
        var role = player.Role.Id;
        return role == "Cultist" || role == "Witch Doctor"; // TODO
    }

    public override bool Use(IPlayer target)
    {
        if (Cooldown > 0 || !Vulnerable(target) || !CanConvert())
            return false;

        var cultists = CultistsAlive();
        if (cultists.Count == 0 || cultists[0] != User)
            return false;

        var newRole = Match.Roles["Cultist"].Build();
        target.ChangeRole(newRole);

        foreach (var cultist in CultistsAlive())
        {
            var suggest = cultist.Abilities.Get<CultSuggest>();
            if (suggest != null) suggest.Cooldown = Setup.NightsBetweenUses;
        }

        return true;
    }
}

public interface ICultSuggestSetup : ICooldownSetup
{
    bool ImmunityPreventsConversion { get; set; }
}

[RegisterSetup]
public class CultSuggestSetup : ICultSuggestSetup
{
    public int NightsBetweenUses { get; set; } = 1;
    public bool ImmunityPreventsConversion { get; set; } = true;
}