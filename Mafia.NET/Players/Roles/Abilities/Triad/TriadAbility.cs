using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Triad
{
    public interface ITriadAbility
    {
    }

    public abstract class TriadAbility<T> : BaseAbility<T>, ITriadAbility where T : ITriadSetup, new()
    {
        public static readonly string NightChatId = "Triad";

        public override void Chat()
        {
            Match.Chat.Open(NightChatId, User);
        }

        public override bool DetectableBy(ISheriffSetup setup)
        {
            return setup.DetectsMafiaTriad;
        }

        protected override Key GuiltyName()
        {
            return SheriffKey.Triad;
        }
    }

    public interface ITriadSetup : IAbilitySetup
    {
    }
}