namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Mafioso", typeof(MafiosoSetup))]
    public class Mafioso : MafiaAbility<MafiosoSetup>
    {
    }

    public class MafiosoSetup : IMafiaSetup
    {

    }
}
