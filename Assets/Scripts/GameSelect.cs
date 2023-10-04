using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;
using TMPro;

interface GameFamilyNames
{
    static string DollHouseObject = "DollhouseFamily";
    static string DartsObject = "DartsGameFamily";
    static string WinePourObject = "WinePourFamily";
}

public class GameSelect : MonoBehaviour
{
    public int currGameCount = 2;
    public bool skiptoGame;
    [Serializable]public class SkipToggle
    {
        public string name;
        public bool play;
    }
    public SkipToggle[] toggleArr = new SkipToggle[2];
    static MonoBehaviour selfInstance;//for Coroutine

    int currGameIndex;
    [SerializeField] public UnityEvent[] gameStartEventArr;
    public GameObject[] gameObjectArr;
    public UnityEvent[] gameResetEventArr;
    public Sprite[] gameSprArr;
    public string[] gameTaglineArr;
    int gamelistLength;
    
    //Cart
    public SpriteRenderer gameCart;
    Vector2 cartInitPos;
    float cartLowerSpeed = -2.5f;

    //Screen
    public TextMeshProUGUI tagLineText;
    public GameObject selectScreenObject;
    public GameObject returningText;
    bool isLoading;

    //Start and Closing Game
    public static float GameStartTime { get; private set; } = 1.4f;
    public static float GameCloseTime { get; private set; } = 3.2f; //Time to close after winning a game
    static bool isGaming;

    // Start is called before the first frame update
    void Start()
    {
        cartInitPos = gameCart.transform.position;
        gamelistLength = gameStartEventArr.Length;
        if (skiptoGame)
        {
            for(int _gameIndex = 0; _gameIndex < toggleArr.Length;_gameIndex++)
            {
                if (toggleArr[_gameIndex].play)
                {
                    currGameIndex = _gameIndex;
                    StartLoad(_gameIndex);
                    gameCart.sprite = gameSprArr[_gameIndex];
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLoading)
        {
            if (!isGaming)
            {
                if (Input.GetKeyDown(KeyCode.LeftArrow))
                {
                    SelectLastGame();
                }
                else if (Input.GetKeyDown(KeyCode.RightArrow))
                {
                    SelectNextGame();
                }
                else if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.DownArrow))
                {
                    StartLoad(currGameIndex);
                }
                else if (Input.GetKeyDown(KeyCode.R))
                {
                    currGameIndex = UnityEngine.Random.Range(0, currGameCount);
                    gameCart.sprite = gameSprArr[currGameIndex];
                    StartLoad(currGameIndex);
                }
                gameCart.sprite = gameSprArr[currGameIndex];
            }
        }
        else
            gameCart.transform.position += new Vector3(0f, cartLowerSpeed, 0f)*Time.deltaTime;
    }

    //loading Games
    public void StartLoad()
    {
        if (!isLoading)
        {
            StartCoroutine(LoadGame());
            isLoading = true;
        }
    }
    public void StartLoad(int gameIndex)
    {
        if (!isLoading)
        {
            StartCoroutine(LoadGame(gameIndex));
            isLoading = true;
        }
    }

    IEnumerator LoadGame()
    {
        StartCoroutine(LoadGame(currGameIndex));
        yield return new WaitForFixedUpdate(); //just end it?
    }
    IEnumerator LoadGame(int gameIndex)
    {
        tagLineText.text = gameTaglineArr[currGameIndex];
        tagLineText.gameObject.SetActive(true);
        yield return new WaitForSeconds(GameStartTime);

        tagLineText.gameObject.SetActive(false);

        gameObjectArr[gameIndex].SetActive(true);
        gameStartEventArr[gameIndex].Invoke();
        isGaming = true;
        gameCart.transform.position = cartInitPos;
        isLoading = false;
        selectScreenObject.SetActive(false);

        isGaming = true;

        TimerBehavior.StartTimer();
    }

    //Menu
    public void SelectNextGame()
    {
        if (currGameIndex < gamelistLength-1)
            currGameIndex += 1;
        else
            currGameIndex = 0;
    }
    public void SelectLastGame()
    {
        if (currGameIndex > 0)
            currGameIndex -= 1;
        else
            currGameIndex = gamelistLength - 1;
    }

    //in Game

    public void QuitGame()
    {
        StartCoroutine(DoQuitGame());
    }
    IEnumerator DoQuitGame()
    {
        TimerBehavior.EndTimer();
        gameResetEventArr[currGameIndex].Invoke();
        returningText.SetActive(true);
        yield return new WaitForSeconds(GameCloseTime);

        returningText.SetActive(false);
        gameObjectArr[currGameIndex].SetActive(false);
        selectScreenObject.SetActive(true);

        gameCart.transform.position = cartInitPos;
        isGaming = false;
    }
    public static void SetNotGaming()
    {
        isGaming = false;
    }
}
