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
        this.startPosition = startPosition + new Vector2(15, -15);
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
            yield return new WaitForSeconds(wave.endDelay);
        }
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
        new Wave() { spawns=new List<EnemySpawn>() {
             new EnemySpawn() { type = EnemyType.Wrapping, count = 5, interval = 1.5f, endDelay = 0}
        }, endDelay = 5f }
    };
}
