using System.Collections.Generic;
using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Actions;

namespace Mafia.NET.Players.Roles.Abilities.Town
{
    [RegisterKey]
    public enum DoctorKey
    {
        UserAddMessage,
        UserRemoveMessage,
        UserChangeMessage,
        TargetAttacked
    }

    [RegisterAbility("Doctor", typeof(DoctorSetup))]
    public class Doctor : TownAbility<DoctorSetup>
    {
        public override void NightEnd(in IList<IAbilityAction> actions)
        {
            var heal = new Heal(this);
            actions.Add(heal);
        }

        protected override void _onNightStart()
        {
            AddTarget(TargetFilter.Living(Match).Except(User), TargetNotification.Enum<DoctorKey>());
        }
    }

    public class DoctorSetup : ITownSetup, IHealSetup
    {
        public bool KnowsIfTargetConverted = false;
        public bool PreventsCultistConversion = false; // TODO
        public bool WitchDoctorWhenConverted = false;
        public bool KnowsIfTargetAttacked { get; set; } = true;
    }
}