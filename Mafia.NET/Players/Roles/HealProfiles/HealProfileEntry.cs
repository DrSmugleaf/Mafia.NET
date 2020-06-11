using System;
using System.Linq;
using Mafia.NET.Registries;

namespace Mafia.NET.Players.Roles.HealProfiles
{
    public class HealProfileEntry : IRegistrable
    {
        public HealProfileEntry(string id, Type profile)
        {
            Id = id;
            Profile = profile;

            if (Profile.GetInterfaces().All(i => i != typeof(IHealProfile)))
                throw new ArgumentException($"Profile {profile} doesn't implement {nameof(IHealProfile)}");
        }

        public Type Profile { get; }

        public string Id { get; }

        public IHealProfile Build(IPlayer user)
        {
            return (IHealProfile) Activator.CreateInstance(Profile, user);
        }
    }
}