using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Abilities
{
    public class Crimes
    {
        protected ISet<string> Set { get; }

        public Crimes()
        {
            Set = new HashSet<string>();
        }

        public void Add(string crime) => Set.Add(crime);

        public ISet<string> Get() => Set;

        public string AsString()
        {
            if (Set.Count == 0) return "No crime";
            return string.Join(", ", Set);
        }
    }
}
