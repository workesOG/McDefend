using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float hp;
    public float speed;
    public Sprite sprite;

    bool initialized = false;
    int currentStep = 0;
    float currentProgress = 0;

    int moveAnimationFPS = 60;
    float tileProgressPerFrame;

    float nextActivation;
    float frameInterval;

    Dictionary<int, Vector2> path = new Dictionary<int, Vector2>()
    {
        { 0, new Vector2(4,2)},
        { 1, new Vector2(4,3)},
        { 2, new Vector2(4,4)},
        { 3, new Vector2(4,5)},
        { 4, new Vector2(4,6)},
        { 5, new Vector2(4,7)},
        { 6, new Vector2(3,7)},
        { 7, new Vector2(2,7)},
        { 8, new Vector2(2,6)},
        { 9, new Vector2(2,5)},
        { 10, new Vector2(3,5)},
        { 11, new Vector2(4,5)},
        { 12, new Vector2(5,5)},
        { 13, new Vector2(6,5)},
        { 14, new Vector2(7,5)},
        { 15, new Vector2(7,6)},
        { 16, new Vector2(7,7)},
        { 17, new Vector2(8,7)},
        { 18, new Vector2(9,7)},
        { 19, new Vector2(10,7)},
        { 20, new Vector2(11,7)},
        { 21, new Vector2(11,6)},
        { 22, new Vector2(11,5)},
        { 23, new Vector2(11,4)},
        { 24, new Vector2(11,3)},
        { 25, new Vector2(12,3)},
        { 26, new Vector2(12,2)},
        { 27, new Vector2(13,2)},
        { 28, new Vector2(14,2)},
        { 29, new Vector2(15,2)},
        { 30, new Vector2(15,3)},
        { 31, new Vector2(15,4)},
        { 32, new Vector2(15,5)},
        { 33, new Vector2(15,6)},
        { 34, new Vector2(14,6)},
        { 35, new Vector2(14,7)},
        { 36, new Vector2(13,7)},
        { 37, new Vector2(13,8)},
        { 38, new Vector2(13,9)},
    };

    public void Initialize(EnemyType type)
    {
        Enemy enemy = EnemyCollection.enemies[type];
        hp = enemy.hp;
        speed = enemy.moveSpeed;

        tileProgressPerFrame = 1f / 600f * speed;

        frameInterval = 1f / (float)moveAnimationFPS;
        nextActivation = Time.time + frameInterval;
        initialized = true;
    }

    void Update()
    {
        if (!initialized)
            return;

        if (Time.time >= nextActivation)
        {
            MoveTowardsGoal();
            nextActivation += frameInterval;
        }
    }

    void MoveTowardsGoal()
    {
        if (currentStep == path.Count)
        {
            Destroy(this.gameObject);
            return;
        }

        Vector2 startPosition;
        Vector2 nextPosition = GameHandler.instance.tiles[path[currentStep]].GetComponent<RectTransform>().anchoredPosition;

        if (currentStep == 0)
            startPosition = GameHandler.instance.tiles[new Vector2(4, 1)].GetComponent<RectTransform>().anchoredPosition;
        else
            startPosition = GameHandler.instance.tiles[path[currentStep - 1]].GetComponent<RectTransform>().anchoredPosition;

        currentProgress += tileProgressPerFrame;
        Vector2 enemyPosition = Vector2.Lerp(startPosition, nextPosition, currentProgress);
        enemyPosition += new Vector2(15, -15);
        GetComponent<RectTransform>().anchoredPosition = enemyPosition;
        if (currentProgress >= 1.0f)
        {
            currentProgress = 0f;
            currentStep++;
        }
    }
}
