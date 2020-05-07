using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Mafia.NET.Matches.Phases.Vote
{
    public class Accuser
    {
        public IPlayer Player { get; set; }
        protected IPlayer? Target { get; set; }
        public string DisplayName { get; set; }
        public bool AnonymousVote { get; set; }
        public bool Active { get; set; }

        public Accuser(IPlayer player, bool anonymousVote)
        {
            Player = player;
            Target = null;
            DisplayName = Player.Name;
            AnonymousVote = anonymousVote;
            Active = true;
        }

        public IPlayer? Accusing() => Target;

        public bool Accuse(IPlayer target, out Notification? notification)
        {
            notification = null;
            if (!Active || Target == target) return false;

            var change = Target != null;
            Target = target;

            if (change)
            {
                if (AnonymousVote)
                {
                    notification = Notification.Chat($"Someone has changed their vote to someone else.");
                }
                else
                {
                    notification = Notification.Chat($"{Player.Name} has changed their vote to {target.Name}.");
                }
            }
            else
            {
                if (AnonymousVote)
                {
                    notification = Notification.Chat("Someone has voted to try someone.");
                }
                else
                {
                    notification = Notification.Chat($"{Player.Name} has voted to try {target.Name}.");
                }
            }

            return true;
        }

        public bool Unaccuse(out Notification? notification)
        {
            notification = null;
            if (!Active || Target == null) return false;

            Target = null;

            if (AnonymousVote)
            {
                notification = Notification.Chat("Someone has cancelled their vote.");
            }
            else
            {
                notification = Notification.Chat($"{Player.Name} has cancelled their vote.");
            }

            return true;
        }
    }

    public class AccuseManager
    {
        public IMatch Match { get; set; }
        protected IDictionary<IPlayer, Accuser> Accusers { get; set; }
        protected Action<IPlayer> EnoughVotes { get; }
        private bool _active { get; set; }
        public bool Active
        {
            get => _active;
            set
            {
                _active = value;
                foreach (var accuser in Accusers.Values) accuser.Active = true;
            }
        }

        public AccuseManager(IMatch match, Action<IPlayer> enoughVotes)
        {
            Match = match;
            Accusers = new Dictionary<IPlayer, Accuser>();
            EnoughVotes = enoughVotes;
            Active = true;

            foreach (var player in Match.LivingPlayers.Values)
            {
                Accusers[player] = new Accuser(player, match.Setup.AnonymousVoting);
            }
        }

        public void Accuse(IPlayer accuser, IPlayer target)
        {
            if (!Active) return;

            var accused = Accusers[accuser].Accuse(target, out var notification);

            if (accused)
            {
                foreach (var player in Match.AllPlayers.Values)
                {
                    player.OnNotification(notification);
                }

                if (VotesAgainst(target) >= RequiredVotes())
                {
                    Active = false;
                    EnoughVotes(target);
                }
            }
        }

        public Accuser Get(IPlayer accuser) => Accusers[accuser];

        public void Unaccuse(IPlayer accuser)
        {
            if (!Active) return;

            var unaccused = Accusers[accuser].Unaccuse(out var notification);

            if (unaccused)
            {
                foreach (var player in Match.AllPlayers.Values)
                {
                    player.OnNotification(notification);
                }
            }
        }

        public IList<Accuser> GetAccusers(IPlayer player)
        {
            return Accusers.Values.Where(accuser => accuser.Accusing() == player).ToList();
        }

        public int VotesAgainst(IPlayer player) => GetAccusers(player).Count;

        public int TotalVotes() => Accusers.Values.Count; // TODO: Vote power

        public int RequiredVotes() => TotalVotes() / 2 + 1;
    }
}
