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
    static private GameManager _instance = null;
    static public GameManager GetInstance() { return _instance; }

    private GameState _currentState;
    public GameState currentState
    {
        get => _currentState;
        set
        {
            if (_currentState == value) return; // ���°� ������ ����� ���� �α� ���

            Debug.Log($"GameState �����: {_currentState} -> {value}");
            _currentState = value;
        }
    }

    private PhotonView photonView;

    public SpriteRenderer boardRenderer;    //�ٵ��� spr
    private Vector2[,] gridPosition;        //����

    private Dictionary<int, bool> playerReadyStatus = new Dictionary<int, bool>();
    private GameObject[,] placedStones;     //�� ����
    [HideInInspector] public int player;
    private int currentPlayerTurn;




    private void Awake()
    {
        InitializeGrid();

        photonView = GetComponent<PhotonView>();
        _instance = this;
    }

    private void Start()
    {
        placedStones = new GameObject[15, 15];
    }


    #region Init
    private void InitializeGrid()
    {
        int gridSize = 15;
        gridPosition = new Vector2[gridSize, gridSize];

        //�ٵ��� �̹��� ����
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;

        //�� ���� ������ ���� ���
        float cellWidth = boardWidth / gridSize;
        float cellHeight = boardHeight / gridSize;

        Vector3 boardPosition = boardRenderer.transform.position;

        for (int i = 0; i < gridSize; i++)
        {
            for (int j = 0; j < gridSize; j++)
            {
                float posX = boardPosition.x + (i * cellWidth) - (boardWidth / 2) + (cellWidth / 2);
                float posY = boardPosition.y + (j * cellHeight) - (boardHeight / 2) + (cellHeight / 2);
                gridPosition[i, j] = new Vector2(posX, posY);
            }
        }
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



    #region �غ���� 
    public void SetReadyState()
    {
        currentState = GameState.READY;
        InitializeGame();
    }

    public void OnPlayerReady()
    {
        int localPlayerID = PhotonNetwork.LocalPlayer.ActorNumber;
        playerReadyStatus[localPlayerID] = true;

        //��� Ŭ���̾�Ʈ�� READY ���� ����
        photonView.RPC("PlayerReady", RpcTarget.All, localPlayerID);

        // ������ Ŭ���̾�Ʈ�� ��� �÷��̾��� �غ� ���� Ȯ��
        if (PhotonNetwork.IsMasterClient)
            CheckAllPlayersReady();
    }

    [PunRPC]
    public void PlayerReady(int playerID)
    {
        playerReadyStatus[playerID] = true;
        Debug.Log($"Player {playerID} is ready");

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

    //���� ����
    private void StartGame()
    {
        currentState = GameState.PLAYING;
        photonView.RPC("SetGameState", RpcTarget.All, GameState.PLAYING);
        currentPlayerTurn = 1;
    }

    [PunRPC]
    public void SetGameState(GameState state)
    {
        currentState = state;
    }

    [PunRPC]
    public void PlaceStone(int x, int y, int playerNumber)
    {
        string stonePath = playerNumber == 1 ? "Prefabs/R_Black" : "Prefabs/R_White";
        GameObject stonePrefab = Resources.Load<GameObject>(stonePath);
        GameObject newStone = Instantiate(stonePrefab, gridPosition[x, y], Quaternion.identity);
        placedStones[x, y] = newStone;
    }

    private void PlaceStoneAtMousePosition()
    {
        if (currentState != GameState.PLAYING || player != currentPlayerTurn)
            return;

        Vector2 mouseWorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        int closestX = -1;
        int closestY = -1;

        float closetDistance = Mathf.Infinity;

        //���� ����� ���� ��ġ ã��
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

        if (placedStones[closestX, closestY] == null)
        {
            photonView.RPC("PlaceStone", RpcTarget.All, closestX, closestY, player);
            ChangeTurn();
        }
    }

    private void ChangeTurn()
    {
        //���� ���� �÷��̾�� ����
        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
        photonView.RPC("UpdateCurrentTurn", RpcTarget.All, currentPlayerTurn);
        
        Debug.Log($"������ {currentPlayerTurn}�� ���Դϴ�.");
    }

    [PunRPC]
    public void UpdateCurrentTurn(int newTurn)
    {
        currentPlayerTurn = newTurn;
    }

    public void CheckPlayer2Turn()
    {
        if (currentPlayerTurn == 2)
        {
            // �ΰ��� �ܼ��� �ν��Ͻ��� ã�� CheckPlayer2CanPlaceStone �޼��带 ȣ��
            InGameConsole console = FindObjectOfType<InGameConsole>();
            if (console != null)
            {
                console.CheckPlayer2CanPlaceStone(true);
            }
        }
        else
        {
            InGameConsole console = FindObjectOfType<InGameConsole>();
            if (console != null)
            {
                console.CheckPlayer2CanPlaceStone(false);
            }
        }
    }
}

