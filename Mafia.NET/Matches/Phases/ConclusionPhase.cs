﻿using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;

namespace Mafia.NET.Matches.Phases
{
    public class ConclusionPhase : BasePhase
    {
        public IList<IVictory> Victories;

        public ConclusionPhase(IList<IVictory> victories, IMatch match, uint duration = 120) : base(match, "Conclusion",
            duration, actionable: false)
        {
            Victories = victories;
        }

        public override IPhase NextPhase()
        {
            return this;
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers);
            ChatManager.UnMuteUnDeafen();

            var conclusion = Notification.Popup(DayKey.Conclusion);
            var roles = new EntryBundle();
            foreach (var player in Match.AllPlayers)
                roles.Chat(DayKey.ConclusionRoleReveal, player, player.Role);

            foreach (var player in Match.AllPlayers)
            {
                foreach (var victory in Victories)
                {
                    player.OnNotification(conclusion);
                    player.OnNotification(victory.Popup);
                    player.OnNotification(victory.WinnersList);
                }

                player.OnNotification(roles);
            }

            base.Start();
        }

        public override void End()
        {
            base.End();
            Match.End();
        }
    }
}