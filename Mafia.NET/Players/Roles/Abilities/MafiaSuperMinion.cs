using System.Linq;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Mafia Super Minion", -3, typeof(MafiaSuperMinionSetup))]
    public class MafiaSuperMinion : DayStartAbility<MafiaSuperMinionSetup>
    {
        public IRole SuperRole { get; set; }

        public bool NoGodfather()
        {
            return !Match.LivingPlayers.Any(player =>
                player.Role.Team == User.Role.Team &&
                player.Role.Abilities.All.Any(ability => ability is MafiaHead));
        }

        public override bool TryUse()
        {
            if (Setup.ReplacesGodfather && NoGodfather())
            {
                var transform = Get<Transform>();
                var entry = Role.Team.Id == "Triad"
                    ? Match.RoleSetup.Roles.Ids["Dragon Head"]
                    : Match.RoleSetup.Roles.Ids["Godfather"]; // TODO
                transform.NewRole = new Role(entry);
                transform.Use();
                return true;
            }

            return false;
        }
    }

    public class MafiaSuperMinionSetup : IAbilitySetup
    {
        public bool ReplacesGodfather = false;
    }
}