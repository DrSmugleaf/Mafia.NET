using Mafia.NET.Matches.Chats;
using Mafia.NET.Matches.Players.Votes;
using Mafia.NET.Players;
using Mafia.NET.Players.Votes;
using System.Collections.Generic;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class AccusePhase : BasePhase
    {
        IPhase Procedure { get; } // TODO

        public AccusePhase(IMatch match, int duration = 80) : base(match, "Time Left", duration, new NightPhase(match))
        {
            Procedure = match.Setup.Procedure;
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
                Match.SupersedePhase(new TrialPhase(Match, e.Accused));
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

        public override void Start()
        {
            foreach (var voter in Match.LivingPlayers.Values)
            {
                voter.Accuse += Accused;
                voter.Unaccuse += Unaccused;
                voter.AccuseChange += AccuseChange;
            }

            var notification = NotificationEventArgs.Popup("Today's public vote and trial will begin now.");
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }

            notification = NotificationEventArgs.Popup($"{Match.LivingPlayers.Count / 2 + 1} votes are needed to send someone to trial.");
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }

        public override IPhase End()
        {
            foreach (var voter in Match.LivingPlayers.Values)
            {
                voter.Accuse -= Accused;
                voter.Unaccuse -= Unaccused;
                voter.AccuseChange -= AccuseChange;
            }

            return base.End();
        }
    }
}
