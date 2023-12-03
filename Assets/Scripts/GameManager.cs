using Photon.Pun;
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
            if (_instance == null )
            {
                _instance = FindObjectOfType<GameManager>();

                if (_instance == null )
                {
                    GameObject gameManager = new GameObject("GameManager");
                    _instance = gameManager.AddComponent<GameManager>();
                }
            }

            return _instance;
        }
    }


    private GameState currentState;
    public GameState CurrentState { get; set; }

    private static int GRID_SIZE = 15;

    private PhotonView photonView;

    [SerializeField] private SpriteRenderer boardRenderer;
    private Vector2[,] gridPosition;        //격자

    private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();
    private GameObject[,] placedStones;     //돌 추적
    public int player { get; set; }
    private int currentPlayerTurn;


    private void Awake()
    {
        InitializeGrid();
        photonView = GetComponent<PhotonView>();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    #region Init
    private void InitializeGrid()
    {
        gridPosition = new Vector2[GRID_SIZE, GRID_SIZE];

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

        placedStones = new GameObject[GRID_SIZE, GRID_SIZE];
    }

    private void OnDrawGizmos()
    {
        if (gridPosition != null)
        {
            Gizmos.color = Color.red;

            int gridSize = gridPosition.GetLength(0);
            for (int i = 0; i < gridSize; i++)
            {
                for (int j = 0; j < gridSize; j++)
                {
                    Gizmos.DrawSphere(new Vector3(gridPosition[i, j].x, gridPosition[i, j].y, 0), 0.1f);
                }
            }
        }
    }

    private void InitializeGame()
    {
        for (int i = 0; i < placedStones.GetLength(0); i++)
        {
            for (int j = 0; j < placedStones.GetLength(1); j++)
            {
                if (placedStones[i, j] != null)
                {
                    Destroy(placedStones[i, j]);
                    placedStones[i, j] = null;
                }
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



    #region 게임 준비 상태 
    public void SetReadyState()
    {
        currentState = GameState.READY;
        InitializeGame();
    }

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
    #endregion

    #region 게임 진행 상태
    private void StartGame()
    {
        currentState = GameState.PLAYING;
        photonView.RPC("SetGameState", RpcTarget.All, GameState.PLAYING);
        currentPlayerTurn = 1;
    }

    public bool IsGamePlaying()
    {
        return currentState == GameState.PLAYING;
    }



    private void PlaceStoneAtMousePosition()
    {
        if (currentState != GameState.PLAYING || player != currentPlayerTurn)
            return;

        int closestX;
        int closestY;

        FindClosestGridPosition(out closestX, out closestY);

        TryPlaceStoneAndChagneTurn(closestX, closestY);
    }

    private void TryPlaceStoneAndChagneTurn(int closestX, int closestY)
    {
        if (placedStones[closestX, closestY] == null)
        {
            photonView.RPC("PlaceStone", RpcTarget.All, closestX, closestY, player);
            ChangeTurn();
        }
    }

    private void FindClosestGridPosition(out int closestX, out int closestY)
    {
        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        closestX = -1;
        closestY = -1;
        float closetDistance = Mathf.Infinity;

        for (int i = 0; i < 15; i++)
        {
            for (int j = 0; j < 15; j++)
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

    private void ChangeTurn()
    {
        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
        photonView.RPC("UpdateCurrentTurn", RpcTarget.All, currentPlayerTurn);

        Debug.Log($"지금은 {currentPlayerTurn}의 턴입니다.");
    }
    #endregion

    #region PunRPC

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
        placedStones[x, y] = newStone;
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

