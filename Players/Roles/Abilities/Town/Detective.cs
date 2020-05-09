using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Detective", typeof(DetectiveSetup))]
    public class Detective : TownAbility<DetectiveSetup>, IDetector
    {
        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetNotification()
            {
                UserAddMessage = (target) => $"You will track {target.Name}'s activity.",
                UserRemoveMessage = (target) => "You won't track anyone tonight.",
                UserChangeMessage = (old, _new) => $"You will instead target {_new.Name}'s activity."
            });
        }

        public void Detect(IPlayer target)
        {
            User.Crimes.Add("Trespassing");

            string visitedMessage = "Your target did not do anything tonight.";
            if (target.Role.Ability.DetectTarget(out var visited, Setup))
            {
                visitedMessage = $"Your target visited {visited.Name} tonight.";
            }

            var targetNotification = Notification.Chat(visitedMessage);
            User.OnNotification(targetNotification);
        }
    }

    public class DetectiveSetup : ITownSetup, IIgnoresDetectionImmunity
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
    }
}
