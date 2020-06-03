using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Triad
{
    public interface ITriadAbility
    {
    }

    public abstract class TriadAbility<T> : BaseAbility<T>, ITriadAbility where T : class, ITriadSetup, new()
    {
        public override void NightStart(in IList<IAbilityAction> actions)
        {
            var chat = new ChatAction<TriadChat>(this, TriadChat.Name);
            actions.Add(chat);
        }

        public override bool DetectableBy(ISheriffSetup setup = null)
        {
            return setup?.DetectsMafiaTriad == true;
        }

        public override Key GuiltyName()
        {
            return SheriffKey.Triad;
        }
    }

    public interface ITriadSetup : IAbilitySetup
    {
    }

    public class TriadChat : Chat
    {
        public static readonly string Name = "Triad";

        public TriadChat() : base(Name)
        {
        }
    }
}