using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using WOC_Core;

namespace WOC_Server
{
    public static class CardExtension
    {
        public static bool Play(this Card card, Combatant target)
        {
            bool result = false;
            foreach (var effect in card.effects)
            {
                CardEffect cardEffect = effect as CardEffect;
                if (cardEffect.Play(target))
                {
                    result = true;
                }
            }
            return result;
        }
    }

    public class CardEffect : ICardEffect
    {
        public Card card;
        public Player player;

        public CardEffect(Player player, Card card)
        {
            this.player = player;
            this.card = card;
        }

        public virtual bool Play(Combatant target)
        {
            return false;
        }
    }

    public class CardEffectDamage : CardEffect
    {
        public int value = 0;

        public CardEffectDamage(Player player, Card card) : base(player, card)
        {
        }

        public override bool Play(Combatant target)
        {
            if (target != null && target != player)
            {
                target.life -= value;
                return true;
            }
            return false;
        }
    }

    public class CardEffectDraw : CardEffect
    {
        public int value = 0;
        public CardEffectDraw(Player player, Card card) : base(player, card)
        {
        }

        public override bool Play(Combatant target)
        {
            player.DrawCards(value);
            return true;
        }
    }

    public class CardEffectHeal : CardEffect
    {
        public int value = 0;

        public CardEffectHeal(Player player, Card card) : base(player, card)
        {
        }

        public override bool Play(Combatant target)
        {
            if (target != null)
            {
                target.life += value;
                return true;
            }
            return false;
        }
    }
}
