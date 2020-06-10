namespace Mafia.NET.Players.Roles.HealProfiles
{
    [RegisterHeal("No Heal")]
    public class NoHealProfile : HealProfile
    {
        public NoHealProfile(IPlayer user) : base(user)
        {
        }

        public override bool HealedBy(IPlayer healer)
        {
            return false;
        }
    }
}