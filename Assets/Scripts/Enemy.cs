using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    Wrapping,
    Cup,
    Bag
}

public class Enemy
{
    public float hp;
    public float moveSpeed;
    public EnemyType type;
    public Sprite sprite;
    public int moneyGain;
}

public static class EnemyCollection
{
    public static Dictionary<EnemyType, Enemy> enemies = new Dictionary<EnemyType, Enemy>()
    {
        { EnemyType.Wrapping, new Enemy(){ hp=75, moveSpeed=8, type=EnemyType.Wrapping, sprite=SpriteCollection.instance.sprites[0], moneyGain=5 } },
        { EnemyType.Cup, new Enemy(){ hp=150, moveSpeed=12, type=EnemyType.Cup, sprite=SpriteCollection.instance.sprites[1], moneyGain=10 } },
        { EnemyType.Bag, new Enemy(){ hp=750, moveSpeed=6, type=EnemyType.Bag, sprite=SpriteCollection.instance.sprites[2], moneyGain=20 } },
    };
}