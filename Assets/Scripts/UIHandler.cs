using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject _towerBuyButton;
    public GameObject _moneyText;
    public TMP_Text moneyText;
    public Transform towerBuyBeam;
    public ColorBlock defaultColorBlock;
    public ColorBlock adjustedColorBlock;
    public GameObject gameOverScreen;
    public GameObject loseGame;
    public GameObject winGame;
    public TMP_Text livesText;
    public TMP_Text wavesText;

    List<GameObject> buyButtons = new List<GameObject>();

    public static UIHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Update()
    {
        moneyText.text = $"${GameHandler.instance.money}";
        livesText.text = $"{GameHandler.instance.lives} / 5";
        wavesText.text = $"{GameHandler.instance.currentWave} / 10";

        if (GameHandler.instance.lives <= 0)
        {
            gameOverScreen.transform.SetAsLastSibling();
            gameOverScreen.SetActive(true);
            loseGame.SetActive(true);
            Time.timeScale = 0;
        }

        // Check for win condition
        if (GameHandler.instance.allWavesSpawned && GameHandler.instance.AreNoEnemiesPresent())
        {
            WinGame();
        }
    }

    public void WinGame()
    {
        gameOverScreen.transform.SetAsLastSibling();
        gameOverScreen.SetActive(true);
        winGame.SetActive(true);
        Time.timeScale = 0;
    }

    public void Initialize()
    {
        moneyText = Instantiate(_moneyText, towerBuyBeam).GetComponent<TMP_Text>();
        moneyText.text = $"${GameHandler.instance.money}";
        foreach (KeyValuePair<TowerType, Tower> kvp in TowerCollection.towers)
        {
            GameObject buyButton = Instantiate(_towerBuyButton, towerBuyBeam);
            buyButton.transform.Find("Tower Text").GetComponent<TMP_Text>().text = TowerCollection.towerNamesFormatted[kvp.Key];
            buyButton.transform.Find("Price").GetComponent<TMP_Text>().text = $"${kvp.Value.price}";
            buyButton.GetComponent<TowerBuyButtonTypeHolder>().type = kvp.Key;
            buyButton.GetComponent<Button>().onClick.AddListener(() =>
            {
                if (GameHandler.instance.selectedTowerType == kvp.Key && GameHandler.instance.towerPlacementActivated)
                {
                    // Unselect this tower type
                    GameHandler.instance.towerPlacementActivated = false;
                    GameHandler.instance.selectedTowerType = TowerType.None;
                    buyButton.GetComponent<Button>().colors = defaultColorBlock;
                }
                else
                {
                    // Select this tower type
                    GameHandler.instance.towerPlacementActivated = true;
                    GameHandler.instance.selectedTowerType = kvp.Key;
                    Button button = buyButton.GetComponent<Button>();
                    button.colors = adjustedColorBlock;
                    foreach (GameObject towerBuyButton in buyButtons)
                    {
                        if (towerBuyButton.GetComponent<TowerBuyButtonTypeHolder>().type != kvp.Key)
                        {
                            towerBuyButton.GetComponent<Button>().colors = defaultColorBlock;
                        }
                    }
                }
            });
            buyButtons.Add(buyButton);
        }
    }
}
