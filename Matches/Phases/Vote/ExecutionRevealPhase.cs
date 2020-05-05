﻿using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using System;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class ExecutionRevealPhase : BasePhase
    {
        public IPlayer Player;

        public ExecutionRevealPhase(IMatch match, IPlayer player, int duration = 10) : base(match, "Execution Reveal", duration, new NightPhase(match))
        {
            Player = player;
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers.Values);
            var role = Notification.Popup($"{Player.Name}'s role was {Player.Role.Name}.");
            
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(role);
            }

            if (Player.LastWill.Length == 0) return;

            var lastWill = Notification.Chat($"{Player.Name} left us his last will:{Environment.NewLine}{Player.LastWill}");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(lastWill);
            }
        }

        public override IPhase End()
        {
            var notification = Notification.Popup("Let us reconvene tomorrow.");
            
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            return base.End();
        }
    }
}
