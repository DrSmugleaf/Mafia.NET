using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches
{
    public class Graveyard
    {
        public IMatch Match { get; }
        public List<IDeath> PublicDeaths { get; }
        public List<IDeath> UndisclosedDeaths { get; }
        public List<IDeath> Threats { get; }

        public Graveyard(IMatch match)
        {
            Match = match;
            PublicDeaths = new List<IDeath>();
            UndisclosedDeaths = new List<IDeath>();
        }

        public IReadOnlyList<IDeath> All() => PublicDeaths.Union(UndisclosedDeaths).ToList();

        public void Disclose()
        {
            PublicDeaths.AddRange(UndisclosedDeaths);
            UndisclosedDeaths.Clear();
        }

        public void SettleThreats()
        {
            var victims = new Dictionary<IPlayer, IDeath>();

            foreach (var threat in Threats)
            {
                var victim = threat.Victim;

                if (victim.Role.Ability.CurrentlyDeathImmune && !threat.PiercesImmunity) continue;

                if (victims.TryGetValue(victim, out var old))
                {
                    victims[victim] = new Death(old, threat.Description);
                }
                else
                {
                    threat.Victim.Alive = false;
                    victims[victim] = threat;
                }
            }

            Threats.Clear();
            UndisclosedDeaths.AddRange(victims.Values);
        }

        public bool DiedOn(int day, DeathCause cause)
        {
            return All().Any(death => death.Day == day && death.Cause == cause);
        }

        public bool DiedToday(DeathCause cause) => DiedOn(Match.Phase.Day, cause);

        public bool LynchedToday() => DiedToday(DeathCause.LYNCH);
    }
}
