using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
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
            Notification notification;
            if (!e.Voter.Anonymous)
            {
                notification = Notification.Chat($"{e.Voter.Name} has voted to try {e.Accused.Name}.");
            } else
            {
                notification = Notification.Chat("Someone has voted to lynch someone.");
            }

            var notificationEvent = new NotificationEventArgs(notification);
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notificationEvent);
            }

            if (VotesAgainst(e.Accused).Count > Match.LivingPlayers.Count / 2)
            {
                var procedure = new ProcedureStartEventArgs(e.Accused, Match.LivingPlayers, Procedure);
                OnProcedureStart(procedure);
            }
        }

        protected void Unaccused(object sender, UnaccuseEventArgs e)
        {
            Notification notification;
            if (!e.Voter.Anonymous)
            {
                notification = Notification.Chat($"{e.Voter.Name} has cancelled their vote.");
            } else
            {
                notification = Notification.Chat("Someone has cancelled their vote.");
            }

            var notificationEvent = new NotificationEventArgs(notification);
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notificationEvent);
            }
        }

        protected virtual void OnProcedureStart(ProcedureStartEventArgs e)
        {
            var notification = Notification.Popup($"The town has decided to put {e.Against.Name} to trial.");
            var notificationEvent = new NotificationEventArgs(notification);

            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notificationEvent);
            }

            ProcedureStart?.Invoke(this, e);
        }

        public override void Start()
        {
            foreach (var voter in Match.LivingPlayers.Values)
            {
                voter.Accuse += Accused;
                voter.Unaccuse += Unaccused;
            }
        }

        public override IPhase End(IMatch match)
        {
            foreach (var voter in Match.LivingPlayers.Values)
            {
                voter.Accuse -= Accused;
                voter.Unaccuse -= Unaccused;
            }

            return base.End(match);
        }
    }
}
