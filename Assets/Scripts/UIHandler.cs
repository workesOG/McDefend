using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIHandler : MonoBehaviour
{
    public GameObject _towerBuyButton;
    public Transform towerBuyBeam;
    public ColorBlock defaultColorBlock;
    public ColorBlock adjustedColorBlock;

    List<GameObject> buyButtons = new List<GameObject>();

    public static UIHandler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    public void Initialize()
    {
        Debug.Log(TowerCollection.towers.Count);
        foreach (KeyValuePair<TowerType, Tower> kvp in TowerCollection.towers)
        {
            GameObject buyButton = Instantiate(_towerBuyButton, towerBuyBeam);
            buyButton.transform.Find("Text").GetComponent<TMP_Text>().text = kvp.Key.ToString();
            buyButton.GetComponent<TowerBuyButtonTypeHolder>().type = kvp.Key;
            buyButton.GetComponent<Button>().onClick.AddListener(() =>
            {
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
            });
            buyButtons.Add(buyButton);
        }
    }
}
