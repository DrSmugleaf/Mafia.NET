using System.Linq;
using Mafia.NET.Matches;

namespace Mafia.NET.Players.Roles.HealProfiles
{
    public interface IHealProfile
    {
        IPlayer User { get; set; }
        IMatch Match { get; }

        bool HealedBy(IPlayer healer);
    }

    [RegisterHeal("Heal")]
    public class HealProfile : IHealProfile
    {
        public HealProfile(IPlayer user)
        {
            User = user;
        }

        public IPlayer User { get; set; }
        public IMatch Match => User.Match;

        public virtual bool HealedBy(IPlayer healer)
        {
            var threats = Match.Graveyard.ThreatsOn(User)
                .Where(death => death.Stoppable)
                .ToList();

            if (threats.Count > 0)
            {
                var threat = threats[0];
                Match.Graveyard.Threats.Remove(threat);

                return true;
            }

            return false;
        }
    }
}