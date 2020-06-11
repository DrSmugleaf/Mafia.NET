using System.Linq;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterAbility("Mafia Minion", -3, typeof(MafiaMinionSetup))]
    public class MafiaMinion : DayStartAbility<MafiaMinionSetup>
    {
        // TODO: Disguiser
        public bool AloneOnTeam()
        {
            return Match.LivingPlayers
                .Count(player => player.Role.Team == User.Role.Team) == 1;
        }

        public override bool ResolveUse()
        {
            if (Setup.BecomesHenchmanIfAlone && AloneOnTeam())
            {
                var transform = new Transform();
                transform.FromParent(this);
                var entry = Role.Team.Id == "Triad"
                    ? Match.RoleSetup.Roles.Ids["Enforcer"]
                    : Match.RoleSetup.Roles.Ids["Mafioso"]; // TODO
                transform.NewRole = new Role(entry);
                transform.Use();
                return true;
            }

            return false;
        }
    }

    public class MafiaMinionSetup : IAbilitySetup
    {
        public bool BecomesHenchmanIfAlone = true;
    }
}