using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class TowerController : MonoBehaviour
{
    public int range;
    public int upgradeStage = 0;
    public TowerType type;
    protected bool initialized = false;
    protected float nextActivation;
    protected float attackInterval = 1f;
    protected int damage = 10;
    protected List<EnemyController> enemiesInRange = new List<EnemyController>();

    public virtual void Initialize(TowerType type)
    {
        Tower tower = TowerCollection.towers[type];
        this.type = type;
        range = tower.range;
        damage = tower.damage;
        attackInterval = tower.attackInterval;
        nextActivation = Time.time + attackInterval;
        initialized = true;
    }

    public void Update()
    {
        if (!initialized) return;

        FindEnemiesInRange();
        UpdateAttack();
    }

    protected virtual void FindEnemiesInRange()
    {
        enemiesInRange.Clear();

        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in allEnemies)
        {
            float distance = Vector2.Distance(GetComponent<RectTransform>().anchoredPosition, enemy.GetComponent<RectTransform>().anchoredPosition);
            if (distance <= (range) * 120f) // Multiplying by tile size approximation
            {
                enemiesInRange.Add(enemy);
            }
        }
    }

    protected virtual void UpdateAttack()
    {
        // To be overridden by child classes
    }

    protected virtual void DealDamage(EnemyController enemy, float damageAmount)
    {
        enemy.hp -= damageAmount;
        Debug.Log(damageAmount);
        if (enemy.hp <= 0)
        {
            Destroy(enemy.gameObject);
            GameHandler.instance.money += EnemyCollection.enemies[enemy.GetComponent<EnemyController>().GetEnemyType()].moneyGain;
        }
    }

    public virtual void Upgrade()
    {
        if (upgradeStage < 3)
        {
            int upgradeCost = TowerCollection.towers[type].upgradePrices[upgradeStage];
            if (GameHandler.instance.money >= upgradeCost)
            {
                GameHandler.instance.money -= upgradeCost;
                upgradeStage++;
                damage += (damage / 2); // Increase damage by 50%
            }
        }
    }
}
