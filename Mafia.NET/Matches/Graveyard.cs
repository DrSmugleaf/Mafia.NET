﻿using System;
using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Notifications;
using Mafia.NET.Players;
using Mafia.NET.Players.Deaths;

namespace Mafia.NET.Matches
{
    public class Graveyard
    {
        public Graveyard(IMatch match)
        {
            Match = match;
            PublicDeaths = new List<IDeath>();
            UndisclosedDeaths = new List<IDeath>();
            Threats = new List<IDeath>();
            Announcements = new List<Notification>();
        }

        public IMatch Match { get; }
        public List<IDeath> PublicDeaths { get; }
        public List<IDeath> UndisclosedDeaths { get; }
        public List<IDeath> Threats { get; }
        public List<Notification> Announcements { get; }

        public IReadOnlyList<IDeath> AllDeaths()
        {
            return PublicDeaths.Union(UndisclosedDeaths).ToList();
        }

        public void Disclose()
        {
            PublicDeaths.AddRange(UndisclosedDeaths);
            UndisclosedDeaths.Clear();
        }

        public void SettleThreats()
        {
            var victims = new Dictionary<IPlayer, IDeath>();

            foreach (var threat in Threats)
            {
                var victim = threat.Victim;
                if (victims.TryGetValue(victim, out _))
                {
                    victims[victim].Description += Environment.NewLine + threat.Description;
                }
                else
                {
                    threat.Victim.Alive = false; // TODO: Change active depending on the action
                    victims[victim] = threat;
                }
            }

            Threats.Clear();
            UndisclosedDeaths.AddRange(victims.Values);
        }

        public bool DiedOn(int day, DeathCause cause)
        {
            return AllDeaths().Any(death => death.Day == day && death.Cause == cause);
        }

        public bool DiedToday(DeathCause cause)
        {
            return DiedOn(Match.Phase.Day, cause);
        }

        public bool LynchedToday()
        {
            return DiedToday(DeathCause.Lynch);
        }

        public List<IDeath> ThreatsOn(IPlayer player)
        {
            return Threats.Where(death => death.Victim == player).ToList();
        }

        public void Announce()
        {
            foreach (var announcement in Announcements)
            foreach (var player in Match.AllPlayers)
                player.OnNotification(announcement);

            Announcements.Clear();
        }
    }
}