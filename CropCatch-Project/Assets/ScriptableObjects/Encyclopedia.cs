using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.Antlr3.Runtime.Collections;
using UnityEngine;

[CreateAssetMenu(fileName = "GameSaveData", menuName = "ScriptableObjects/Encyclopedia")]
public class Encyclopedia : ScriptableObject
{
    public Dictionary<string, int> discovery = new Dictionary<string, int>()
    {
        {"Bag Fish", 0}, //island fish
        {"Clown Fish", 0},
        {"Gold Fish", 0},
        {"Angel Fish", 0},
        {"Sword Fish", 0},
        {"Rainbow Fish", 0},

        {"Bone Fish", 0}, //swamp fish
        {"Cow Fish", 0},
        {"Frog Fish", 0},
        {"Hazmat Fish", 0},
        {"Slime Fish", 0},
        {"Witch Fish", 0},

        {"Cancer", 0}, // space fish
        {"Sand Fish", 0},
        {"Star Fish", 0},
        {"Capricorn", 0},
        {"Pisces", 0},
        {"Cosmic Fish", 0}
    };

    public void Reset()
    {
        List<string> keys = new List<string>();
        foreach(var key in discovery.Keys)
        {
            keys.Add(key);
        }

        for(int i = 0; i < keys.Count; i++)
        {
            discovery[keys[i]] = 0;
        }
    }
}
