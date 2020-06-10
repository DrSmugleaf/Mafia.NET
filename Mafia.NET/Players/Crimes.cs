using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using JetBrains.Annotations;
using Mafia.NET.Extension;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Roles;
using Mafia.NET.Players.Roles.Abilities;

namespace Mafia.NET.Players
{
    [RegisterKey]
    public enum CrimeKey
    {
        NotGuilty,
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
        public static readonly IImmutableSet<Key> All = Key.Enum<CrimeKey>()
            .Where(key => !Equals(key, new Key(CrimeKey.NotGuilty)))
            .ToImmutableHashSet();

        public Crimes(IPlayer user)
        {
            User = user;
            Committed = new HashSet<Key>();
        }

        public IPlayer User { get; set; }
        protected ISet<Key> Committed { get; }
        [CanBeNull] public Framing Framing { get; set; }

        public void Add(Key crime)
        {
            Committed.Add(crime);
        }

        public bool Innocent()
        {
            return Committed.Count == 0 || User.Perks.CurrentlyDetectionImmune;
        }

        public Notification Crime(IRole role, Enum key)
        {
            if (Framing != null)
                return Notification.Chat(role, key, User, Framing.Crime);
            if (Innocent()) return Notification.Chat(CrimeKey.NotGuilty, User);

            var randomIndex = User.Match.Random.Next(Committed.Count);
            var crime = Committed.ElementAt(randomIndex);

            return Notification.Chat(role, key, User, crime);
        }

        public IReadOnlyList<Key> AllCommitted()
        {
            return new List<Key>(Committed).AsReadOnly();
        }

        public Key RoleName()
        {
            if (Framing != null) return Framing.Sheriff;
            if (User.Perks.CurrentlyDetectionImmune) return new Key("CitizenName");
            return User.Role.Name;
        }

        public Key Sheriff(SheriffSetup setup)
        {
            if (Framing != null) return Framing.Sheriff;
            return User.Role.DetectionProfile.ResolveKey(setup);
        }

        public void OnDayStart()
        {
            Framing = null;
        }
    }

    public class Framing
    {
        public Framing(IMatch match)
        {
            Crime = Crimes.All.Random(match.Random);
            Sheriff = ResolveSheriff(match);
        }

        public Key Crime { get; }
        public Key Sheriff { get; }

        public static Key ResolveSheriff(IMatch match)
        {
            return match.AllPlayers
                .Where(player =>
                {
                    var role = player.Role;
                    return role.Team.Id == "Mafia" ||
                           role.Team.Id == "Triad" ||
                           role.IsCategory("Neutral Killing") ||
                           role.Id == "Cultist" ||
                           role.Id == "Witch Doctor";
                })
                .OrderByDescending(player => player.Alive)
                .Select(player => player.Role.DetectionProfile.GuiltyKey())
                .DefaultIfEmpty(SheriffKey.Mafia)
                .Random(match.Random);
        }
    }
}