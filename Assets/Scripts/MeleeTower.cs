using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MeleeTower : TowerController
{
    private float attackDuration;
    private float attackTimer = 0;
    private bool isAttacking = false;
    private bool canStun;
    private float stunDuration;
    private List<EnemyController> stunnedEnemies = new List<EnemyController>();

    private RectTransform rectTransform;
    private Vector2 originalPosition;
    private float jumpHeight = 30f; // Height of the jump in pixels

    public override void Initialize(TowerType type)
    {
        base.Initialize(type);

        Tower towerData = TowerCollection.towers[type];
        attackDuration = towerData.attackDuration;
        canStun = towerData.canStun;
        stunDuration = towerData.stunDuration;

        rectTransform = GetComponent<RectTransform>();
        originalPosition = rectTransform.anchoredPosition;
    }

    protected override void UpdateAttack()
    {
        // Check if it's time to attack
        if (Time.time >= nextActivation && enemiesInRange.Count > 0 && !isAttacking)
        {
            PerformAttack();
            nextActivation = Time.time + attackInterval;
        }

        // Update attack animation (jumping)
        if (isAttacking)
        {
            UpdateJumpAnimation();
        }

        // Update stunned enemies
        UpdateStunnedEnemies();
    }

    private void PerformAttack()
    {
        isAttacking = true;
        attackTimer = 0;

        // Damage all enemies in range
        foreach (EnemyController enemy in new List<EnemyController>(enemiesInRange))
        {
            if (enemy != null)
            {
                DealDamage(enemy, damage);

                // Apply stun if this tower can stun
                if (canStun && !stunnedEnemies.Contains(enemy))
                {
                    StunEnemy(enemy);
                }
            }
        }
    }

    private void UpdateJumpAnimation()
    {
        attackTimer += Time.deltaTime;

        if (attackTimer >= attackDuration)
        {
            // Reset position and end attack
            rectTransform.anchoredPosition = originalPosition;
            isAttacking = false;
            attackTimer = 0;
        }
        else
        {
            // Simple up and down animation using sine wave
            float jumpProgress = attackTimer / attackDuration; // 0 to 1
            float jumpCurve = Mathf.Sin(jumpProgress * Mathf.PI); // Sine curve for smooth up/down
            float currentJumpHeight = jumpHeight * jumpCurve;

            // Apply the jump height to the y position
            rectTransform.anchoredPosition = new Vector2(
                originalPosition.x,
                originalPosition.y + currentJumpHeight
            );
        }
    }

    private void StunEnemy(EnemyController enemy)
    {
        stunnedEnemies.Add(enemy);
        // You can add visual effect for stunned enemy here
        // For example, change the enemy color or add a stun animation

        // Schedule the enemy to be unstunned after stunDuration
        StartCoroutine(UnstunEnemyAfterDelay(enemy, stunDuration));
    }

    private IEnumerator UnstunEnemyAfterDelay(EnemyController enemy, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (enemy != null && stunnedEnemies.Contains(enemy))
        {
            stunnedEnemies.Remove(enemy);
            // Remove visual stun effect here
        }
    }

    private void UpdateStunnedEnemies()
    {
        // Remove any destroyed enemies from stunnedEnemies list
        for (int i = stunnedEnemies.Count - 1; i >= 0; i--)
        {
            if (stunnedEnemies[i] == null)
            {
                stunnedEnemies.RemoveAt(i);
            }
        }
    }

    // Override the FindEnemiesInRange method to use a more accurate AOE radius
    protected override void FindEnemiesInRange()
    {
        enemiesInRange.Clear();

        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in allEnemies)
        {
            // For melee tower, we use a simpler circle-based range check
            float distance = Vector2.Distance(GetComponent<RectTransform>().anchoredPosition, enemy.GetComponent<RectTransform>().anchoredPosition);
            if (distance <= range * 120f) // Adjust the multiplier based on your tile size
            {
                enemiesInRange.Add(enemy);
            }
        }
    }
}