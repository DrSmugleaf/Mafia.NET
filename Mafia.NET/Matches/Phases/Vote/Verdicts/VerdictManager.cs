﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players;

namespace Mafia.NET.Matches.Phases.Vote.Verdicts;

public class VerdictManager
{
    public VerdictManager(IPlayer accused)
    {
        Match = accused.Match;
        Accused = accused;
        Active = true;
        Verdicts = Match.LivingPlayers
            .Where(voter => voter != accused)
            .ToDictionary(voter => voter, voter => Verdict.Abstain);
    }

    public IMatch Match { get; }
    public IPlayer Accused { get; }
    public bool Active { get; private set; }
    private IDictionary<IPlayer, Verdict> Verdicts { get; }

    public void AddVerdict(IPlayer voter, Verdict verdict)
    {
        if (!Active ||
            voter == Accused ||
            !Verdicts.TryGetValue(voter, out var oldVerdict)) return;

        Verdicts[voter] = verdict;

        DayKey key;
        if (verdict != Verdict.Abstain && oldVerdict == Verdict.Abstain)
            key = DayKey.VoteAdd;
        else if (verdict == Verdict.Abstain && oldVerdict != Verdict.Abstain)
            key = DayKey.VoteRemove;
        else if (verdict != Verdict.Abstain && oldVerdict != Verdict.Abstain)
            key = DayKey.VoteChange;
        else
            return;

        var notification = Notification.Chat(key, voter);
        foreach (var player in Match.AllPlayers) player.OnNotification(notification);
    }

    public IList<IPlayer> Voters(Verdict verdict)
    {
        return Verdicts
            .Where(vote => vote.Value == verdict)
            .Select(vote => vote.Key)
            .ToList();
    }

    public IDictionary<Verdict, int> VerdictCount()
    {
        var count = new Dictionary<Verdict, int>();
        foreach (Verdict verdict in Enum.GetValues(typeof(Verdict)))
            count[verdict] = 0;

        foreach (var verdict in Verdicts.Values)
            count[verdict]++;

        return count;
    }

    public Notification Decision()
    {
        var count = VerdictCount();
        var innocent = count[Verdict.Innocent];
        var guilty = count[Verdict.Guilty];

        return Notification.Popup(Innocent() ? DayKey.DecisionPardon : DayKey.DecisionGuilty, Accused, innocent,
            guilty);
    }

    public EntryBundle Votes()
    {
        var message = new EntryBundle();

        foreach (var pair in Verdicts)
        {
            var player = pair.Key;
            var verdict = pair.Value;

            message.Chat(verdict switch
            {
                Verdict.Abstain => DayKey.VerdictAbstain,
                Verdict.Innocent => DayKey.VerdictInnocent,
                Verdict.Guilty => DayKey.VerdictGuilty,
                _ => throw new NotImplementedException()
            }, player);
        }

        return message;
    }

    public bool Innocent()
    {
        var votes = VerdictCount();
        return votes[Verdict.Innocent] > votes[Verdict.Guilty];
    }

    public void End()
    {
        Active = false;
    }
}