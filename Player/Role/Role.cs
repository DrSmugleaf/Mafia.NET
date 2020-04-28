namespace Mafia.NET.Player.Role
{
    class Role : IRole
    {
        public string Name { get; set; }

        public Role(string name)
        {
            Name = name;
        }
    }
}
