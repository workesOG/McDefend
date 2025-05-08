using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PopUpManager : MonoBehaviour
{
    public GameObject _popUpPrefab;
    public GameObject currentPopUp;
    public Transform canvas;

    public static PopUpManager instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void CreatePopUp(PopUpData data)
    {
        Time.timeScale = 0;

        currentPopUp = Instantiate(_popUpPrefab, canvas);
        currentPopUp.transform.Find("Pop-Up Window/Title").GetComponent<TMP_Text>().text = data.title;
        currentPopUp.transform.Find("Pop-Up Window/Content").GetComponent<TMP_Text>().text = data.content;

        currentPopUp.transform.Find("Pop-Up Window/Close").GetComponent<Button>().onClick.AddListener(ClosePopUp);
        currentPopUp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 0);
        currentPopUp.transform.SetAsLastSibling();
    }

    public void ClosePopUp()
    {
        Debug.Log("Closing pop-up");
        Time.timeScale = 1;
        Destroy(currentPopUp);
    }
}

public class PopUpData
{
    public string title;
    public string content;
}

