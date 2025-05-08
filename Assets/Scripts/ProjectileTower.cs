using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProjectileTower : TowerController
{
    private GameObject projectilePrefab;
    private EnemyController currentTarget;
    private float rotationSpeed = 5f;
    private Transform towerRotationPart;
    private float projectileSpeed;
    private float projectileLifetime;
    private bool canPierce;
    private int pierceCount;

    public override void Initialize(TowerType type)
    {
        base.Initialize(type);

        Tower towerData = TowerCollection.towers[type];
        projectilePrefab = towerData.projectilePrefab;
        rotationSpeed = towerData.rotationSpeed;
        projectileSpeed = towerData.projectileSpeed;
        projectileLifetime = towerData.projectileLifetime;
        canPierce = towerData.canPierce;
        pierceCount = towerData.pierceCount;

        // Find the turret head or rotation part (might need to adjust based on your prefab structure)
        towerRotationPart = transform.Find("TowerHead");
        if (towerRotationPart == null)
            towerRotationPart = transform; // Use the main transform if no specific part found
    }

    protected override void UpdateAttack()
    {
        // Always count down the attack timer
        if (Time.time >= nextActivation)
        {
            if (currentTarget != null && enemiesInRange.Contains(currentTarget))
            {
                // Still tracking the same target
                FireProjectile();
                nextActivation = Time.time + attackInterval;
            }
            else
            {
                // Find the enemy that is furthest along the path
                FindFurthestEnemy();
                if (currentTarget != null)
                {
                    FireProjectile();
                    nextActivation = Time.time + attackInterval;
                }
                else
                {
                    // No valid targets, but still reset the timer
                    nextActivation = Time.time + attackInterval;
                }
            }
        }

        // Rotate towards the current target
        if (currentTarget != null)
        {
            RotateTowardsTarget();
        }
    }

    private void FindFurthestEnemy()
    {
        currentTarget = null;
        int furthestStep = -1;

        foreach (EnemyController enemy in enemiesInRange)
        {
            int enemyStep = enemy.GetCurrentStep();
            if (enemyStep > furthestStep)
            {
                furthestStep = enemyStep;
                currentTarget = enemy;
            }
        }
    }

    private void FireProjectile()
    {
        if (currentTarget == null || projectilePrefab == null) return;

        // Instantiate the projectile at the canvas level
        GameObject projectile = Instantiate(projectilePrefab, GameHandler.instance.canvas);

        // Ensure the projectile has a RectTransform
        RectTransform projectileRect = projectile.GetComponent<RectTransform>();
        if (projectileRect == null)
        {
            projectileRect = projectile.AddComponent<RectTransform>();
        }

        // Position at tower's position
        projectileRect.anchoredPosition = GetComponent<RectTransform>().anchoredPosition;

        // Add or get projectile script
        Projectile projectileScript = projectile.GetComponent<Projectile>();
        if (projectileScript == null)
        {
            projectileScript = projectile.AddComponent<Projectile>();
        }

        // Initialize the projectile
        projectileScript.Initialize(currentTarget, damage, projectileSpeed, projectileLifetime, canPierce, pierceCount);
    }

    private void RotateTowardsTarget()
    {
        if (currentTarget == null) return;

        Vector2 targetPosition = currentTarget.GetComponent<RectTransform>().anchoredPosition;
        Vector2 towerPosition = GetComponent<RectTransform>().anchoredPosition;
        Vector2 direction = targetPosition - towerPosition;

        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
        Quaternion targetRotation = Quaternion.Euler(0, 0, angle);
        towerRotationPart.rotation = Quaternion.Slerp(towerRotationPart.rotation, targetRotation, rotationSpeed * Time.deltaTime);
    }
}