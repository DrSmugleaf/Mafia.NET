using Mafia.NET.Localization;
using Mafia.NET.Matches.Phases.Vote.Verdicts;
using Mafia.NET.Players;
using Mafia.NET.Players.Roles.Abilities.Mafia;

namespace Mafia.NET.Matches.Phases.Vote
{
    public class DefensePhase : BasePhase
    {
        public DefensePhase(IMatch match, IPlayer player, uint duration = 15) : base(match, "Defense", duration)
        {
            Player = player;
        }

        public IPlayer Player { get; }

        public override IPhase NextPhase()
        {
            return new VerdictVotePhase(Match, Player);
        }

        public override void Start()
        {
            ChatManager.Open(Match.AllPlayers, true);

            if (!Player.Blackmailed || Match.Setup.Roles.Abilities.Setup<BlackmailerSetup>().BlackmailedTalkDuringTrial)
                ChatManager.Main().Participants[Player].Muted = false;

            var notification = Entry.Popup(DayKey.OnTrial, Player);

            foreach (var player in Match.AllPlayers) player.OnNotification(notification);

            base.Start();
        }
    }
}