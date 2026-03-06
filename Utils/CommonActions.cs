using MegaCrit.Sts2.Core.CardSelection;
using MegaCrit.Sts2.Core.Commands;
using MegaCrit.Sts2.Core.Commands.Builders;
using MegaCrit.Sts2.Core.Entities.Cards;
using MegaCrit.Sts2.Core.Entities.Creatures;
using MegaCrit.Sts2.Core.GameActions.Multiplayer;
using MegaCrit.Sts2.Core.Localization;
using MegaCrit.Sts2.Core.Localization.DynamicVars;
using MegaCrit.Sts2.Core.Models;

namespace BaseLib.Utils;

public static class CommonActions
{
    public static AttackCommand CardAttack(CardModel card, CardPlay play, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        return CardAttack(card, play.Target, hitCount, vfx, sfx, tmpSfx);
    }
    public static AttackCommand CardAttack(CardModel card, Creature? target, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        decimal damage = card.DynamicVars.Damage.BaseValue;
        return CardAttack(card, target, damage, hitCount, vfx, sfx, tmpSfx);
    }
    public static AttackCommand CardAttack(CardModel card, Creature? target, decimal damage, int hitCount = 1, string? vfx = null, string? sfx = null, string? tmpSfx = null)
    {
        AttackCommand cmd = DamageCmd.Attack(damage).WithHitCount(hitCount).FromCard(card);
        var combatState = card.CombatState;
        
        switch (card.TargetType)
        {
            case TargetType.AnyEnemy:
                if (target == null) return cmd;
                cmd.Targeting(target);
                break;
            case TargetType.AllEnemies:
                if (combatState == null) return cmd;
                cmd.TargetingAllOpponents(combatState);
                break;
            case TargetType.RandomEnemy:
                if (combatState == null) return cmd;
                cmd.TargetingRandomOpponents(combatState);
                break;
            default:
                throw new Exception($"Unsupported AttackCommand target type {card.TargetType} for card {card.Title}");
        }

        if (vfx != null || sfx != null || tmpSfx != null) cmd.WithHitFx(vfx: vfx, sfx: sfx, tmpSfx: tmpSfx);

        return cmd;
    }

    public static async Task<decimal> CardBlock(CardModel card, CardPlay play)
    {
        return await CardBlock(card, card.DynamicVars.Block, play);
    }
    public static async Task<decimal> CardBlock(CardModel card, BlockVar blockVar, CardPlay play)
    {
        return await CreatureCmd.GainBlock(card.Owner.Creature, blockVar, play);
    }

    public static async Task<IEnumerable<CardModel>> Draw(CardModel card, PlayerChoiceContext context)
    {
        return await CardPileCmd.Draw(context, card.DynamicVars.Cards.BaseValue, card.Owner);
    }
    public static async Task<T?> Apply<T>(Creature target, CardModel? card, decimal amount, bool silent = false) where T : PowerModel
    {
        return await PowerCmd.Apply<T>(target, amount, card?.Owner.Creature, card, silent);
    }
    public static async Task<T?> ApplySelf<T>(CardModel card, decimal amount, bool silent = false) where T : PowerModel
    {
        return await PowerCmd.Apply<T>(card.Owner.Creature, amount, card.Owner.Creature, card, silent);
    }

    public static async Task<IEnumerable<CardModel>> SelectCards(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType, int count = 1)
    {
        CardSelectorPrefs prefs = new(selectionPrompt, count);
        var pile = pileType.GetPile(card.Owner);
        return await CardSelectCmd.FromSimpleGrid(context, pile.Cards, card.Owner, prefs);
    }
    public static async Task<IEnumerable<CardModel>> SelectCards(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType, int minCount, int maxCount)
    {
        CardSelectorPrefs prefs = new(selectionPrompt, minCount, maxCount);
        var pile = pileType.GetPile(card.Owner);
        return await CardSelectCmd.FromSimpleGrid(context, pile.Cards, card.Owner, prefs);
    }

    public static async Task<CardModel?> SelectSingleCard(CardModel card, LocString selectionPrompt, PlayerChoiceContext context, PileType pileType)
    {
        CardSelectorPrefs prefs = new(selectionPrompt, 1);
        CardPile pile = pileType.GetPile(card.Owner);
        return (await CardSelectCmd.FromSimpleGrid(context, pile.Cards, card.Owner, prefs)).FirstOrDefault();
    }
}
