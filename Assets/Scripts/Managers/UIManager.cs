using Photon.Pun;
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
    
    [SerializeField] private Transform matchResultPosition;

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

    /// <summary>
    /// 게임 시작 버튼
    /// </summary>
    public void OnReadyButtonClicked()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGamePlaying())
            GameManager.Instance.OnPlayerReady();

        else
            Debug.LogError("Can`t Change ready state while the game is is PLAYING state");
    }

    /// <summary>
    /// 게임 나가기 버튼
    /// </summary>
    public void OnClickLeaveRoom()
    {
        if (GameManager.Instance != null && !GameManager.Instance.IsGamePlaying())
            PhotonNetwork.LeaveRoom();

        else
            Debug.LogError("Can`t Leave the room while the game is in PLYAING state");
    }

    /// <summary>
    /// 게임 승리 및 패배 이미지 표시 생성
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
    /// 게임 승리 및 패배 이미지 표시 제거
    /// </summary>
    [PunRPC]
    public void RemoveResultPanelRPC()
    {
        Debug.Log("RemoveResultPanelRPC called");

        GameObject matchResultParent = GameObject.Find("UI_MatchResult");
        if (matchResultParent != null)
        {
            foreach (Transform child in matchResultParent.transform)
                Destroy(child.gameObject);
        }
    }
}
