using Mafia.NET.Players.Controllers;

namespace Mafia.NET.Web.Chat
{
    public class SessionPlayer
    {
        public SessionPlayer(string id)
        {
            Id = id;
        }

        public string Id { get; set; }

        public IController Controller(string name)
        {
            return new Controller(name);
        }
    }
}