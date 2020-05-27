using System;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    public interface IAbilityAction
    {
        void Try(Action<IAbilityAction> action);
        void Chat();
        void Detain();
        void Vest();
        void Switch();
        void Block();
        void Misc();
        void Kill();
        void Protect();
        void Clean();
        void Detect();
        void Disguise();
        void MasonRecruit();
        void CultRecruit();
        void Revenge();
    }
}