using Mafia.NET.Matches;
using System;
using System.Collections.Generic;

#nullable enable

namespace Mafia.NET.Players.Roles.Abilities
{
    public class Target
    {
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
                    var e = new TargetRemoveEventArgs(old);
                    OnTargetRemove(e);
                }
                else if (Targeted != null && old == null)
                {
                    var e = new TargetAddEventArgs(Targeted);
                    OnTargetAdd(e);
                }
                else if (Targeted != null && old != null)
                {
                    var e = new TargetChangeEventArgs(old, Targeted);
                    OnTargetChange(e);
                }
            }
        }
        public TargetFilter Filter { get; }
#nullable disable
        public event EventHandler<TargetAddEventArgs> TargetAdd;
        public event EventHandler<TargetRemoveEventArgs> TargetRemove;
        public event EventHandler<TargetChangeEventArgs> TargetChange;
#nullable enable

        public Target(TargetFilter filter)
        {
            Filter = filter;
        }

        public bool Try(out IPlayer? target)
        {
            target = Targeted;
            return target != null;
        }

        public IReadOnlyDictionary<int, IPlayer> ValidTargets(IMatch match) => Filter.Filter(match.AllPlayers);

        public virtual void OnTargetAdd(TargetAddEventArgs e) => TargetAdd?.Invoke(this, e);

        public virtual void OnTargetRemove(TargetRemoveEventArgs e) => TargetRemove?.Invoke(this, e);

        public virtual void OnTargetChange(TargetChangeEventArgs e) => TargetChange?.Invoke(this, e);
    }

    public class TargetAddEventArgs : EventArgs
    {
        public IPlayer Target { get; }

        public TargetAddEventArgs(IPlayer target) => Target = target;
    }

    public class TargetRemoveEventArgs : EventArgs
    {
        public IPlayer Target { get; }

        public TargetRemoveEventArgs(IPlayer target) => Target = target;
    }

    public class TargetChangeEventArgs : EventArgs
    {
        public IPlayer Old { get; }
        public IPlayer New { get; }

        public TargetChangeEventArgs(IPlayer oldPlayer, IPlayer newPlayer)
        {
            Old = oldPlayer;
            New = newPlayer;
        }
    }
}
