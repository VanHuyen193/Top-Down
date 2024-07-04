using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Passive Data", menuName = "2D Top-down/Passive Data")]
public class PassiveData : ItemData
{
    public Passive.Modifier baseStats;
    public Passive.Modifier[] growth;

    public Passive.Modifier GetLevelData(int level)
    {
        if(level - 2 < growth.Length)
        {
            return growth[level - 2];
        }
        Debug.LogWarning("Passive doesn't have its level up stats");
        return new Passive.Modifier();
    }
}
