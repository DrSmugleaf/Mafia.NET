using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Players.Roles.Abilities;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Neutral;
using Mafia.NET.Players.Roles.Abilities.Triad;

namespace Mafia.NET.Players.Roles
{
    [RegisterKey]
    public enum CrimeKey
    {
        Trespassing,
        Kidnapping,
        Corruption,
        IdentityTheft,
        Soliciting,
        Murder,
        DisturbingThePeace,
        Conspiracy,
        DestructionOfProperty,
        Arson
    }

    public class Crimes
    {
        public static readonly IImmutableSet<Key> All = Key.Enum<CrimeKey>().ToImmutableHashSet();
        public static readonly Key NoCrime = new Key("CrimeNoCrime");

        public IPlayer Player;

        public Crimes(IPlayer player)
        {
            Player = player;
            Committed = new HashSet<Key>();
        }

        protected ISet<Key> Committed { get; }
        [CanBeNull] public Framing Framing { get; set; }

        public void AddKey(Key crime)
        {
            Committed.Add(crime);
        }
        
        public void Add(CrimeKey crime)
        {
            AddKey(crime);
        }

        public Key Crime()
        {
            if (Framing != null) return Framing.Crime;
            if (Committed.Count == 0 || Player.Role.Ability.DetectionImmune) return NoCrime;
            return Committed.ElementAt(Player.Match.Random.Next(Committed.Count));
        }

        public IReadOnlyList<Key> AllCommitted()
        {
            return new List<Key>(Committed).AsReadOnly();
        }

        public Key RoleName()
        {
            if (Framing != null) return Framing.RoleName;
            if (Player.Role.Ability.DetectionImmune) return new Key("CitizenName"); // TODO: Check
            return Player.Role.Name;
        }

        public Key Innocence(ISheriffSetup setup)
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

        public Key Crime { get; }
        public Key RoleName { get; }
    }
}