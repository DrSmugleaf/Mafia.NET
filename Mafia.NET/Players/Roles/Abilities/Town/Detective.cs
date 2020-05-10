using Mafia.NET.Matches.Chats;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterAbility("Detective", typeof(DetectiveSetup))]
    public class Detective : TownAbility<DetectiveSetup>, IDetector
    {
        public void Detect(IPlayer target)
        {
            User.Crimes.Add("Trespassing");

            var visitedMessage = "Your target did not do anything tonight.";
            if (target.Role.Ability.DetectTarget(out var visited, Setup))
                visitedMessage = $"Your target visited {visited.Name} tonight.";

            var targetNotification = Notification.Chat(visitedMessage);
            User.OnNotification(targetNotification);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), new TargetNotification
            {
                UserAddMessage = target => $"You will track {target.Name}'s activity.",
                UserRemoveMessage = target => "You won't track anyone tonight.",
                UserChangeMessage = (old, current) => $"You will instead target {current.Name}'s activity."
            });
        }
    }

    public class DetectiveSetup : ITownSetup, IIgnoresDetectionImmunity
    {
        public bool IgnoresDetectionImmunity { get; set; } = true;
    }
}