using Mafia.NET.Matches;
using Mafia.NET.Matches.Chats;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Abilities.Mafia;
using Mafia.NET.Players.Roles.Abilities.Neutral;
using Mafia.NET.Players.Roles.Abilities.Triad;
using Mafia.NET.Players.Roles.Categories;
using System.Linq;
using System.Linq.Expressions;

namespace Mafia.NET.Players.Roles.Abilities
{
    public interface IAbility
    {
        IMatch Match { get; set; }
        IPlayer User { get; set; }
        string Name { get; set; }
        MessageRandomizer MurderDescriptions { get; set; }
        TargetManager TargetManager { get; set; }
        IAbilitySetup AbilitySetup { get; set; }
        bool Active { get; set; }
        bool RoleBlockImmune { get; }
        bool DeathImmune { get; }
        bool CurrentlyDeathImmune { get; set; }
        bool DetectionImmune { get; }
        int Cooldown { get; set; }
        int Charges { get; set; }

        void Initialize();
        bool TryVictory(out IVictory victory);
        Notification VictoryNotification();
        void AddTarget(TargetFilter filter, TargetMessage message);
        void AddTarget(IPlayer target, TargetMessage message);
        void Disable();
        void PiercingDisable();
        void Threaten(IPlayer victim);
        void PiercingThreaten(IPlayer victim);
        bool DetectableBy(ISheriffSetup setup);
        string Guilty(ISheriffSetup setup);
        bool AloneTeam();
        void OnDayStart();
        bool OnDayEnd();
        bool OnNightStart(); // Detainments, chats
        void BeforeNightEnd(); // Unblackmail
        bool AfterNightEnd(); // Vests, switches & roleblockers, framing & arson & misc, killing & suicides, janitor, investigations, disguiser, mason recruitment, cult recruitment
    }

    public abstract class BaseAbility<T> : IAbility where T : IAbilitySetup, new()
    {
        public IMatch Match { get; set; }
        public IPlayer User { get; set; }
        public string Name { get; set; }
        public MessageRandomizer MurderDescriptions { get; set; }
        public TargetManager TargetManager { get; set; }
        public IAbilitySetup AbilitySetup { get; set; }
        public T Setup { get => (T)AbilitySetup; }
        public bool Active { get; set; }
        public bool RoleBlockImmune { get; }
        public bool DeathImmune { get; }
        public bool CurrentlyDeathImmune { get; set; }
        public bool DetectionImmune { get; }
        private int _cooldown { get; set; }
        public int Cooldown
        {
            get => _cooldown;
            set => _cooldown = value >= 0 ? value : 0;
        }
        private int _charges { get; set; }
        public int Charges
        {
            get => _charges;
            set => _charges = value >= 0 ? value : 0;
        }

        public BaseAbility()
        {
            Active = true;
            RoleBlockImmune = Setup is IRoleBlockImmune rbImmuneSetup ? rbImmuneSetup.RoleBlockImmune : false;
            DeathImmune = false;
            CurrentlyDeathImmune = DeathImmune;
            Cooldown = 0;
        }

        public void Initialize()
        {
            AbilitySetup = Match.Abilities.Setup<T>();
        }

        public virtual bool TryVictory(out IVictory victory)
        {
            var living = Match.LivingPlayers.Values;
            var enemies = User.Role.Enemies();
            victory = null;

            if (living.SelectMany(player => player.Role.Goals()).Intersect(enemies).Any()) return false;

            victory = new Victory(User, VictoryNotification());
            return true;
        }

        public virtual Notification VictoryNotification() => User.Role.Categories[0].Goal.VictoryNotification(User);

        public void AddTarget(TargetFilter filter, TargetMessage message)
        {
            TargetManager.Add(filter.Build(User, message));
        }

        public void AddTarget(IPlayer target, TargetMessage message)
        {
            AddTarget(TargetFilter.Only(target), message);
        }

        public virtual void Disable()
        {
            if (RoleBlockImmune) return;
            Active = false;
        }

        public void PiercingDisable() => Active = false;

        public virtual void Threaten(IPlayer victim)
        {
            if (victim.Role.Ability.CurrentlyDeathImmune) return;
            var threat = new Death(this, victim);
            Match.Graveyard.Threats.Add(threat);
        }

        public void PiercingThreaten(IPlayer victim)
        {
            var threat = new Death(this, victim);
            Match.Graveyard.Threats.Add(threat);
        }

        public abstract bool DetectableBy(ISheriffSetup setup);

        public string Guilty(ISheriffSetup setup)
        {
            return !DetectableBy(setup) || DetectionImmune ? "Not Suspicious" : GuiltyName();
        }

        protected abstract string GuiltyName();

        public bool AloneTeam()
        {
            return Match.LivingPlayers.Values
                .Where(player => player.Role.Affiliation == User.Role.Affiliation)
                .Count() == 1;
        }

        public virtual void OnDayStart()
        {
            User.Crimes.Framing = null;
            CurrentlyDeathImmune = DeathImmune;
            TargetManager.Reset();
            Active = true;
            _onDayStart();
        }

        public virtual bool OnDayEnd()
        {
            if (Active) _onDayEnd();
            return Active;
        }

        public virtual bool OnNightStart()
        {
            TargetManager.Reset();

            if (Active)
            {
                if (Setup is ICooldownSetup && Cooldown > 0)
                {
                    return false;
                }
                else
                {
                    _onNightStart();
                    return true;
                }
            }
            else return false;
        }

        public void BeforeNightEnd()
        {
            User.Blackmailed = false;
        }

        public virtual bool AfterNightEnd()
        {
            if (Active)
            {
                if (Setup is ICooldownSetup cooldownSetup)
                {
                    if (Cooldown > 0)
                    {
                        Cooldown--;
                        return false;
                    }
                    else
                    {
                        var action = _afterNightEnd();
                        if (action) Cooldown = cooldownSetup.Cooldown;
                        return action;
                    }
                }
                else
                {
                    return _afterNightEnd();
                }
            }
            else
            {
                Cooldown--;
                return false;
            }
        }

        protected virtual void _onDayStart() => Expression.Empty();

        protected virtual void _onDayEnd() => Expression.Empty();

        protected virtual void _onNightStart() => Expression.Empty();

        protected virtual bool _afterNightEnd() => false;
    }

    public interface ICooldownSetup
    {
        int Cooldown { get; set; }
    }

    public interface IChargeSetup
    {
        int Charges { get; set; }
    }

    public interface IRoleBlockImmune
    {
        bool RoleBlockImmune { get; set; }
    }

    public interface IDetectionImmune
    {
        bool DetectionImmune { get; set; }
    }

    public interface ISheriffSetup
    {
        bool DetectsMafiaTriad { get; set; }
        bool DetectsSerialKiller { get; set; }
        bool DetectsArsonist { get; set; }
        bool DetectsCult { get; set; }
        bool DetectsMassMurderer { get; set; }
    }
}
