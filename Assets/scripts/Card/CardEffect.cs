using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;

public class CardEffect : MonoBehaviour
{
    public enum Type
    {
        Damage,
        Block,
        Heal,
        DrawCard,
        DiscardCard,
        CopyToDeck,
        CopyToDiscard,
        UpgradeCard,
        ApplyWeak,
        ApplyStrenght,
        ApplyVulnarable,
        ShuffleDeck,
        ShuffleDiscard
    }
    public int value;
    public Type type;
    public string display = "";

    public override string ToString()
    {
        return string.Format(display, value) + "\n";
    }

    public void Apply(Character chara)
    {
        Debug.Log(string.Format("Applying {0} {1} to {2}", value, type.ToString(), chara.title));
    }

    public void Apply(Player player)
    {
        Debug.Log(string.Format("Applying {0} {1} to {2}", value, type.ToString(), player.pseudo));

    }
}
