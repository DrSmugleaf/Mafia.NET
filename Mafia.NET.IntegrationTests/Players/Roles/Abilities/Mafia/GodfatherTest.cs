using System.Collections;
using System.Collections.Generic;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Perks;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Mafia;

[TestFixture]
[TestOf(typeof(MafiaHeadSetup))]
public class GodfatherTest : BaseMatchTest
{
    [TestCaseSource(typeof(AllyCases))]
    public void Ally(string rolesString, bool killWithoutMafioso, bool godfatherTarget, bool allySuggest)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new MafiaHeadSetup
        {
            CanKillWithoutMafioso = killWithoutMafioso
        });
        match.Start();

        var godfather = match.AllPlayers[0];
        var mafioso = match.AllPlayers[1];
        var target = match.AllPlayers[2];

        match.Skip<NightPhase>();

        if (godfatherTarget) godfather.Target(target);
        if (allySuggest) mafioso.Target(target);

        match.Skip<DeathsPhase>();

        Deaths(match, godfatherTarget || allySuggest ? 1 : 0);
        Assert.That(godfather.Alive, Is.True);
        Assert.That(mafioso.Alive, Is.True);
        Assert.That(target.Alive, Is.EqualTo(!godfatherTarget && !allySuggest));
    }

    [TestCaseSource(typeof(AloneCases))]
    public void Alone(string rolesString, bool killWithoutMafioso, bool kill)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new MafiaHeadSetup
        {
            CanKillWithoutMafioso = killWithoutMafioso
        });
        match.Start();

        var godfather = match.AllPlayers[0];
        var target = match.AllPlayers[1];

        match.Skip<NightPhase>();

        if (kill) godfather.Target(target);

        match.Skip<DeathsPhase>();

        Deaths(match, killWithoutMafioso && kill ? 1 : 0);
        Assert.That(godfather.Alive, Is.True);
        Assert.That(target.Alive, Is.EqualTo(!killWithoutMafioso || !kill));
    }


    [TestCaseSource(typeof(GuardAllyCases))]
    public void GuardAlly(string rolesString, bool kill, bool suggest, bool guard, bool ignoresImmunity)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        match.AbilitySetups.Replace(new GuardSetup
        {
            IgnoresInvulnerability = ignoresImmunity
        });
        match.Start();

        var godfather = match.AllPlayers[0];
        var mafioso = match.AllPlayers[1];
        var bodyguard = match.AllPlayers[2];
        var target = match.AllPlayers[3];

        match.Skip<NightPhase>();

        if (kill) godfather.Target(target);
        if (suggest) mafioso.Target(target);
        if (guard) bodyguard.Target(target);

        match.Skip<DeathsPhase>();

        var attack = kill || suggest;
        var fight = guard && attack;
        Deaths(match, fight ? 2 : attack ? 1 : 0);

        Assert.That(godfather.Alive, Is.True);
        Assert.That(mafioso.Alive, Is.EqualTo(!fight));
        Assert.That(bodyguard.Alive, Is.EqualTo(!fight));
        Assert.That(target.Alive, Is.EqualTo(!attack || fight));
    }

    [TestCaseSource(typeof(GuardAloneCases))]
    public void GuardAlone(string rolesString, bool killWithoutMafioso, bool kill, bool guard, bool immune,
        bool ignoresImmunity)
    {
        var roleNames = rolesString.Split(",");
        var match = new Match(roleNames);
        if (!immune) match.Perks[roleNames[0]].Defense = AttackStrength.None;
        match.AbilitySetups.Replace(new MafiaHeadSetup
        {
            CanKillWithoutMafioso = killWithoutMafioso
        }, new GuardSetup
        {
            IgnoresInvulnerability = ignoresImmunity
        });
        match.Start();

        var godfather = match.AllPlayers[0];
        var bodyguard = match.AllPlayers[1];
        var target = match.AllPlayers[2];

        match.Skip<NightPhase>();

        if (kill) godfather.Target(target);
        if (guard) bodyguard.Target(target);

        match.Skip<DeathsPhase>();

        var fight = guard && kill && killWithoutMafioso;
        var gfDead = fight && (!immune || ignoresImmunity);
        var targetDead = kill && killWithoutMafioso && !guard;
        Deaths(match, gfDead ? 2 : targetDead || fight ? 1 : 0);

        Assert.That(godfather.Alive, Is.EqualTo(!gfDead));
        Assert.That(bodyguard.Alive, Is.EqualTo(!fight));
        Assert.That(target.Alive, Is.EqualTo(!targetDead));
    }
}

public class AllyCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var combinations = new Dictionary<string, string>
        {
            ["Godfather"] = "Mafioso",
            ["Dragon Head"] = "Enforcer"
        };

        foreach (var combination in combinations)
        {
            var head = combination.Key;
            var suggester = combination.Value;
            var roleNames = $"{head},{suggester},Citizen";

            foreach (var killWithoutMafioso in new[] {true, false})
            foreach (var godfatherTarget in new[] {true, false})
            foreach (var allySuggest in new[] {true, false})
                yield return new object[] {roleNames, killWithoutMafioso, godfatherTarget, allySuggest};
        }
    }
}

public class AloneCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var godfathers = new[] {"Godfather", "Dragon Head"};

        foreach (var godfather in godfathers)
        {
            var roleNames = $"{godfather},Citizen,Citizen";

            foreach (var killWithoutMafioso in new[] {true, false})
            foreach (var kill in new[] {true, false})
                yield return new object[] {roleNames, killWithoutMafioso, kill};
        }
    }
}

public class GuardAllyCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var combinations = new Dictionary<string, string>
        {
            ["Godfather"] = "Mafioso",
            ["Dragon Head"] = "Enforcer"
        };

        foreach (var combination in combinations)
        {
            var head = combination.Key;
            var suggester = combination.Value;
            var roleNames = $"{head},{suggester},Bodyguard,Citizen";

            foreach (var kill in new[] {true, false})
            foreach (var guard in new[] {true, false})
            foreach (var immune in new[] {true, false})
            foreach (var ignoresImmunity in new[] {true, false})
                yield return new object[] {roleNames, kill, guard, immune, ignoresImmunity};
        }
    }
}

public class GuardAloneCases : IEnumerable
{
    public IEnumerator GetEnumerator()
    {
        var godfathers = new[] {"Godfather", "Dragon Head"};

        foreach (var godfather in godfathers)
        {
            var roleNames = $"{godfather},Bodyguard,Citizen";

            foreach (var killWithoutMafioso in new[] {true, false})
            foreach (var kill in new[] {true, false})
            foreach (var suggest in new[] {true, false})
            foreach (var guard in new[] {true, false})
            foreach (var ignoresImmunity in new[] {true, false})
                yield return new object[] {roleNames, killWithoutMafioso, kill, suggest, guard, ignoresImmunity};
        }
    }
}