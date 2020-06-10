﻿using System.Collections.Generic;

namespace Mafia.NET.Players.Roles.Perks
{
    public enum AttackStrength
    {
        None = 0,
        Base = 1,
        Pierce = 2
    }

    public class PerkManager
    {
        public PerkManager(IPlayer user = null)
        {
            User = user;
            Defense = AttackStrength.None;
            CurrentDefense = AttackStrength.None;
            DetectionImmune = false;
            CurrentlyDetectionImmune = false;
            RoleBlockImmune = false;
            CurrentlyRoleBlockImmune = false;
            RoleBlockers = new List<IPlayer>();
            Blackmailed = false;
            Doused = false;
        }

        public IPlayer User { get; set; }

        [RegisterPerk] public AttackStrength Defense { get; set; }

        public AttackStrength CurrentDefense { get; set; }

        [RegisterPerk] public bool DetectionImmune { get; set; }

        public bool CurrentlyDetectionImmune { get; set; }

        [RegisterPerk] public bool RoleBlockImmune { get; set; }

        public bool CurrentlyRoleBlockImmune { get; set; }
        public IList<IPlayer> RoleBlockers { get; set; }
        public bool RoleBlocked => RoleBlockers.Count > 0;
        public bool Blackmailed { get; set; }
        public bool Doused { get; set; }

        public bool RoleBlock(bool piercing = false, params IPlayer[] blockers)
        {
            if (CurrentlyRoleBlockImmune && !piercing) return false;
            foreach (var blocker in blockers) RoleBlockers.Add(blocker);
            return true;
        }

        public bool RoleBlock(IPlayer blocker, bool piercing = false)
        {
            return RoleBlock(piercing, blocker);
        }

        public void OnDayStart()
        {
            CurrentDefense = Defense;
            CurrentlyDetectionImmune = DetectionImmune;
            CurrentlyRoleBlockImmune = RoleBlockImmune;
            RoleBlockers.Clear();

            if (Blackmailed) User.Match.Chat.Main().Mute(User);
        }

        public void BeforeNightEnd()
        {
            Blackmailed = false;
            // TODO: Reduce cooldowns
        }
    }
}