using Photon.Pun;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviourPun
{
    private static UIManager _instance;
    public static UIManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<UIManager>();

                if (_instance == null)
                {
                    GameObject uiManager = new GameObject("UIManager");
                    _instance = uiManager.AddComponent<UIManager>();
                }
            }

            return _instance;
        }
    }

    private Texture2D cursorDefault;
    private Texture2D cursorClick;
    [SerializeField] private Transform matchResultPosition;
    [SerializeField] private GameObject selection;
    [SerializeField] private GameObject startButton01;
    [SerializeField] private GameObject startButton02;

    [SerializeField] private float setTime = 60f;
    [SerializeField] private TMP_Text countdownText;
    [SerializeField] private TMP_Text guideText;

    #region Init
    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

    }
    private void Start()
    {
        InitSundry();
        InitCursor();
        SetCustomCursor(cursorDefault);
    }

    private void InitSundry()
    {
        selection.SetActive(false);
        startButton02.SetActive(false);
        countdownText.text = setTime.ToString();
    }
    /// <summary>
    /// ���콺 Ŀ�� ����
    /// </summary>
    private void InitCursor()
    {
        cursorDefault = Resources.Load<Texture2D>("Arts/Button_Mouse");
        cursorClick = Resources.Load<Texture2D>("Arts/Button_MouseClick");
    }

    private void SetCustomCursor(Texture2D cursorTexture) => Cursor.SetCursor(cursorTexture, Vector2.zero, CursorMode.Auto);
    #endregion

    private void Update()
    {
        if (GameManager.Instance.IsGamePlaying())
            UpdateTimerText();

        if (Input.GetMouseButtonDown(0))
            SetCustomCursor(cursorClick);

        else if (Input.GetMouseButtonUp(0))
            SetCustomCursor(cursorDefault);
    }

    /// <summary>
    /// ���� ���� ��ư
    /// </summary>
    public void OnReadyButtonClicked()
    {
        if (GameManager.Instance != null)
        {
            if (!GameManager.Instance.IsGamePlaying())
                GameManager.Instance.OnPlayerReady();
            else
                Debug.LogError("Can`t Leave the room while the game is in PLYAING state");

            ToggleStartButtons();
            SoundManager.Instance.PlayButtonAndClickSound();
        }
    }

    public void ToggleStartButtons()
    {
        startButton01.SetActive(!startButton01.activeSelf);
        startButton02.SetActive(!startButton02.activeSelf);
    }

    public void OnClickLeaveRoom()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGamePlaying())
            PhotonNetwork.LeaveRoom();

        else
            Debug.LogError("Can`t Leave the room while the game is in PLYAING state");

        SoundManager.Instance.PlayButtonAndClickSound();
    }

    /// <summary>
    /// �� ���� �г� ���
    /// </summary>
    public void OnClickCreateRoom() => selection.SetActive(true);
    public void OnClickCancelRoom()
    {
        selection.SetActive(false);
        SoundManager.Instance.PlayButtonAndClickSound();
    }

    /// <summary>
    /// Ÿ�̸�
    /// </summary>
    private void UpdateTimerText()
    {
        if (setTime > 0f)
        {
            setTime -= Time.deltaTime;
            countdownText.text = $"�����ð�: {Mathf.Round(setTime)}��";
        }

        else if (setTime <= 0f)
        {
            Time.timeScale = 0f;
            countdownText.text = $"�ð� ����!";
        }
    }

    /// <summary>
    /// ���� �¸� �� �й� �̹��� ǥ�� ����
    /// </summary>
    /// <param name="winningPlayer"></param>
    [PunRPC]
    public void UpdateResultPanelRPC(int winningPlayer)
    {
        int localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
        string resultToLoad = (localPlayerID == winningPlayer) ? "Prefabs/UI_Ingame_Win" : "Prefabs/UI_Ingame_Loose";
        GameObject resultPrefab = Resources.Load<GameObject>(resultToLoad);
        Instantiate(resultPrefab, matchResultPosition.transform);
    }

    /// <summary>
    /// ���� �¸� �� �й� �̹��� ǥ�� ����
    /// </summary>
    [PunRPC]
    public void RemoveResultPanelRPC()
    {
        GameObject matchResultParent = GameObject.Find("UI_MatchResult");
        if (matchResultParent != null)
        {
            foreach (Transform child in matchResultParent.transform)
                Destroy(child.gameObject);
        }
    }

    [PunRPC]
    public void ResetTimerRPC(float newTime)
    {
        setTime = newTime;
        UpdateTimerText();
    }

    [PunRPC] public void UpdateTurnText(string playerName) => guideText.text = $"������ {playerName}���� �����Դϴ�.";
}
