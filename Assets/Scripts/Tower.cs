using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TowerType
{
    None,
    FryShooter,
    SpicyNugget,
    Catapult,
    BigMac,
}

public class Tower
{
    // Base properties
    public int price;
    public int[] upgradePrices;
    public int range;
    public Sprite sprite;
    public TowerType type;
    public TileType allowedTileType;
    public float attackInterval = 1.0f;
    public int damage = 10;
    public bool hasProjectile = true;
    public bool isAOE = false;
    public bool isContinuous = false;
    public System.Type towerClass;

    // ProjectileTower specific properties
    public GameObject projectilePrefab; // Prefab for the projectile
    public float projectileSpeed = 300f;
    public float projectileLifetime = 2f;
    public bool canPierce = false;
    public int pierceCount = 1;

    // FlamethrowerTower specific properties
    public GameObject flameEffectPrefab; // Prefab for the flame effect
    public float damageTickRate = 0.2f;
    public float flameRange = 30f;

    // MeleeTower specific properties
    public GameObject attackAnimationPrefab; // Prefab for the attack animation
    public float attackDuration = 0.5f;
    public bool canStun = false;
    public float stunDuration = 0.5f;

    // Common properties for towers that rotate
    public float rotationSpeed = 5f;
}

public static class TowerCollection
{
    // Reference to prefabs - these would be set in the editor or via code
    private static GameObject fryProjectilePrefab;
    private static GameObject bigProjectilePrefab;
    private static GameObject flameEffectPrefab;
    private static GameObject attackAnimationPrefab;

    // Method to set prefabs
    public static void SetPrefabs(GameObject fryProjectile, GameObject bigProjectile,
                                 GameObject flameEffect, GameObject attackAnim)
    {
        fryProjectilePrefab = fryProjectile;
        bigProjectilePrefab = bigProjectile;
        flameEffectPrefab = flameEffect;
        attackAnimationPrefab = attackAnim;

        // Update prefabs in the tower definitions
        towers[TowerType.FryShooter].projectilePrefab = fryProjectilePrefab;
        towers[TowerType.Catapult].projectilePrefab = bigProjectilePrefab;
        towers[TowerType.SpicyNugget].flameEffectPrefab = flameEffectPrefab;
        towers[TowerType.BigMac].attackAnimationPrefab = attackAnimationPrefab;
    }

    public static Dictionary<TowerType, Tower> towers = new Dictionary<TowerType, Tower>()
    {
        { TowerType.FryShooter, new Tower(){ 
            // Base properties
            price=30,
            upgradePrices=new int[3]{50, 100, 250},
            range=2,
            sprite=SpriteCollection.instance.sprites[3],
            type=TowerType.FryShooter,
            allowedTileType=TileType.Normal,
            attackInterval=1.0f,
            damage=20,
            hasProjectile=true,
            isAOE=false,
            isContinuous=false,
            towerClass=typeof(ProjectileTower),
            
            // Specific properties for ProjectileTower
            projectileSpeed=350f,
            projectileLifetime=2f,
            canPierce=false,
            pierceCount=1,
            rotationSpeed=5f
        }},
        { TowerType.SpicyNugget, new Tower(){ 
            // Base properties
            price=75,
            upgradePrices=new int[3]{90, 120, 340},
            range=2,
            sprite=SpriteCollection.instance.sprites[4],
            type=TowerType.SpicyNugget,
            allowedTileType=TileType.Grease,
            attackInterval=0.1f,
            damage=5,
            hasProjectile=false,
            isAOE=false,
            isContinuous=true,
            towerClass=typeof(FlamethrowerTower),
            
            // Specific properties for FlamethrowerTower
            damageTickRate=0.05f,
            flameRange=25f,
            rotationSpeed=3.5f
        }},
        { TowerType.Catapult, new Tower(){ 
            // Base properties
            price=140,
            upgradePrices=new int[3]{150, 300, 650},
            range=3,
            sprite=SpriteCollection.instance.sprites[5],
            type=TowerType.Catapult,
            allowedTileType=TileType.Normal,
            attackInterval=2.0f,
            damage=120,
            hasProjectile=true,
            isAOE=false,
            isContinuous=false,
            towerClass=typeof(ProjectileTower),
            
            // Specific properties for ProjectileTower
            projectileSpeed=250f, // Slower projectile for the catapult
            projectileLifetime=3f,
            canPierce=false,
            pierceCount=1, // Can hit 2 enemies
            rotationSpeed=2f // Slower rotation for the catapult
        }},
        { TowerType.BigMac, new Tower(){ 
            // Base properties
            price=40,
            upgradePrices=new int[3]{35, 135, 280},
            range=2,
            sprite=SpriteCollection.instance.sprites[6],
            type=TowerType.BigMac,
            allowedTileType=TileType.Normal,
            attackInterval=1.5f,
            damage=15,
            hasProjectile=false,
            isAOE=true,
            isContinuous=false,
            towerClass=typeof(MeleeTower),
            
            // Specific properties for MeleeTower
            attackDuration=0.7f,
            canStun=true,
            stunDuration=0.3f
        }},
    };

    public static Dictionary<TowerType, string> towerNamesFormatted = new Dictionary<TowerType, string>()
    {
        { TowerType.FryShooter, "Fry Shooter" },
        { TowerType.SpicyNugget, "Spicy Nugget" },
        { TowerType.Catapult, "Catapult" },
        { TowerType.BigMac, "Big Mac" },
    };
}