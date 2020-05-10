namespace Mafia.NET.Players.Roles.Abilities.Mafia
{
    [RegisterAbility("Blackmailer", typeof(BlackmailerSetup))]
    public class Blackmailer : MafiaAbility<BlackmailerSetup>, IMisc
    {
        public void Misc(IPlayer target)
        {
            target.Blackmailed = true;
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User.Role.Affiliation), new TargetNotification
            {
                UserAddMessage = target => $"You will blackmail {target.Name}.",
                UserRemoveMessage = target => "You won't blackmail anyone.",
                UserChangeMessage = (old, current) => $"You will instead blackmail {current.Name}."
            });
        }
    }

    public class BlackmailerSetup : MafiaMinionSetup
    {
        public bool BlackmailedTalkDuringTrial = false;
    }
}