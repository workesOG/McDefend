using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController : MonoBehaviour
{
    public int range;
    public int upgradeStage = 0;
    public TowerType type;
    bool initialized = false;
    float nextActivation;

    public void Initialize(TowerType type)
    {
        Tower tower = TowerCollection.towers[type];
        this.type = type;
        range = tower.range;

        //nextActivation = Time.time + frameInterval;
        initialized = true;
    }

    public void Update()
    {
        
    }
}
