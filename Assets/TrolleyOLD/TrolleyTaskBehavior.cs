using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.Playables;
using TMPro;

namespace RitualNight
{
    namespace PartyGames
    {
        public class TrolleyTaskBehavior : MonoBehaviour//TaskBehavior
        {
            public bool HasWon;
            public PlayerController_TR PlayerController;
            [SerializeField] private Camera mainCam;
            private Vector3 mainCamInitPos;
            private float mainCamInitSize;

            [SerializeField] private float numberChance;

            [SerializeField] private GameObject numberCanvasObj;
            [SerializeField] private TextMeshProUGUI numberText;
            [SerializeField] private bool isNumber;

            [SerializeField] private RuntimeAnimatorController animController;
            private Animator _camAnim;
            private bool _alreadyHasAnimComp;

            [SerializeField] private PlayableDirector timelineDirector;

            [SerializeField] private GameObject maskObj;

            [SerializeField] private Animator carAnim;
            [SerializeField] private SpriteRenderer carSprRend;
            [SerializeField] private Sprite carInitSpr;

            [Header("Win")]
            [SerializeField] private TrackItemSets[] trackItemsSetsArr;
            private TrackItemSets _selectedSet;
            public int CorrectTrack;
            [System.Serializable]
            public class TrackItemSets
            {
                public string name;
                public Sprite[] ItemSprArr; //init in inspector
                public Sprite[] EvilItemSprArr; //init in inspector
            }

            public bool IsCamFollow;
            public bool IsCamReturn;

            [SerializeField] private SpriteRenderer[] trackItemRendArr;
            [SerializeField] private GameObject bubbleObj;
            [SerializeField] private SpriteRenderer bubbleItemRend;

            [Header("Tracks")]
            [SerializeField] private SpriteRenderer[] changeTrackRendArr; //0down 1mid 2up
            [SerializeField] private Sprite[] changeTrackSprArr; 
            public int[] ChangeTrackResultArr;
            [SerializeField] private float switchOneChance;
            [SerializeField] private float switchTwoChance;
            private bool _isHardPattern;

            [Header ("MiniManager")]
            public TrolleyMiniManager TRMM;

            public void Open() //override 
            {
                //PlayerController.gameObject.SetActive(true);
            }
            public void StartOpen() //override
            {
                mainCamInitPos = new Vector3(0, 0, -10);//mainCam.transform.position;
                mainCamInitSize = 5;//mainCam.orthographicSize;
                RestartGame();
                //PlayerController.StartSetUp();
                //HasWon = false;
                //base.StartOpen();
            }
            private void RestartGame()
            {
                numberCanvasObj.SetActive(false);
                isNumber = false;

                bubbleObj.SetActive(true);
                bubbleItemRend.enabled = true;
                IsCamFollow = false;
                IsCamReturn = false;
                PlayerController.StartGame();

                timelineDirector.Stop();
                timelineDirector.time = 0;
                timelineDirector.Play();
                //timelineDirector.Evaluate();

                carSprRend.sprite = carInitSpr;

                Animator _anim = mainCam.gameObject.GetComponent<Animator>();
                if (_anim != null)
                {
                    _alreadyHasAnimComp = true;
                    _camAnim = _anim;
                }
                else
                {
                    _camAnim = mainCam.gameObject.AddComponent<Animator>();
                }
                _camAnim.runtimeAnimatorController = animController;
                _camAnim.enabled = true;
                _camAnim.SetTrigger("Restart");
                ChangeTrackResultArr = new int[3];
                ChooseItemPattern();
                RandomizeSecondTracks();
            }
            private void RandomizeSecondTracks()
            {
                _isHardPattern = false;

                foreach (SpriteRenderer _sprRend in changeTrackRendArr)
                {
                    _sprRend.sprite = changeTrackSprArr[2];
                    _sprRend.color = Color.white;
                }
                for (int i = 0; i < 3; i++)
                {
                    ChangeTrackResultArr[i] = 0;
                }
                float _chance = Random.Range(0f, 1f);
                if (_chance < switchOneChance)
                {
                    //(0,1,2) position of the change track
                    int _firstChangeTrack = Random.Range(0, 3);

                    //(1,0,-1) minus one
                    int _trackIndexOffset = -(_firstChangeTrack - 1);

                    //(-1,0,1) + offset =(-2,-1,0,1,2):change index of track, also used to play animation
                    //when offset is 1 = start index 0 = (0,1,2)
                    int trackChangeIndex = Random.Range(-1, 2) + _trackIndexOffset;

                    //(-2,-1,0,1,2) +2 into usable index (0,1,2,3,4)
                    changeTrackRendArr[_firstChangeTrack].sprite = changeTrackSprArr[trackChangeIndex + 2];

                    int _firstChangeTrackEnd = _firstChangeTrack + trackChangeIndex;
                    ChangeTrackResultArr[_firstChangeTrack] = _firstChangeTrackEnd- _firstChangeTrack;

                    if (_chance < switchTwoChance)
                    {
                        _isHardPattern = true;
                        int _nextChangeTrack = (_firstChangeTrack + 1) % 3;
                        int _nextChangeTrackEnd = 0;
                        int _fail = 0;
                        while (_nextChangeTrackEnd == _firstChangeTrackEnd || _nextChangeTrackEnd == _nextChangeTrack)
                        {
                            _fail++;
                            if (_fail > 100)
                            {
                                return;
                            }
                            _nextChangeTrackEnd = (_nextChangeTrackEnd + 1) % 3;
                        }
                        ChangeTrackResultArr[_nextChangeTrack] = _nextChangeTrackEnd- _nextChangeTrack;
                        changeTrackRendArr[_nextChangeTrack].sprite = changeTrackSprArr[_nextChangeTrackEnd - _nextChangeTrack+2];
                        
                        int _lastChangeTrack = (_nextChangeTrack + 1) % 3;
                        int _lastChangeTrackEnd = 0;
                        _fail = 0;
                        while (_lastChangeTrackEnd == _firstChangeTrackEnd || _lastChangeTrackEnd == _nextChangeTrackEnd)
                        {
                            _fail++;
                            if (_fail > 100)
                            {
                                return;
                            }
                            _lastChangeTrackEnd = (_lastChangeTrackEnd + 1) % 3;
                        }
                        ChangeTrackResultArr[_lastChangeTrack] = _lastChangeTrackEnd- _lastChangeTrack;
                        changeTrackRendArr[_lastChangeTrack].sprite = changeTrackSprArr[_lastChangeTrackEnd - _lastChangeTrack + 2];
                    }
                    else//Reverse the first switch
                    {
                        changeTrackRendArr[_firstChangeTrackEnd].sprite = changeTrackSprArr[(-trackChangeIndex) + 2];
                        ChangeTrackResultArr[_firstChangeTrackEnd] = _firstChangeTrack- _firstChangeTrackEnd;
                    }
                }
                else 
                {
                    foreach (SpriteRenderer _sprRend in changeTrackRendArr)
                    {
                        _sprRend.sprite = changeTrackSprArr[2];
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        ChangeTrackResultArr[i] = 0;
                    }
                }
            }

