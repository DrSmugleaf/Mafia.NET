using System.Collections;
using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Perks;
using NUnit.Framework;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral
{
    [TestFixture]
    [TestOf(typeof(Control))]
    public class WitchTest : BaseMatchTest
    {
        [TestCaseSource(typeof(ControlCases))]
        public void Control(string rolesString, bool controlFirst, bool controlSecond, bool attack, bool causesSelf, bool knows)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.AbilitySetups.Replace(new ControlSetup
            {
                CanCauseSelfTargets = causesSelf,
                VictimKnows = knows
            });
            match.Start();

            var witch = match.AllPlayers[0];
            var killer = match.AllPlayers[1];
            var originalTarget = match.AllPlayers[2];
            var newTarget = match.AllPlayers[3];

            match.Skip<NightPhase>();

            if (controlFirst) witch.Targets[0] = killer;
            if (controlSecond) witch.Targets[1] = newTarget;
            if (attack) killer.Target(originalTarget);

            match.Skip<DeathsPhase>();

            var control = controlFirst && controlSecond;
            Assert.True(witch.Alive);
            Assert.True(killer.Alive);
            Assert.That(originalTarget.Alive, Is.EqualTo(control || !attack));
            Assert.That(newTarget.Alive, Is.EqualTo(!control));
            
            Deaths(match, control || attack ? 1 : 0);
        }
        
        [TestCaseSource(typeof(SelfTargetCases))]
        public void SelfTarget(string rolesString, bool control, bool attack, bool causesSelf, bool knows)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Perks["Serial Killer"].Defense = AttackStrength.None;
            match.AbilitySetups.Replace(new ControlSetup
            {
                CanCauseSelfTargets = causesSelf,
                VictimKnows = knows
            });
            match.Start();

            var witch = match.AllPlayers[0];
            var killer = match.AllPlayers[1];

            match.Skip<NightPhase>();

            if (control)
            {
                witch.Targets[0] = killer;
                witch.Targets[1] = killer;
            }
            if (attack) killer.Target(witch);

            match.Skip<DeathsPhase>();

            Assert.That(witch.Alive, Is.EqualTo(control && causesSelf || !attack));
            Assert.That(killer.Alive, Is.EqualTo(!control || !causesSelf));
            
            Deaths(match, control && causesSelf || attack ? 1 : 0);
        }
    }
    
    public class ControlCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roleNames = "Witch,Serial Killer,Citizen,Citizen";

            foreach (var controlFirst in new[] {true, false})
            foreach (var controlSecond in new[] {true, false})
            foreach (var attack in new[] {true, false})
            foreach (var causesSelf in new[] {true, false})
            foreach (var knows in new[] {true, false})
                yield return new object[] {roleNames, controlFirst, controlSecond, attack, causesSelf, knows};
        }
    }
    
    public class SelfTargetCases : IEnumerable
    {
        public IEnumerator GetEnumerator()
        {
            var roleNames = "Witch,Serial Killer,Citizen";

            foreach (var control in new[] {true, false})
            foreach (var attack in new[] {true, false})
            foreach (var causesSelf in new[] {true, false})
            foreach (var knows in new[] {true, false})
                yield return new object[] {roleNames, control, attack, causesSelf, knows};
        }
    }
}