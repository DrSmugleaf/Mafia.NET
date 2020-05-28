using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Neutral;
using Mafia.NET.Players.Roles.Abilities.Town;
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
        public static readonly Key NotGuilty = new Key("CrimeNotGuilty");

        public IPlayer User;

        public Crimes(IPlayer user)
        {
            User = user;
            Committed = new HashSet<Key>();
        }

        protected ISet<Key> Committed { get; }
        [CanBeNull] public Framing Framing { get; set; }

        public void Add(Key crime)
        {
            Committed.Add(crime);
        }

        public Notification Crime(Enum key)
        {
            if (Framing != null) return Notification.Chat(key, User, Framing.Crime);

            if (Committed.Count == 0 || User.Role.Ability.DetectionImmune)
                return Notification.Chat(NotGuilty, User);

            var crime = Committed
                .ElementAt(User.Match.Random.Next(Committed.Count));

            return Notification.Chat(key, User, crime);
        }

        public IReadOnlyList<Key> AllCommitted()
        {
            return new List<Key>(Committed).AsReadOnly();
        }

        public Key RoleName()
        {
            if (Framing != null) return Framing.RoleName;
            if (User.Role.Ability.DetectionImmune) return new Key("CitizenName");
            return User.Role.Name;
        }

        public Key Sheriff(ISheriffSetup setup)
        {
            if (Framing != null) return Framing.RoleName;
            return User.Role.Ability.DirectSheriff(setup);
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
                    role.Ability is IMafiaAbility ||
                    role.Ability is ITriadAbility ||
                    role.IsCategory("Neutral Killing") ||
                    role.Ability is Cultist ||
                    role.Ability is WitchDoctor)
                .Select(role => role.Name)
                .ToList();

            RoleName = roles.ElementAt(match.Random.Next(roles.Count()));
        }

        public Key Crime { get; }
        public Key RoleName { get; }
    }
}