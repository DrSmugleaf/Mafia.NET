using Mafia.NET.Matches;
using System.Collections.Generic;

#nullable enable

namespace Mafia.NET.Players.Roles.Abilities
{
    public class Target
    {
        public IPlayer User { get; }
        private IPlayer? _targeted { get; set; }
        public IPlayer? Targeted
        {
            get => _targeted;
            set
            {
                var old = _targeted;
                _targeted = value;

                if (Targeted == null && old != null)
                {
                    var userNotification = Message.UserRemove(old);
                    User.OnNotification(userNotification);

                    var targetNotification = Message.TargetRemove(old);
                    old.OnNotification(targetNotification);
                }
                else if (Targeted != null && old == null)
                {
                    var userNotification = Message.UserAdd(Targeted);
                    User.OnNotification(userNotification);

                    var targetNotification = Message.TargetAdd(Targeted);
                    Targeted.OnNotification(targetNotification);
                }
                else if (Targeted != null && old != null)
                {
                    var userNotification = Message.UserChange(old, Targeted);
                    User.OnNotification(userNotification);

                    var targetNotification = Message.TargetChange(old, Targeted);
                    old.OnNotification(targetNotification);
                    Targeted.OnNotification(targetNotification);
                }
            }
        }
        public TargetFilter Filter { get; }
        public TargetMessage Message { get; set; }

        public Target(IPlayer user, TargetFilter filter, TargetMessage message)
        {
            User = user;
            Filter = filter;
            Message = message;
        }

        public bool Try(out IPlayer? target)
        {
            target = Targeted;
            return target != null;
        }

        public void Set(IPlayer? target) => Targeted = target;

        public IReadOnlyDictionary<int, IPlayer> ValidTargets(IMatch match) => Filter.Filter(match.AllPlayers);
    }
}
