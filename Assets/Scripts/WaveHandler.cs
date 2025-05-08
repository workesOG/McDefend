using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WaveHandler : MonoBehaviour
{
    Vector2 startPosition;
    public GameObject _enemy;
    public Transform canvas;

    public static WaveHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Initialize(Vector2 startPosition)
    {
        this.startPosition = startPosition + new Vector2(60, -60);
        StartCoroutine(SpawnWaves());
    }

    IEnumerator SpawnWaves()
    {
        foreach (var wave in WaveCollection.waves)
        {
            foreach (EnemySpawn spawn in wave.spawns)
            {
                for (int i = 0; i < spawn.count; i++)
                {
                    GameObject go = Instantiate(_enemy, canvas);
                    go.GetComponent<RectTransform>().anchoredPosition = startPosition;
                    Image imageComponent = go.GetComponent<Image>();
                    imageComponent.sprite = EnemyCollection.enemies[spawn.type].sprite;
                    imageComponent.enabled = true;
                    go.GetComponent<EnemyController>().Initialize(spawn.type);
                    yield return new WaitForSeconds(spawn.interval);
                }
                yield return new WaitForSeconds(spawn.endDelay);
            }
            if (GameHandler.instance.currentWave > 10)
                GameHandler.instance.currentWave++;

            yield return new WaitForSeconds(wave.endDelay);
        }
        GameHandler.instance.allWavesSpawned = true;
    }
}

public class Wave
{
    public List<EnemySpawn> spawns;
    public float endDelay;
}

public class EnemySpawn
{
    public EnemyType type;
    public int count;
    public float interval;
    public float endDelay;
}

public static class WaveCollection
{
    public static List<Wave> waves = new List<Wave>()
    {
        // Wave 1: Introduction - Basic wrapping enemies
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 5, interval = 1.5f, endDelay = 0}
        }, endDelay = 5f },

        // Wave 2: Introduction to cups - Mix of wrapping and cups
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 3, interval = 1.2f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 2, interval = 1.8f, endDelay = 0}
        }, endDelay = 6f },

        // Wave 3: First challenge - More cups and faster wrapping
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 4, interval = 1.0f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 3, interval = 1.5f, endDelay = 0}
        }, endDelay = 7f },

        // Wave 4: Introduction to bags - Mix of all types
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 3, interval = 0.8f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 2, interval = 1.2f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 1, interval = 2.0f, endDelay = 0}
        }, endDelay = 8f },

        // Wave 5: Increased difficulty - More enemies, faster spawns
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 6, interval = 0.7f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 4, interval = 1.0f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 2, interval = 1.8f, endDelay = 0}
        }, endDelay = 8f },

        // Wave 6: Challenge wave - Heavy on cups and bags
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Cup, count = 5, interval = 0.8f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 3, interval = 1.5f, endDelay = 0}
        }, endDelay = 9f },

        // Wave 7: Intense wave - Fast spawns of all types
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 8, interval = 0.6f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 6, interval = 0.8f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 4, interval = 1.2f, endDelay = 0}
        }, endDelay = 9f },

        // Wave 8: Boss wave - Heavy on bags
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 4, interval = 0.5f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 3, interval = 0.7f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 6, interval = 1.0f, endDelay = 0}
        }, endDelay = 10f },

        // Wave 9: Almost final challenge - Very fast spawns
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 10, interval = 0.4f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 8, interval = 0.6f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 5, interval = 0.8f, endDelay = 0}
        }, endDelay = 10f },

        // Wave 10: Ultimate wave - Everything at once
        new Wave() { spawns=new List<EnemySpawn>() {
            new EnemySpawn() { type = EnemyType.Wrapping, count = 12, interval = 0.3f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Cup, count = 10, interval = 0.4f, endDelay = 1f},
            new EnemySpawn() { type = EnemyType.Bag, count = 8, interval = 0.6f, endDelay = 0}
        }, endDelay = 12f }
    };
}
