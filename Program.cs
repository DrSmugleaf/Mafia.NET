using Mafia.NET.Player.Team;

namespace Mafia.NET
{
    class Program
    {
        static void Main(string[] args)
        {
            var teams = TeamInformation.LoadAll();
        }
    }
}
