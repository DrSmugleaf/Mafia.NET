using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Perks;

namespace Mafia.NET.Players.Deaths
{
    public class Death : IDeath
    {
        // TODO: Abilities limited by threats and death
        public Death(
            int day,
            IPlayer victim,
            DeathCause cause,
            string description,
            IPlayer? killer = null,
            AttackStrength strength = AttackStrength.Base,
            bool direct = true,
            bool stoppable = true)
        {
            Day = day;
            Victim = victim;
            VictimName = victim.Name;
            VictimRole = victim.Role.Name;
            Cause = cause;
            Killer = killer;
            LastWill = victim.LastWill;
            DeathNote = killer?.DeathNote;
            Description = description;
            Strength = strength;
            Direct = direct;
            Stoppable = stoppable;
        }

        public Death(
            IAbility ability,
            IPlayer victim,
            AttackStrength strength = AttackStrength.Base,
            bool direct = true,
            bool stoppable = true) :
            this(
                victim.Match.Phase.Day,
                victim,
                DeathCause.Murder,
                ability.MurderDescriptions.Get(victim.Match.Random),
                ability.User,
                strength,
                direct,
                stoppable)
        {
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

        public void WithVictim(IPlayer player)
        {
            Victim = player;
            VictimName = player.Name;
            VictimRole = player.Role.Name;
            LastWill = player.LastWill;
        }

        public override string ToString()
        {
            return VictimName.ToString();
        }
    }
}