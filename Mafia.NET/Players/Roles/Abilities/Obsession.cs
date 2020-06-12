using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Extension;
using Mafia.NET.Localization;
using Mafia.NET.Notifications;
using Mafia.NET.Players.Deaths;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Roles.Abilities.Registry;
using Mafia.NET.Players.Roles.Abilities.Setups;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum ObsessionKey
    {
        Target,
        Success,
        Fail,
        Jester
    }
    
    [RegisterAbility("Obsession", 10, typeof(ObsessionSetup))]
    public class Obsession : Ability<IObsessionSetup>
    {
        public bool Failed { get; set; }
        public bool Success { get;set; }
        public IPlayer Target { get; set; }

        // TODO: Victory condition
        // TODO: Avoid self confirming roles (Mayor, Marshall, Crier...)
        public override void Initialize(AbilitySetupEntry setup, IPlayer user)
        {
            if (Initialized) return;
            
            base.Initialize(setup, user);

            var candidates = Match.LivingPlayers.AsEnumerable();
            if (Setup.TargetAlwaysTown)
                candidates = candidates.OrderByDescending(candidate => candidate.Role.Team.Id == "Town");

            Target = candidates.Random(Match.Random);
        }

        public override bool Active()
        {
            return base.Active() && !Failed && !Success;
            // TODO: Switch back to active if the obsession is somehow revived
        }

        public override void DayStart(in IList<IAbility> abilities)
        {
            if (!Active()) return;
            
            if (Match.Graveyard.DeathOf(Target, out var death))
            {
                if (death.Cause == DeathCause.Lynch)
                {
                    Success = true;

                    var successNotification = Notification.Chat(Role, ObsessionKey.Success, Target);
                    User.OnNotification(successNotification);
                }
                else
                {
                    Failed = true;
                
                    if (Setup.BecomeJesterOnFailure)
                    {
                        var jester = Notification.Chat(Role, ObsessionKey.Jester, Target);
                        User.OnNotification(jester);

                        var newRole = Match.Roles["Jester"].Build();
                        User.ChangeRole(newRole);
                        return;
                    }
                    else
                    {
                        var fail = Notification.Chat(Role, ObsessionKey.Fail, Target);
                        User.OnNotification(fail);
                    }
                }
            }
            
            var notification = Notification.Chat(Role, ObsessionKey.Target, Target);
            User.OnNotification(notification);
        }
    }
    
    public interface IObsessionSetup : IAbilitySetup
    {
        bool BecomeJesterOnFailure { get; set; }
        bool TargetAlwaysTown { get; set; }
        bool MustSurviveToEnd { get; set; }
    }
    
    public class ObsessionSetup : IObsessionSetup
    {
        public bool BecomeJesterOnFailure { get; set; } = true;
        public bool TargetAlwaysTown { get; set; } = true; // TODO: Validate game setup
        public bool MustSurviveToEnd { get; set; } = false;
    }
}