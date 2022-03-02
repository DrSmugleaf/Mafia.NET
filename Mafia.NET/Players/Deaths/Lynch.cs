using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Deaths
{
    public class Lynch : ILynch
    {
        public Lynch(
            int day,
            IPlayer victim,
            DeathCause cause,
            VerdictManager? verdicts = null)
        {
            Day = day;
            Victim = victim;
            VictimName = victim.Name;
            VictimRole = victim.Role.Name;
            Cause = cause;
            Killer = null;
            LastWill = victim.LastWill;
            DeathNote = null;
            Description = "";
            Strength = AttackStrength.Lynch;
            Direct = false;
            Stoppable = false;
            For = verdicts?.Voters(Verdict.Guilty) ?? new List<IPlayer>();
            Against = verdicts?.Voters(Verdict.Innocent) ?? new List<IPlayer>();
            Abstained = verdicts?.Voters(Verdict.Abstain) ?? new List<IPlayer>();
        }

        public int Day { get; set; }
        public IPlayer Victim { get; protected set; }
        public Text VictimName { get; set; }
        public Key? VictimRole { get; set; }
        public DeathCause Cause { get; set; }
        public IPlayer? Killer { get; set; }
        public string? LastWill { get; set; }
        public string? DeathNote { get; set; }
        public string Description { get; set; }
        public AttackStrength Strength { get; set; }
        public bool Direct { get; set; }
        public bool Stoppable { get; set; }
        public IList<IPlayer> For { get; }
        public IList<IPlayer> Against { get; }
        public IList<IPlayer> Abstained { get; }

        public void WithVictim(IPlayer player)
        {
            Victim = player;
            VictimName = player.Name;
            VictimRole = player.Role.Name;
            LastWill = player.LastWill;
        }
    }
}