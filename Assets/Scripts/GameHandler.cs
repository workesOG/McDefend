using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public GameObject _tile;
    public GameObject _tower;
    public Transform mapTransform;
    public Transform canvas;
    [HideInInspector]
    public Dictionary<Vector2, GameObject> tiles = new Dictionary<Vector2, GameObject>();
    [HideInInspector]
    public List<Vector2> takenTiles = new List<Vector2>();
    [HideInInspector]
    public Vector2 spawnTile;
    [HideInInspector]
    bool waveHandlerInitialized = false;

    public bool towerPlacementActivated = false;
    public TowerType selectedTowerType = TowerType.None;

    public int money = 50;

    public Dictionary<char, TileType> charTileTypeMap = new Dictionary<char, TileType>()
    {
        { 'n', TileType.Normal },
        { 'g', TileType.Grease },
        { 'p', TileType.Path },
        { 's', TileType.Spawnpoint },
        { 'e', TileType.Endpoint },
        { 'b', TileType.Border },
    };

    List<char> map = new List<char>()
    {
        'b','b','b','s','b','b','b','b','b','b','b','b','b','b','b','b',
        'b','n','n','p','n','n','n','n','n','n','n','p','p','p','p','b',
        'b','n','n','p','n','n','n','n','g','n','p','p','n','n','p','b',
        'b','n','n','p','n','n','n','n','g','g','p','n','n','n','p','b',
        'b','p','p','p','p','p','p','g','g','g','p','n','n','n','p','b',
        'b','p','n','p','n','n','p','g','g','n','p','g','g','p','p','b',
        'b','p','p','p','n','n','p','p','p','p','p','g','p','p','n','b',
        'b','n','g','g','n','n','n','n','n','n','n','n','p','n','n','b',
        'b','b','b','b','b','b','b','b','b','b','b','b','e','b','b','b',
    };

    public static GameHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        GenerateMap();
        UIHandler.instance.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (waveHandlerInitialized)
            return;

        if (tiles[spawnTile].GetComponent<RectTransform>().anchoredPosition != new Vector2(0, 0))
        {
            WaveHandler.instance.Initialize(tiles[spawnTile].GetComponent<RectTransform>().anchoredPosition);
            waveHandlerInitialized = true;
        }
    }

    public void PlaceTower(TowerType type, Vector2 tile)
    {
        GameObject go = Instantiate(_tower, canvas);
        go.GetComponent<RectTransform>().anchoredPosition = tiles[tile].GetComponent<RectTransform>().anchoredPosition + new Vector2(10, -10);
        Image imageComponent = go.GetComponent<Image>();
        imageComponent.sprite = TowerCollection.towers[type].sprite;
        imageComponent.enabled = true;
        go.GetComponent<TowerController>().Initialize(type);
    }

    void GenerateMap()
    {
        int i = 1;
        int j = 1;
        foreach (char tileChar in map)
        {
            GameObject go = Instantiate(_tile, mapTransform);
            TileController controller = go.GetComponent<TileController>();
            controller.Initialize(charTileTypeMap[tileChar], i, j);
            tiles.Add(new Vector2(i, j), go);

            if (tileChar == 's')
                spawnTile = new Vector2(i, j);

            i++;
            if (i > 16)
            {
                i = 1;
                j++;
            }
        }
    }
}
