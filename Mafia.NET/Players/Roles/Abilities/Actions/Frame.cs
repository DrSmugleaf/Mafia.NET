namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public class Frame : AbilityAction
    {
        public Frame(IAbility ability,
            int priority = 4,
            bool direct = true,
            bool stoppable = true) : base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);
            target.Crimes.Framing = new Framing(Match);
            return true;
        }
    }
}