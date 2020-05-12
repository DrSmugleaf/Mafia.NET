namespace Mafia.NET.Web.Models
{
    public class CreateGameViewModel
    {
        public string Name { get; set; }
        public string Game { get; set; }

        public bool IsValid()
        {
            return Name.Length < 31 && Name.Length > 2 && Game.Length > 0;
        }
    }
}