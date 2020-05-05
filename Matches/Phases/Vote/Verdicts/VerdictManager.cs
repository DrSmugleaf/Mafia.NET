using Mafia.NET.Matches.Chats;
using Mafia.NET.Players;
using Mafia.NET.Players.Votes;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts
{
    public class VerdictManager
    {
        public IMatch Match { get; }
        public IPlayer Player { get; }
        public bool Active { get; private set; }
        private IDictionary<IPlayer, Verdict> Verdicts { get; }

        public VerdictManager(IMatch match, IPlayer player)
        {
            Match = match;
            Player = player;
            Active = true;
            Verdicts = match.LivingPlayers.Values.Where(voter => voter != player).ToDictionary(voter => voter, voter => Verdict.ABSTAIN);
        }

        public void AddVerdict(IPlayer voter, Verdict verdict)
        {
            var oldVerdict = Verdicts[voter];
            Verdicts[voter] = verdict;

            string message;
            if (verdict != Verdict.ABSTAIN && oldVerdict == Verdict.ABSTAIN)
            {
                message = $"{voter} has voted.";
            }
            else if (verdict == Verdict.ABSTAIN && oldVerdict != Verdict.ABSTAIN)
            {
                message = $"{voter} has removed their vote.";
            }
            else if (verdict != Verdict.ABSTAIN && oldVerdict != Verdict.ABSTAIN)
            {
                message = $"{voter} has changed their vote.";
            }
            else
            {
                return;
            }

            var notification = NotificationEventArgs.Chat(message);
            foreach (var player in Match.AllPlayers.Values)
            {
                player.OnNotification(notification);
            }
        }

        public IDictionary<Verdict, int> VerdictCount()
        {
            return Verdicts.Values.GroupBy(x => x).ToDictionary(x => x.Key, x => x.Count());
        }

        public NotificationEventArgs Decision()
        {
            var count = VerdictCount();
            var innocent = count[Verdict.INNOCENT];
            var guilty = count[Verdict.GUILTY];
            var decision = Innocent() ?
                $"The town has decided to pardon {Player.Name} by a vote of {innocent} to {guilty}" :
                $"The town has decided to lynch {Player.Name} by a vote of {guilty} to {innocent}";

            return NotificationEventArgs.Popup(decision);
        }

        public NotificationEventArgs Votes()
        {
            string message = "";

            foreach (var verdict in Verdicts)
            {
                message += $"[{verdict.Key.Name} " + verdict.Value switch
                {
                    Verdict.ABSTAIN => "abstained",
                    Verdict.INNOCENT => "voted Innocent",
                    Verdict.GUILTY => "voted Guilty",
                    _ => throw new NotImplementedException()
                } + $"]{Environment.NewLine}";
            }

            return NotificationEventArgs.Chat(message);
        }

        public bool Innocent()
        {
            var votes = VerdictCount();
            return votes[Verdict.INNOCENT] > votes[Verdict.GUILTY];
        }

        public void End()
        {
            Active = false;
        }
    }
}
