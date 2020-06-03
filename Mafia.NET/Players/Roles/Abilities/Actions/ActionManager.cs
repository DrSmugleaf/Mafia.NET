using System.Collections.Generic;
using System.Linq;
using Mafia.NET.Matches;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public enum NightSubPhase
    {
        Start,
        End
    }

    public class ActionManager
    {
        public ActionManager(IMatch match)
        {
            Match = match;
            NightSubPhase = NightSubPhase.Start;
        }

        public IMatch Match { get; }
        public NightSubPhase NightSubPhase { get; set; }

        public IList<IAbilityAction> ActionsNightStart()
        {
            var actions = new List<IAbilityAction>();

            foreach (var player in Match.AllPlayers) player.Ability.NightStart(actions);

            return actions
                .OrderBy(action => action.Priority)
                .ThenBy(action => action.User.Number)
                .ToList();
        }

        public IList<IAbilityAction> ActionsNightEnd()
        {
            var actions = new List<IAbilityAction>();

            foreach (var player in Match.AllPlayers) player.Ability.NightEnd(actions);

            return actions
                .OrderBy(action => action.Priority)
                .ThenBy(action => action.User.Number)
                .ToList();
        }

        public void OnDayStart()
        {
            foreach (var player in Match.AllPlayers) player.Role.Ability.OnDayStart();
        }

        public void OnDayEnd()
        {
            foreach (var player in Match.AllPlayers) player.Role.Ability.OnDayEnd();
        }

        public void OnNightStart()
        {
            NightSubPhase = NightSubPhase.Start;
            foreach (var action in ActionsNightStart()) action.TryUse();
            foreach (var player in Match.AllPlayers) player.Role.Ability.OnNightStart();
        }

        public void BeforeNightEnd()
        {
            foreach (var player in Match.AllPlayers) player.Role.Ability.BeforeNightEnd();
        }

        public void OnNightEnd()
        {
            NightSubPhase = NightSubPhase.End;
            foreach (var action in ActionsNightEnd()) action.TryUse();
            foreach (var player in Match.AllPlayers) player.Role.Ability.OnNightEnd();
        }
    }
}