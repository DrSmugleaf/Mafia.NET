using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Bases;
using Mafia.NET.Players.Targeting;

namespace Mafia.NET.Players.Roles.Abilities
{
    [RegisterKey]
    public enum FrameKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage
    }

    [RegisterAbility("Frame", 4)]
    public class Frame : NightEndAbility
    {
        public override void NightStart(in IList<IAbility> abilities)
        {
            SetupTargets<FrameKey>(abilities, TargetFilter.Living(Match).Except(User.Role.Team));
        }

        public override bool Use(IPlayer target)
        {
            User.Crimes.Add(CrimeKey.Trespassing);
            target.Crimes.Framing = new Framing(Match);
            return true;
        }
    }
}