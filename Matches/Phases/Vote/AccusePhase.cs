using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
using Mafia.NET.Players.Votes;
using System;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases.Vote
{
    class AccusePhase : BasePhase
    {
        IPhase Procedure { get; }
        event EventHandler<ProcedureStartEventArgs> ProcedureStart;

        public AccusePhase(IMatch match, int duration = 80) : base(match, "Time Left", duration, new NightPhase(match))
        {
            Procedure = match.Settings.Procedure;
        }

        public IList<IPlayer> VotesAgainst(IPlayer accused)
        {
            IList<IPlayer> votes = new List<IPlayer>();

            foreach (var living in Match.LivingPlayers.Values)
            {
                if (living.Accuses == accused)
                {
                    votes.Add(living);
                }
            }

            return votes;
        }

        protected void Accused(object sender, AccuseEventArgs e)
        {
            NotificationEventArgs notification;
            if (!e.Voter.Anonymous)
            {
                notification = NotificationEventArgs.Chat($"{e.Voter.Name} has voted to try {e.Accused.Name}.");
            } else
            {
                notification = NotificationEventArgs.Chat("Someone has voted to lynch someone.");
            }

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            if (VotesAgainst(e.Accused).Count > Match.LivingPlayers.Count / 2)
            {
                var procedure = new ProcedureStartEventArgs(e.Accused, Match.LivingPlayers, Procedure);
                OnProcedureStart(procedure);
            }
        }

        protected void Unaccused(object sender, UnaccuseEventArgs e)
        {
            NotificationEventArgs notification;
            if (e.Voter.Anonymous)
            {
                notification = NotificationEventArgs.Chat("Someone has cancelled their vote.");
            }
            else
            {
                notification = NotificationEventArgs.Chat($"{e.Voter.Name} has cancelled their vote.");
            }

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }

        protected void AccuseChange(object sender, AccuseChangeEventArgs e)
        {
            NotificationEventArgs notification;
            if (e.Voter.Anonymous)
            {
                notification = NotificationEventArgs.Chat($"Someone has changed their vote to someone else.");
            }
            else
            {
                notification = NotificationEventArgs.Chat($"{e.Voter.Name} has changed their vote to {e.NewAccuse.Name}.");
            }

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }

        protected virtual void OnProcedureStart(ProcedureStartEventArgs e)
        {
            var notification = NotificationEventArgs.Popup($"The town has decided to put {e.Against.Name} to trial.");

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            ProcedureStart?.Invoke(this, e);
        }

        public override void Start()
        {
            foreach (var voter in Match.LivingPlayers.Values)
            {
                voter.Accuse += Accused;
                voter.Unaccuse += Unaccused;
                voter.AccuseChange += AccuseChange;
            }
        }

        public override IPhase End(IMatch match)
        {
            foreach (var voter in Match.LivingPlayers.Values)
            {
                voter.Accuse -= Accused;
                voter.Unaccuse -= Unaccused;
                voter.AccuseChange -= AccuseChange;
            }

            return base.End(match);
        }
    }
}
