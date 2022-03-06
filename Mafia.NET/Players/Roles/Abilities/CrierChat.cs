using Mafia.NET.Localization;
using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Roles.Abilities.Actions;
using Mafia.NET.Players.Roles.Abilities.Registry;

namespace Mafia.NET.Players.Roles.Abilities;

[RegisterAbility("Crier Chat", -1)]
public class CrierChatAbility : NightChat<CrierChat>
{
    public override string ChatId => "Crier";
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
            if (player.Abilities.Any<CrierChatAbility>())
            {
                Mute(player, false);
                Participants[player].Nickname = new Key(player.Role, ChatKey.Nickname);
            }
            else
            {
                Mute(player);
            }

        Initialized = true;
    }
}