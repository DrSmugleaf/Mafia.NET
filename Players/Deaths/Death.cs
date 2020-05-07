using Mafia.NET.Players.Roles.Abilities;
using System;

#nullable enable

namespace Mafia.NET.Players.Deaths
{
    public class Death : IDeath
    {
        public int Day { get; set; }
        public IPlayer Victim { get; set; }
        public DeathCause Cause { get; set; }
        public IPlayer? Killer { get; set; }
        public Note LastWill { get; set; }
        public Note? DeathNote { get; set; }
        public string Description { get; set; }

        public Death(int day, IPlayer victim, DeathCause cause, string description, IPlayer? killer = null)
        {
            Day = day;
            Victim = victim;
            Cause = cause;
            Killer = killer;
            LastWill = victim.LastWill;
            DeathNote = killer?.DeathNote;
            Description = description;
        }

        public Death(IAbility ability, IPlayer victim)
        {
            Day = ability.Match.PhaseManager.Day;
            Victim = victim;
            Cause = DeathCause.MURDER;
            Killer = ability.User;
            LastWill = victim.LastWill;
            DeathNote = Killer.DeathNote;
            Description = ability.MurderDescriptions.Get();
        }

        public Death(IDeath death, string description)
        {
            Day = death.Day;
            Victim = death.Victim;
            Cause = death.Cause;
            Killer = death.Killer;
            LastWill = death.LastWill;
            DeathNote = death.DeathNote;
            Description = death.Description + Environment.NewLine + description;
        }
    }
}
