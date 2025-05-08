using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameHandler : MonoBehaviour
{
    public static GameHandler instance;
    public GameObject _tile;
    public GameObject _tower;
    public GameObject _projectilePrefab1; // Fry projectile
    public GameObject _projectilePrefab2; // Big projectile
    public GameObject _flameEffectPrefab;
    public GameObject _attackAnimationPrefab;
    public GameObject gameOverScreen;
    public GameObject loseGame;
    public GameObject winGame;
    public TMP_Text livesText;
    public TMP_Text wavesText;
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
    [HideInInspector]
    public bool allWavesSpawned = false;

    public bool towerPlacementActivated = false;
    public TowerType selectedTowerType = TowerType.None;

    public int money = 50;
    public int lives = 5;
    public int currentWave = 0;

    private Dictionary<int, PopUpData> wavePopUps = new Dictionary<int, PopUpData>()
    {
        { 1, new PopUpData() { title = "Mindre CO2 mere ansvar", content = "McDonald's har forpligtet sig til at reducere sine drivhusgasudledninger med over 50% inden 2030 og sigter mod at opnå nettonuludledning i 2050." } },
        { 2, new PopUpData() { title = "Grøn energi i Danmark", content = "I samarbejde med Norlys installerer McDonald's lynladestationer ved over 75 restauranter i Danmark, hvilket gør det nemmere at oplade elbiler undervejs." } },
        { 3, new PopUpData() { title = "Verdens første HFC-fri restaurant", content = "I 2003 åbnede McDonald's i Vejle verdens første restaurant uden HFC-kølemidler, hvilket reducerer skadelige emissioner og beskytter ozonlaget." } },
        { 4, new PopUpData() { title = "Farvel til fluorstoffer", content = "99,5% af McDonald's emballage indeholder ikke længere tilsatte fluorstoffer, og arbejdet fortsætter for at finde bæredygtige alternativer." } },
        { 5, new PopUpData() { title = "Madspild bliver til ny energi", content = "I Danmark samarbejder McDonald's med Daka ReFood for at genanvende madaffald til biogas og gødning, hvilket bidrager til en cirkulær økonomi." } },
        { 6, new PopUpData() { title = "Bæredygtig kaffe i koppen", content = "Kaffeleverandøren Löfbergs, som forsyner McDonald's i Norden, har modtaget en bæredygtighedspris for deres arbejde med sporbarhed og ansvarlig kaffeproduktion." } },
        { 7, new PopUpData() { title = "FSC-certificeret emballage", content = "98,6% af McDonald's papirbaserede emballage er nu FSC- eller PEFC-certificeret, hvilket sikrer bæredygtig skovdrift og reducerer afskovning. " } },
        { 8, new PopUpData() { title = "Mindre plastik i Happy Meal", content = "McDonald's har reduceret brugen af ny plast i Happy Meal-legetøj med 63,7% og arbejder på at gøre legetøjet endnu mere miljøvenligt." } },
        { 9, new PopUpData() { title = "Bæredygtigt oksekød", content = "98,8% af McDonald's oksekød kommer fra leverandører, der overholder politikker for afskovningsfri produktion, hvilket beskytter skove og biodiversitet." } },
        { 10, new PopUpData() { title = "Fremtidens landbrug", content = "Med regenerativt landbrug forbedres jordens sundhed og biodiversitet, hvilket gør vores fødevaresystemer mere modstandsdygtige over for klimaforandringer. McDonald's samarbejder med landmænd globalt for at implementere disse praksisser og reducere landbrugsrelaterede emissioner med 16% inden 2030." } }
    };

    private HashSet<int> shownWavePopUps = new HashSet<int>();

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

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        // Set up tower prefabs
        TowerCollection.SetPrefabs(_projectilePrefab1, _projectilePrefab2,
                                 _flameEffectPrefab, _attackAnimationPrefab);

        GenerateMap();
        UIHandler.instance.Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        if (!waveHandlerInitialized)
        {
            if (tiles[spawnTile].GetComponent<RectTransform>().anchoredPosition != new Vector2(0, 0))
            {
                WaveHandler.instance.Initialize(tiles[spawnTile].GetComponent<RectTransform>().anchoredPosition);
                waveHandlerInitialized = true;
            }
            return;
        }

        if (lives <= 0)
        {
            gameOverScreen.SetActive(true);
            loseGame.SetActive(true);
            Time.timeScale = 0;
        }

        // Check for win condition
        if (allWavesSpawned && AreNoEnemiesPresent())
        {
            WinGame();
        }

        Debug.Log($"{currentWave} : {shownWavePopUps.Contains(currentWave)} : {wavePopUps.ContainsKey(currentWave)}");
        // Show wave pop-up if needed
        if (currentWave > 0 && !shownWavePopUps.Contains(currentWave) && wavePopUps.ContainsKey(currentWave))
        {
            if (PopUpManager.instance != null)
            {
                PopUpManager.instance.CreatePopUp(wavePopUps[currentWave]);
                shownWavePopUps.Add(currentWave);
                Debug.Log("Pop Up instantiated");
            }
        }

        livesText.text = $"{lives} / 5";
        wavesText.text = $"{currentWave} / 10";
    }

    public bool AreNoEnemiesPresent()
    {
        EnemyController[] enemies = FindObjectsOfType<EnemyController>();
        return enemies.Length == 0;
    }

    public void WinGame()
    {
        gameOverScreen.SetActive(true);
        winGame.SetActive(true);
        Time.timeScale = 0;
    }

    public void PlaceTower(TowerType type, Vector2 tile)
    {
        GameObject go = Instantiate(_tower, canvas);
        go.GetComponent<RectTransform>().anchoredPosition = tiles[tile].GetComponent<RectTransform>().anchoredPosition + new Vector2(60, -60);
        Image imageComponent = go.GetComponent<Image>();
        imageComponent.sprite = TowerCollection.towers[type].sprite;
        imageComponent.enabled = true;

        // Add the appropriate tower controller based on the tower class in the Tower data
        Tower towerData = TowerCollection.towers[type];
        TowerController towerController = (TowerController)go.AddComponent(towerData.towerClass);
        towerController.Initialize(type);
        takenTiles.Add(tile);
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