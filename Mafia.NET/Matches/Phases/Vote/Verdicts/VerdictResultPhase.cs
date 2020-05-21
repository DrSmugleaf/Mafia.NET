﻿using Mafia.NET.Localization;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictResultPhase : BasePhase
    {
        public VerdictResultPhase(IMatch match, VerdictManager verdicts, uint duration = 10) : base(match,
            "Vote Recount", duration)
        {
            Verdicts = verdicts;
        }

        private VerdictManager Verdicts { get; }

        public override IPhase NextPhase()
        {
            return Verdicts.Innocent() ? Supersedes : new LastWordsPhase(Match, Verdicts.Player);
        }

        public override void Start()
        {
            var trialOver = Entry.Popup(DayKey.TrialOver);
            var decision = Verdicts.Decision();
            var messages = Verdicts.Votes();

            foreach (var player in Match.AllPlayers)
            {
                player.OnNotification(trialOver);
                player.OnNotification(decision);
                player.OnNotification(messages);
            }

            base.Start();
        }
    }
}