using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Projectile : MonoBehaviour
{
    private EnemyController target;
    private float speed = 300f;
    private float damage;
    private float lifetime = 2f;
    private RectTransform rectTransform;
    private bool canPierce = false;
    private int pierceCount = 1;
    private int currentPierceCount = 0;
    private List<EnemyController> hitEnemies = new List<EnemyController>();

    public void Initialize(EnemyController target, float damage, float speed = 300f, float lifetime = 2f,
                          bool canPierce = false, int pierceCount = 1)
    {
        this.target = target;
        this.damage = damage;
        this.speed = speed;
        this.lifetime = lifetime;
        this.canPierce = canPierce;
        this.pierceCount = pierceCount;

        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            rectTransform = gameObject.AddComponent<RectTransform>();
        }

        // Make sure initial rotation is towards target
        if (target != null)
        {
            Vector2 targetPosition = target.GetComponent<RectTransform>().anchoredPosition;
            Vector2 direction = (targetPosition - rectTransform.anchoredPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        Destroy(gameObject, lifetime); // Destroy after max lifetime to prevent orphaned projectiles
    }

    void Update()
    {
        if (target == null || target.hp <= 0)
        {
            // If our target is gone, try to find a new one if we can pierce
            if (canPierce && currentPierceCount < pierceCount)
            {
                FindNewTarget();
                if (target == null)
                {
                    Destroy(gameObject);
                    return;
                }
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        // Move toward target
        Vector2 targetPosition = target.GetComponent<RectTransform>().anchoredPosition;
        Vector2 currentPosition = rectTransform.anchoredPosition;
        Vector2 direction = (targetPosition - currentPosition).normalized;

        rectTransform.anchoredPosition += direction * speed * Time.deltaTime;

        // Rotate to face direction of travel - independent of parent rotation
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
        transform.rotation = Quaternion.Euler(0, 0, angle);

        // Check for collision
        float distance = Vector2.Distance(rectTransform.anchoredPosition, targetPosition);
        if (distance < 30f)
        {
            HitTarget();
        }
    }

    void HitTarget()
    {
        if (target != null)
        {
            target.hp -= damage;
            hitEnemies.Add(target);

            if (target.hp <= 0)
            {
                GameHandler.instance.money += EnemyCollection.enemies[target.GetEnemyType()].moneyGain;
                Destroy(target.gameObject);
            }

            // If we can pierce, find a new target
            if (canPierce && ++currentPierceCount < pierceCount)
            {
                FindNewTarget();
                if (target != null) return; // Continue if we found a new target
            }
        }

        Destroy(gameObject);
    }

    void FindNewTarget()
    {
        // Find the nearest enemy that we haven't hit yet
        target = null;
        float closestDistance = float.MaxValue;

        EnemyController[] allEnemies = FindObjectsOfType<EnemyController>();
        foreach (EnemyController enemy in allEnemies)
        {
            if (!hitEnemies.Contains(enemy))
            {
                float distance = Vector2.Distance(rectTransform.anchoredPosition,
                                                 enemy.GetComponent<RectTransform>().anchoredPosition);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    target = enemy;
                }
            }
        }

        // If we found a new target, update rotation towards it
        if (target != null)
        {
            Vector2 targetPosition = target.GetComponent<RectTransform>().anchoredPosition;
            Vector2 direction = (targetPosition - rectTransform.anchoredPosition).normalized;
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}