using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum CrierKey
    {
        MayTalk,
        Nickname
    }

    [RegisterAbility("Crier", typeof(CrierSetup))]
    public class Crier : TownAbility<CrierSetup>
    {
        public override void NightStart(in IList<IAbilityAction> actions)
        {
            var chat = new ChatAction<CrierChat>(this, "Crier");
            actions.Add(chat);
        }
    }

    public class CrierSetup : ITownSetup, IRandomExcluded
    {
        public bool ExcludedFromRandoms { get; set; } = false;
    }

    public interface ICrierChatter : IAbility
    {
    }

    public class CrierChat : Chat
    {
        public CrierChat() : base("Crier")
        {
        }

        public override void Initialize(IMatch match)
        {
            if (Initialized) return;

            foreach (var player in match.AllPlayers)
                if (player.Role.Ability is ICrierChatter)
                    Mute(player);
                else Mute(player, false);

            Initialized = true;
        }
    }
}