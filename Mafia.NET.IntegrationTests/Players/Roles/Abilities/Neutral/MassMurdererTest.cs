using Mafia.Net.IntegrationTests.Matches;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Phases;
using Mafia.NET.Players.Roles.Abilities;
using NUnit.Framework;
using NUnit.Framework.Constraints;

namespace Mafia.Net.IntegrationTests.Players.Roles.Abilities.Neutral
{
    [TestFixture]
    [TestOf(typeof(KillingSpree))]
    public class MassMurdererTest : BaseMatchTest
    {
        // TODO: Stopped by Bodyguard
        [TestCase("Mass Murderer,Investigator,Investigator,Investigator,Investigator", true)]
        [TestCase("Mass Murderer,Investigator,Investigator,Investigator,Investigator", false)]
        public void SpreeLeavesHouse(string rolesString, bool spree)
        {
            var roleNames = rolesString.Split(",");
            var match = new Match(roleNames);
            match.Start();

            var massMurderer = match.AllPlayers[0];
            var ability = massMurderer.Abilities.Get<KillingSpree>()!;
            Assert.NotNull(ability);

            var target = match.AllPlayers[1];
            var other = match.AllPlayers[2];

            match.Skip<NightPhase>();

            Assert.Zero(ability.Cooldown);

            foreach (var player in match.LivingPlayers)
            {
                if (player == massMurderer && !spree) continue;
                player.Target(target);
            }

            target.Target(other);

            match.Skip<DeathsPhase>();

            Assert.That(ability.Cooldown, spree
                ? (IResolveConstraint) Is.Positive
                : Is.Zero);
            Assert.True(massMurderer.Alive);
            Assert.True(target.Alive);
            Deaths(match, spree ? roleNames.Length - 2 : 0);

            var cooldown = ability.Cooldown;

            match.Skip<NightPhase>();

            Assert.That(ability.Cooldown, spree
                ? (IResolveConstraint) Is.Positive
                : Is.Zero);

            foreach (var player in match.LivingPlayers)
            {
                if (player == massMurderer && !spree) continue;
                player.Target(target);
            }

            target.Target(other);

            match.Skip<DeathsPhase>();

            Assert.That(ability.Cooldown, Is.EqualTo(spree
                ? cooldown - 1
                : cooldown));
            Assert.True(massMurderer.Alive);
            Assert.True(target.Alive);
            Deaths(match, spree ? roleNames.Length - 2 : 0);
        }
    }
}