            private void ChooseItemPattern()
            {
                //select a set
                float _chance = Random.Range(0f, 1f);
                if (_chance < numberChance)
                {
                    _selectedSet = trackItemsSetsArr[0];
                    ChooseNumberItemPattern();
                    return;
                }
                else
                {
                    _selectedSet = trackItemsSetsArr[Random.Range(1, trackItemsSetsArr.Length)];
                }


                    int _evilLength = _selectedSet.EvilItemSprArr.Length;
                int _itemLength = _selectedSet.ItemSprArr.Length;
                int _totalLength = _evilLength + _itemLength;

                //evil item is always in front: {evil, evil, evil, good, good}
                int[] _itemIndexArr = new int[_totalLength]; 
                for (int i = 0; i < _totalLength; i++)
                {
                    _itemIndexArr[i] = i;
                }

                bool _hasGoodItem=false;
                List<int> _goodIndexList = new List<int>();
                for (int i = 0; i < 3; i++)//for each track
                {
                    int _chosenIndex = _itemIndexArr[Random.Range(0, _totalLength)];
                    int fail = 0;
                    while (_chosenIndex == -1 )
                    {
                        fail++;
                        if (fail > 100)
                        {
                            print("while loop");
                            print("chInd:" + _chosenIndex);
                            print("i:" + i);
                            print("goodItem:" + _hasGoodItem);
                            print(_itemIndexArr);
                            break;
                        }
                        if (i == 2 && !_hasGoodItem)
                        {
                            _chosenIndex = _itemIndexArr[Random.Range(_evilLength, _totalLength)];
                        }
                        else
                        {
                            _chosenIndex = _itemIndexArr[Random.Range(0, _totalLength)];
                        }
                    } 

                    if (_chosenIndex < _evilLength)
                    {
                        //spawn evil
                        trackItemRendArr[i].sprite = _selectedSet.EvilItemSprArr[_chosenIndex];
                        trackItemRendArr[i].color = Color.white;
                        _itemIndexArr[_chosenIndex] = -1;
                    }
                    else
                    {
                        trackItemRendArr[i].sprite = _selectedSet.ItemSprArr[_chosenIndex-_evilLength];
                        trackItemRendArr[i].color = Color.white;
                        _itemIndexArr[_chosenIndex] = -1;
                        _hasGoodItem = true;
                        _goodIndexList.Add(i); //add track index that has good item
                    }
                }
                if (_goodIndexList.Count == 0)
                {
                    print("no good Item");
                }
                CorrectTrack = _goodIndexList[Random.Range(0, _goodIndexList.Count)];
                bubbleItemRend.sprite = trackItemRendArr[CorrectTrack].sprite;
            }
            private void ChooseNumberItemPattern()
            {
                //This is 
                int _baseNumber = 9;
                numberCanvasObj.SetActive(true);

                isNumber = true;
                bubbleItemRend.enabled = false;

                int _itemLength = _selectedSet.ItemSprArr.Length;
                int[] _itemIndexArr = new int[_itemLength]; //change to array?
                for (int i = 0; i < _itemLength; i++)
                {
                    _itemIndexArr[i] = i;
                }

                int _chosenIndex = _itemIndexArr[Random.Range(0, _itemLength)];
                CorrectTrack = Random.Range(0, 3);
                int _correctNumber = 0;

                for (int i = 0; i < 3; i++)//for each track
                {
                    _chosenIndex = _itemIndexArr[Random.Range(0,_itemLength)];
                    int fail = 0;
                    while (_chosenIndex == -1)
                    {
                        fail++;
                        if (fail > 100)
                        {
                            print("while loop");
                            break;
                        }
                        _chosenIndex = _itemIndexArr[Random.Range(0, _itemLength)];
                    }
                    trackItemRendArr[i].sprite = _selectedSet.ItemSprArr[_chosenIndex];
                    trackItemRendArr[i].color = Color.white;
                    _itemIndexArr[_chosenIndex] = -1;
                    if (i == CorrectTrack)
                    {
                        _correctNumber = _chosenIndex + _baseNumber;
                    }
                }
                int _offsetNumber = Random.Range(-2, 3);
                int _firstNumber = _correctNumber / 2;
                int _secondNumber = _correctNumber - _firstNumber;

                _firstNumber += _offsetNumber;
                _secondNumber -= _offsetNumber;
                numberText.text = _firstNumber + "+" + _secondNumber;
            }
            private void Update()
            {
                if (IsCamReturn)
                {
                    mainCam.orthographicSize = Mathf.Lerp(mainCam.orthographicSize, mainCamInitSize, 0.5f);
                    if (Mathf.Abs(mainCam.orthographicSize - mainCamInitSize) < 0.01f)
                    {
                        mainCam.orthographicSize = mainCamInitSize;
                    }
                    mainCam.transform.position = Vector3.Lerp(mainCam.transform.position, mainCamInitPos, 0.5f);
                    if (Vector3.Distance(mainCam.transform.position, mainCamInitPos) < 0.01f)
                    {
                        mainCam.transform.position = mainCamInitPos;
                    }
                }
            }
            private void LateUpdate()
            {
                float _initSize = 2.1f;
                float _size = _initSize * ((mainCam.orthographicSize) / 5);
                maskObj.transform.localScale = new Vector3(_size, _size, _size);

                if (IsCamFollow)
                {
                    _camAnim.enabled = false;
                    mainCam.transform.position = new Vector3(carAnim.gameObject.transform.position.x, carAnim.gameObject.transform.position.y, mainCam.transform.position.z);
                }
                maskObj.transform.position = mainCam.transform.position + new Vector3(0, 0, 1);
                if (Input.GetKeyDown(KeyCode.R))
                {
                    StartOpen();
                }
            }
            private IEnumerator DoFastFinishTask()
            {
                //SetPartyGameResult(true);
                yield return new WaitForSeconds(1);
                _camAnim.SetTrigger("Restart");
                TRMM.SetWin();
                //SetStateClosing();
                StartClose();
            }
            private IEnumerator DoFinishTask()
            {

                //SetPartyGameResult(true);
                yield return new WaitForSeconds(3.5f);
                _camAnim.SetTrigger("Restart");
                TRMM.SetWin();
                //SetStateClosing();
                StartClose();
            }
            private IEnumerator DoFailedTask()
            {
                yield return new WaitForSeconds(2.5f);
                _camAnim.SetTrigger("Restart");
                carAnim.SetTrigger("Restart");
                RestartGame();
            }
            public void StartClose() //override 
            {
                if (!_alreadyHasAnimComp)
                {
                    Destroy(_camAnim);
                }
                StopAllCoroutines();
                //PlayerController.CloseGame();
                //PlayerController.gameObject.SetActive(false);

                //base.StartClose();
            }
            public void MunchItem()
            {
                trackItemRendArr[ChangeTrackResultArr[PlayerController.SelectedTrack] + PlayerController.SelectedTrack].color = new Color(1, 1, 1, 0);
                numberCanvasObj.SetActive(false);
                bubbleObj.SetActive(false);
            }
            public void WinCheck()
            {
                IsCamFollow = false;
                IsCamReturn = true;
                if (ChangeTrackResultArr[PlayerController.SelectedTrack]+PlayerController.SelectedTrack == CorrectTrack)
                {
                    HasWon = true;
                    carAnim.SetBool("isMunching", false);
                    carAnim.SetBool("isWin", true);
                    if (_isHardPattern)
                    {
                        StartCoroutine(DoFastFinishTask());
                    }
                    else
                    {
                        StartCoroutine(DoFinishTask());
                    }
                }
                else
                {
                    bubbleObj.SetActive(true);
                    if (isNumber)
                    {
                        numberCanvasObj.SetActive(true);
                    }
                    else
                    {
                        bubbleItemRend.enabled = true;
                    }
                    carAnim.SetBool("isMunching", false);
                    carAnim.SetBool("isWin", false);
                    StartCoroutine(DoFailedTask());
                }
            }
        }
    }
}