using Mafia.NET.Localization;
using Mafia.NET.Players.Roles.Abilities.Town;

namespace Mafia.NET.Players.Roles.Abilities.Actions
{
    [RegisterKey]
    public enum DetainKey
    {
        Nickname,
        NightUserAddMessage,
        NightUserRemoveMessage,
        NightTargetAddMessage,
        NightTargetRemoveMessage
    }

    public class Detain : AbilityAction
    {
        public Detain(
            IAbility ability,
            int priority = -2,
            bool direct = true,
            bool stoppable = true) :
            base(ability, priority, direct, stoppable)
        {
        }

        public override bool Use(IPlayer prisoner)
        {
            User.Crimes.Add(CrimeKey.Kidnapping);

            var chatId = $"Jailor-{prisoner.Number}";
            var chat = new ChatAction<JailorChat>(Ability, chatId);
            chat.Use();

            var jail = Match.Chat.Open(chatId);
            jail.Add(prisoner);

            prisoner.Ability.CurrentNightImmunity = (int) AttackStrength.Base;
            prisoner.Ability.PiercingBlockedBy(User);

            return true;
        }
    }
}