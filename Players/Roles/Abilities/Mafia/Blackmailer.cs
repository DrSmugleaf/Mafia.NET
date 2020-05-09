namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Blackmailer", typeof(BlackmailerSetup))]
    public class Blackmailer : MafiaAbility<BlackmailerSetup>
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetMessage()
            {
                UserAddMessage = (target) => $"You will blackmail {target.Name}.",
                UserRemoveMessage = (target) => "You won't blackmail anyone.",
                UserChangeMessage = (old, _new) => $"You will instead blackmail {_new.Name}."
            });
        }

        protected override bool _afterNightEnd()
        {
            if (TargetManager.TryNight(0, out var target))
            {
                target.Blackmailed = true;
                return true;
            }

            return false;
        }
    }

    public class BlackmailerSetup : MafiaMinionSetup
    {
        public bool BlackmailedTalkDuringTrial = false;
    }
}
