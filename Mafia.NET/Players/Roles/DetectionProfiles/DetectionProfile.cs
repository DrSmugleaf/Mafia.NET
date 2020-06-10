using JetBrains.Annotations;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities;

namespace Mafia.NET.Players.Roles.DetectionProfiles
{
    public interface IDetectionProfile
    {
        IPlayer User { get; set; }

        bool DetectableBy([CanBeNull] SheriffSetup setup = null);
        Key ResolveKey([CanBeNull] SheriffSetup setup = null);
        bool TargetDetectableBy([CanBeNull] IDetectSetup setup = null);
        bool TryDetectTarget(out IPlayer target, [CanBeNull] IDetectSetup setup = null);
        bool DetectProperty([CanBeNull] SheriffSetup setup = null);
        Key GuiltyKey();
    }

    public class DetectionProfile : IDetectionProfile
    {
        public DetectionProfile(IPlayer user)
        {
            User = user;
        }

        public IPlayer User { get; set; }

        public bool DetectableBy(SheriffSetup setup = null)
        {
            return !User.Perks.CurrentlyDetectionImmune && DetectProperty(setup);
        }

        public Key ResolveKey(SheriffSetup setup = null)
        {
            if (DetectableBy(setup)) return GuiltyKey();
            return SheriffKey.NotSuspicious;
        }

        public bool TargetDetectableBy(IDetectSetup setup = null)
        {
            return !User.Perks.CurrentlyDetectionImmune ||
                   setup?.IgnoresDetectionImmunity == true;
        }

        public bool TryDetectTarget(out IPlayer target, IDetectSetup setup = null)
        {
            target = TargetDetectableBy(setup) ? User.Targets[0] : null;
            return target != null;
        }

        public bool DetectProperty(SheriffSetup setup = null)
        {
            return User.Role.Team.Id switch
            {
                "Mafia" => setup?.DetectsMafiaTriad,
                "Triad" => setup?.DetectsMafiaTriad,
                _ => User.Role.Id switch
                {
                    "Arsonist" => setup?.DetectsArsonist,
                    "Cultist" => setup?.DetectsCult,
                    "Mass Murderer" => setup?.DetectsMassMurderer,
                    "Serial Killer" => setup?.DetectsSerialKiller,
                    "Witch Doctor" => setup?.DetectsCult,
                    _ => true
                }
            } == true;
        }

        public Key GuiltyKey()
        {
            return User.Role.Team.Id switch
            {
                "Town" => SheriffKey.NotSuspicious,
                "Mafia" => SheriffKey.Mafia,
                "Triad" => SheriffKey.Triad,
                _ => User.Role.Id switch
                {
                    "Arsonist" => SheriffKey.Arsonist,
                    "Cultist" => SheriffKey.Cultist,
                    "Mass Murderer" => SheriffKey.MassMurderer,
                    "Serial Killer" => SheriffKey.SerialKiller,
                    "Witch Doctor" => SheriffKey.Cultist,
                    _ => SheriffKey.NotSuspicious
                }
            };
        }
    }
}