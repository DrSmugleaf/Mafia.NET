using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Neutral;
using Mafia.NET.Players.Roles.Abilities.Triad;

namespace Mafia.NET.Players.Roles
{
    public class Crimes
    {
        public static readonly IImmutableSet<string> All = ImmutableHashSet.Create(
            "Trespassing",
            "Kidnapping",
            "Corruption",
            "Identity theft",
            "Soliciting",
            "Murder",
            "Disturbing the peace",
            "Conspiracy",
            "Destruction of property",
            "Arson");

        public IPlayer Player;

        public Crimes(IPlayer player)
        {
            Player = player;
            Committed = new HashSet<string>();
        }

        protected ISet<string> Committed { get; }
        [CanBeNull] public Framing Framing { get; set; }

        public void Add(string crime)
        {
            Committed.Add(crime);
        }

        public string Crime() // TODO: Localize
        {
            if (Framing != null) return Framing.Crime;
            if (Committed.Count == 0 || Player.Role.Ability.DetectionImmune) return "No crime.";
            return Committed.ElementAt(Player.Match.Random.Next(Committed.Count));
        }

        public string RoleName()
        {
            if (Framing != null) return Framing.RoleName;
            if (Player.Role.Ability.DetectionImmune) return "Citizen";
            return Player.Role.Name;
        }

        public string Innocence(ISheriffSetup setup)
        {
            if (Framing != null) return Framing.RoleName;
            return Player.Role.Ability.Guilty(setup);
        }
    }

    public class Framing
    {
        public Framing(IMatch match)
        {
            Crime = Crimes.All.ElementAt(match.Random.Next(Crimes.All.Count));

            var roles = match.LivingPlayers
                .Select(player => player.Role)
                .Where(role =>
                    role.Ability is IMafiaAbility || role.Ability is ITriadAbility ||
                    role.IsCategory("Neutral Killing") || role.Ability is Cultist || role.Ability is WitchDoctor)
                .Select(role => role.Name)
                .ToList();

            RoleName = roles.ElementAt(match.Random.Next(roles.Count()));
        }

        public string Crime { get; }
        public string RoleName { get; }
    }
}