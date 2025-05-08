using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FlamethrowerTower : TowerController
{
    private GameObject flameEffectPrefab;
    private EnemyController currentTarget;
    private float rotationSpeed;
    private Transform towerRotationPart;
    private GameObject activeFlameEffect;
    private bool isFlameActive = false;
    private float damageTickRate;
    private float nextDamageTick;
    private float flameRange;

    public override void Initialize(TowerType type)
    {
        base.Initialize(type);

        Tower towerData = TowerCollection.towers[type];
        flameEffectPrefab = towerData.flameEffectPrefab;
        rotationSpeed = towerData.rotationSpeed;
        damageTickRate = towerData.damageTickRate;
        flameRange = towerData.flameRange;

        // Find the turret head or rotation part
        towerRotationPart = transform.Find("TowerHead");
        if (towerRotationPart == null)
            towerRotationPart = transform;

        nextDamageTick = Time.time;
    }

    protected override void UpdateAttack()
    {
        // Find target or maintain current target
        if (currentTarget == null || !enemiesInRange.Contains(currentTarget))
        {
            FindFurthestEnemy();
        }

        // Rotate towards the current target
        if (currentTarget != null)
        {
            RotateTowardsTarget();

            // Activate flame effect if it's time and not already active
            if (Time.time >= nextActivation && !isFlameActive)
            {
                ActivateFlameEffect();
            }

            // Apply damage on the damage tick rate while flame is active
            if (isFlameActive && Time.time >= nextDamageTick)
            {
                DealDamage(currentTarget, damage);
                nextDamageTick = Time.time + damageTickRate;
            }
        }
        else
        {
            // No target, deactivate flame
            if (isFlameActive)
            {
                DeactivateFlameEffect();
            }
        }

        // Reset activation timer
        if (Time.time >= nextActivation)
        {
            nextActivation = Time.time + attackInterval;
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

        // If we lost our target, deactivate the flame
        if (currentTarget == null && isFlameActive)
        {
            DeactivateFlameEffect();
        }
    }

    private void ActivateFlameEffect()
    {
        if (currentTarget != null && flameEffectPrefab != null)
        {
            // Instantiate the flame effect as a child of the tower's rotation part
            activeFlameEffect = Instantiate(flameEffectPrefab, towerRotationPart);

            // Position the flame in front of the tower
            activeFlameEffect.transform.localPosition = new Vector3(0, flameRange / 2, 0);

            activeFlameEffect.GetComponent<RectTransform>().anchoredPosition += new Vector2(-40, 187.5f);

            // Scale based on range if needed
            //float effectScaleY = flameRange / 30f; // Adjust based on prefab's original size
            //activeFlameEffect.transform.localScale = new Vector3(1f, effectScaleY, 1f);

            isFlameActive = true;
        }
    }

    private void DeactivateFlameEffect()
    {
        if (activeFlameEffect != null)
        {
            Destroy(activeFlameEffect);
            activeFlameEffect = null;
        }

        isFlameActive = false;
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

    private void OnDestroy()
    {
        // Clean up flame effect when tower is destroyed
        DeactivateFlameEffect();
    }
}