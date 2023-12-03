using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    READY,
    PLAYING,
    GAMEOVER
}

public class GameManager : MonoBehaviourPunCallbacks
{
    private static GameManager _instance;
    public static GameManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null)
                {
                    GameObject gameManager = new GameObject("GameManager");
                    _instance = gameManager.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }

    private PhotonView photonView;
    [SerializeField] private SpriteRenderer boardRenderer;
    private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();

    private const int GRID_SIZE = 15;
    private GameState currentState { get; set; }
    private Vector2[,] gridPosition { get; set; }   //격자
    private GameObject[,] stoneObjects { get; set; }   //바둑알 추적
    private int[,] placedStones { get; set; }   //바둑알 플레이어 번호 저장
    private int currentPlayerTurn { get; set; }   //턴
    public int player { get; set; }


    private void Awake()
    {
        photonView = GetComponent<PhotonView>();

        InitializeGrid();
        InitializeGame();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Initialize method
    private void InitializeGrid()
    {
        gridPosition = new Vector2[GRID_SIZE, GRID_SIZE];
        stoneObjects = new GameObject[GRID_SIZE, GRID_SIZE];
        placedStones = new int[GRID_SIZE, GRID_SIZE];

        //바둑판 이미지 측정
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;

        //각 격자 사이의 간격 계산
        float cellWidth = boardWidth / GRID_SIZE;
        float cellHeight = boardHeight / GRID_SIZE;

        Vector3 boardPosition = boardRenderer.transform.position;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                float posX = boardPosition.x + (i * cellWidth) - (boardWidth / 2) + (cellWidth / 2);
                float posY = boardPosition.y + (j * cellHeight) - (boardHeight / 2) + (cellHeight / 2);
                gridPosition[i, j] = new Vector2(posX, posY);
            }
        }
    }

    private void InitializeGame()
    {
        SetReadyState();

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                if (stoneObjects[i, j] != null)
                {
                    Destroy(stoneObjects[i, j]);
                    stoneObjects[i, j] = null;
                }
                placedStones[i, j] = 0;
            }
        }
    }

    public void InitializePlayerReadyStatus()
    {
        playerReadyStatus.Clear();
        foreach (var player in PhotonNetwork.CurrentRoom.Players)
        {
            playerReadyStatus[player.Key] = false;
        }
    }
    #endregion

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            PlaceStoneAtMousePosition();
        }
    }

    #region Game ReadyState
    /// <summary>
    /// 플레이어가 모두 준비가 되었는지 확인한다.
    /// 방장은 게임 시작이 가능하다.
    /// </summary>
    public void OnPlayerReady()
    {
        int localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
        playerReadyStatus[localPlayerID] = true;
        photonView.RPC("PlayerReady", RpcTarget.All, localPlayerID);

        if (PhotonNetwork.IsMasterClient)
            CheckAllPlayersReady();
    }

    private void CheckAllPlayersReady()
    {
        foreach (var player in playerReadyStatus)
        {
            if (!player.Value)
                return;
        }

        if (PhotonNetwork.IsMasterClient)
            StartGame();
    }

    private void ResetAllPlayersScoreUpdateStatus()
    {
        foreach (var playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            playerInfo.ResetScoreUpdateStatus();
        }
    }
    #endregion

    #region Game PlayingState
    /// <summary>
    /// 게임중 상태를 알림
    /// </summary>
    private void StartGame()
    {
        SetPlayingState();
        photonView.RPC("SetGameState", RpcTarget.All, GameState.PLAYING);
        currentPlayerTurn = 1;
    }

    /// <summary>
    /// 격자선에 정확히 맞추지 않아도 근처로 배정해주는 메서드
    /// 클릭한 위치의 가장 가까운 격자에 바둑알 생성
    /// 턴 변경
    /// </summary>
    private void PlaceStoneAtMousePosition()
    {
        if (!IsGamePlaying() || player != currentPlayerTurn)
            return;

        int closestX;
        int closestY;

        FindClosestGridPosition(out closestX, out closestY);
        TryPlaceStoneAndChagneTurn(closestX, closestY);
    }

    private void FindClosestGridPosition(out int closestX, out int closestY)
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        closestX = -1;
        closestY = -1;
        float closetDistance = Mathf.Infinity;

        for (int i = 0; i < GRID_SIZE; i++)
        {
            for (int j = 0; j < GRID_SIZE; j++)
            {
                float distance = Vector2.Distance(mouseWorldPosition, gridPosition[i, j]);

                if (distance < closetDistance)
                {
                    closestX = i;
                    closestY = j;
                    closetDistance = distance;
                }
            }
        }
    }

    private void TryPlaceStoneAndChagneTurn(int closestX, int closestY)
    {
        if (placedStones[closestX, closestY] == 0)
        {
            photonView.RPC("PlaceStone", RpcTarget.All, closestX, closestY, player);
            ChangeTurn();
        }
    }

    private void ChangeTurn()
    {
        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
        photonView.RPC("UpdateCurrentTurn", RpcTarget.All, currentPlayerTurn);

        Debug.Log($"지금은 {currentPlayerTurn}의 턴입니다.");
    }
    #endregion

    #region 게임 오버 상태
    /// <summary>
    /// 승리 조건을 충족해 GameOver 상태 전환
    /// </summary>
    private void SetGameOver()
    {
        SetGameOverState();
        photonView.RPC("SetGameState", RpcTarget.All, GameState.GAMEOVER);
        StartCoroutine(RestartGameDelay(2.5f));
    }

    private IEnumerator RestartGameDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        InitializePlayerReadyStatus();
        InitializeGame();
        SetReadyState();
    }
    #endregion

    #region Game Rules
    /// <summary>
    /// 오목 승리 조건
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    private bool CheckHorizontalWin(int x, int y, int playerNumber)
    {
        //현재 놓인 돌을 카운트의 시작점
        int count = 1;

        for (int i = x - 1; i >= 0 && placedStones[i, y] == playerNumber; i--)
            count++;

        for (int i = x + 1; i < GRID_SIZE && placedStones[i, y] == playerNumber; i++)
            count++;

        return count >= 5;

    }
    private bool CheckVerticalWin(int x, int y, int playerNumber)
    {
        int count = 1;

        for (int j = y - 1; j >= 0 && placedStones[x, j] == playerNumber; j--)
            count++;

        for (int j = y + 1; j < GRID_SIZE && placedStones[x, j] == playerNumber; j++)
            count++;

        return count >= 5;
    }

    private bool CheckDiagonalWin(int x, int y, int playerNumber)
    {
        int countUp = 1;
        int countDown = 1;

        // 대각선 [/] 모양 검사
        for (int i = x - 1, j = y - 1; i >= 0 && j >= 0 && placedStones[i, j] == playerNumber; i--, j--)
            countDown++;

        for (int i = x + 1, j = y + 1; i < GRID_SIZE && j < GRID_SIZE && placedStones[i, j] == playerNumber; i++, j++)
            countDown++;

        // 대각선 [\] 모양 검사
        for (int i = x + 1, j = y - 1; i < GRID_SIZE && j >= 0 && placedStones[i, j] == playerNumber; i++, j--)
            countUp++;

        for (int i = x - 1, j = y + 1; i >= 0 && j < GRID_SIZE && placedStones[i, j] == playerNumber; i--, j++)
            countUp++;

        return countDown >= 5 || countUp >= 5;
    }

    #endregion

    #region PunRPC
    /// <summary>
    /// 포톤 네트워크
    /// 현재 상태를 모든 플레이어에게 알림
    /// </summary>
    /// <param name="state"></param>
    [PunRPC] public void SetGameState(GameState state) => currentState = state;
    [PunRPC] public void UpdateCurrentTurn(int newTurn) => currentPlayerTurn = newTurn;

    [PunRPC]
    public void PlayerReady(int playerID)
    {
        playerReadyStatus[playerID] = true;
        Debug.Log($"Player {playerID} is ready");

        if (PhotonNetwork.IsMasterClient)
            CheckAllPlayersReady();
    }

    [PunRPC]
    public void PlaceStone(int x, int y, int playerNumber)
    {
        string stonePath = playerNumber == 1 ? "Prefabs/Ingame_Slime" : "Prefabs/Ingame_Yeti";
        GameObject stonePrefab = Resources.Load<GameObject>(stonePath);
        GameObject newStone = Instantiate(stonePrefab, gridPosition[x, y], Quaternion.identity);
        placedStones[x, y] = playerNumber;
        stoneObjects[x, y] = newStone;

        if (CheckHorizontalWin(x, y, playerNumber) || CheckVerticalWin(x, y, playerNumber) || CheckDiagonalWin(x, y, playerNumber))
        {
            photonView.RPC("UpdateScoreRPC", RpcTarget.All, playerNumber);
            Debug.Log($"Player {playerNumber} wins!");
            SetGameOver();
        }
    }

    [PunRPC]
    public void UpdateScoreRPC(int winningPlayer)
    {
        foreach (var playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            playerInfo.UpdateScoreRecord(winningPlayer);
        }
    }
    #endregion

    #region State
    private void SetReadyState()
    {
        currentState = GameState.READY;
        ResetAllPlayersScoreUpdateStatus();
    }

    private void SetPlayingState() => currentState = GameState.PLAYING;
    private void SetGameOverState() => currentState = GameState.GAMEOVER;

    public bool IsGameReady()
    {
        return currentState == GameState.READY;
    }

    public bool IsGamePlaying()
    {
        return currentState == GameState.PLAYING;
    }

    public bool IsGameOver()
    {
        return currentState == GameState.GAMEOVER;
    }

    #endregion

    #region InGameConsole
    public void CheckPlayer2Turn()
    {
        if (currentPlayerTurn == 2)
        {
            // 인게임 콘솔의 인스턴스를 찾고 CheckPlayer2CanPlaceStone 메서드를 호출
            InGameConsole console = FindObjectOfType<InGameConsole>();
            if (console != null)
                console.CheckPlayer2CanPlaceStone(true);
        }
        else
        {
            InGameConsole console = FindObjectOfType<InGameConsole>();
            if (console != null)
                console.CheckPlayer2CanPlaceStone(false);
        }
    }
    #endregion
}