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
    public int price;
    public int[] upgradePrices;
    public int range;
    public Sprite sprite;
    public TowerType type;
    public TileType allowedTileType;
}

public static class TowerCollection
{
    public static Dictionary<TowerType, Tower> towers = new Dictionary<TowerType, Tower>()
    {
        { TowerType.FryShooter, new Tower(){ price=30, upgradePrices=new int[3]{50, 100, 250}, range=2, sprite=SpriteCollection.instance.sprites[3], type=TowerType.FryShooter, allowedTileType=TileType.Normal } },
        { TowerType.SpicyNugget, new Tower(){ price=75, upgradePrices=new int[3]{90, 120, 340}, range=1, sprite=SpriteCollection.instance.sprites[4], type=TowerType.SpicyNugget, allowedTileType=TileType.Grease } },
        { TowerType.Catapult, new Tower(){ price=140, upgradePrices=new int[3]{150, 300, 650}, range=3, sprite=SpriteCollection.instance.sprites[5], type=TowerType.Catapult, allowedTileType=TileType.Normal } },
        { TowerType.BigMac, new Tower(){ price=40, upgradePrices=new int[3]{35, 135, 280}, range=1, sprite=SpriteCollection.instance.sprites[6], type=TowerType.BigMac, allowedTileType=TileType.Normal } },
    };
}