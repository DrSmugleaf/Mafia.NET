using System.Linq;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Mafia Super Minion", -3, typeof(MafiaSuperMinionSetup))]
    public class MafiaSuperMinion : DayStartAbility<MafiaSuperMinionSetup>
    {
        public IRole? HeadRole { get; set; }

        public bool NoHead()
        {
            return !Match.LivingPlayers.Any(player =>
                player.Role.Team == User.Role.Team &&
                player.Role.Abilities.All.Any(ability => ability is MafiaHead));
        }

        public override bool TryUse()
        {
            if (Setup.ReplacesHead && NoHead())
            {
                var transform = Get<Transform>();
                var entry = Role.Team.Id == "Mafia"
                    ? Match.RoleSetup.Roles.Ids["Godfather"]
                    : Match.RoleSetup.Roles.Ids["Dragon Head"]; // TODO
                transform.NewRole = new Role(entry);
                transform.Use();
                return true;
            }

            return false;
        }
    }

    [RegisterSetup]
    public class MafiaSuperMinionSetup : IAbilitySetup
    {
        public bool ReplacesHead = false;
    }
}