using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum TileType
{
    Normal,
    Grease,
    Path,
    Spawnpoint,
    Endpoint,
    Border,
}

public class TileController : MonoBehaviour, IPointerDownHandler
{
    public Vector2 coordinates;
    public TileType type;

    private Dictionary<TileType, Color32> tileTypeColorMap = new Dictionary<TileType, Color32>()
    {
        { TileType.Normal, new Color32(255, 215, 204, 255)},
        { TileType.Grease, new Color32(217, 201, 52, 255)},
        { TileType.Path, new Color32(241, 123, 77, 255)},
        { TileType.Spawnpoint, new Color32(215, 238, 115, 255)},
        { TileType.Endpoint, new Color32(190, 70, 25, 255)},
        { TileType.Border, new Color32(178, 143, 133, 255)},
    };

    public void Initialize(TileType type, int xCoord, int yCoord)
    {
        transform.Find("Tile Fill").GetComponent<Image>().color = tileTypeColorMap[type];
        this.type = type;
        coordinates = new Vector2(xCoord, yCoord);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!GameHandler.instance.towerPlacementActivated)
        {
            Debug.Log("Tower placement is not activated");
            return;
        }

        if (GameHandler.instance.selectedTowerType == TowerType.None)
        {
            Debug.Log("Tower type is \"None\"");
            return;
        }

        if (GameHandler.instance.takenTiles.Contains(coordinates))
        {
            Debug.Log("Tower already placed on this tile");
            return;
        }

        if (!(TowerCollection.towers[GameHandler.instance.selectedTowerType].allowedTileType == type))
        {
            Debug.Log("Tower not placed on the correct tile type");
            return;
        }

        if (GameHandler.instance.money < TowerCollection.towers[GameHandler.instance.selectedTowerType].price)
        {
            GameHandler.instance.towerPlacementActivated = false;
            GameHandler.instance.selectedTowerType = TowerType.None;
            return;
        }

        GameHandler.instance.money -= TowerCollection.towers[GameHandler.instance.selectedTowerType].price;
        GameHandler.instance.PlaceTower(GameHandler.instance.selectedTowerType, coordinates);
    }
}
