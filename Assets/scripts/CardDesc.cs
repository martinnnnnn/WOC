using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json;
using DG.Tweening;

public class CardDesc : MonoBehaviour
{
    public enum TargetType
    {
        CHARACTER,
        BATTLEFIELD
    }

    public enum Effect
    {
        SILENCE,
        ROOT,
    }

    public string name = "default name";
    public float damage;
    public int manaCost;
    public List<Effect> effects;
    TargetType targetType;
    public Color color;

    public TextMesh nameMesh;
    public TextMesh damageMesh;
    public TextMesh manaMesh;
    public TextMesh effectsMesh;
    
    private void Start()
    {
        color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        GetComponent<MeshRenderer>().material.color = color;
        nameMesh.text = name;
        damageMesh.text = damage.ToString();
        manaMesh.text = manaCost.ToString();
        string effectsStr = "";
        foreach (Effect e in effects)
        {
            effectsStr += e.ToString() + "\n";
        }
        effectsMesh.text = effectsStr;
    }
}
