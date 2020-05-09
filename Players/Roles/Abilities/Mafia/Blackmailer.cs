namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Blackmailer", typeof(BlackmailerSetup))]
    public class Blackmailer : MafiaAbility<BlackmailerSetup>, IMisc
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetNotification()
            {
                UserAddMessage = (target) => $"You will blackmail {target.Name}.",
                UserRemoveMessage = (target) => "You won't blackmail anyone.",
                UserChangeMessage = (old, _new) => $"You will instead blackmail {_new.Name}."
            });
        }

        public void Misc(IPlayer target)
        {
            target.Blackmailed = true;
        }
    }

    public class BlackmailerSetup : MafiaMinionSetup
    {
        public bool BlackmailedTalkDuringTrial = false;
    }
}
