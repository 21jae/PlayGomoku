using Photon.Pun;
using Photon.Realtime;
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
    private GameState currentState              { get; set; }
    private Vector2[,] gridPosition             { get; set; }   //����
    private GameObject[,] stoneObjects          { get; set; }   //�ٵϾ� ����
    private int[,] placedStones                 { get; set; }   //�ٵϾ� �÷��̾� ��ȣ ����
    private int currentPlayerTurn               { get; set; }   //��
    public int player                           { get; set; }


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
    /// <summary>
    /// �ٵ��� ����
    /// </summary>
    private void InitializeGrid()
    {
        gridPosition = new Vector2[GRID_SIZE, GRID_SIZE];
        stoneObjects = new GameObject[GRID_SIZE, GRID_SIZE];
        placedStones = new int[GRID_SIZE, GRID_SIZE];

        //�ٵ��� �̹��� ����
        float boardWidth = boardRenderer.bounds.size.x;
        float boardHeight = boardRenderer.bounds.size.y;

        //�� ���� ������ ���� ���
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

    /// <summary>
    /// ������ �ʱ�ȭ
    /// </summary>
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

    /// <summary>
    /// �÷��̾ ��� �غ� �Ǿ����� Ȯ���Ѵ�.
    /// ������ ���� ������ �����ϴ�.
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

    private void StartGame()
    {
        SetPlayingState();
        photonView.RPC("SetGameState", RpcTarget.All, GameState.PLAYING);
        currentPlayerTurn = 1;
    }

    /// <summary>
    /// ���ڼ��� ��Ȯ�� ������ �ʾƵ� ��ó�� �������ִ� �޼���
    /// Ŭ���� ��ġ�� ���� ����� ���ڿ� �ٵϾ� ����
    /// ���� �ۿ� Ŭ���� ���� �Ұ���
    /// </summary>
    private void PlaceStoneAtMousePosition()
    {
        if (!IsGamePlaying() || player != currentPlayerTurn)
            return;

        int closestX;
        int closestY;

        FindClosestGridPosition(out closestX, out closestY);

        if (closestX >= 0 && closestX < GRID_SIZE && closestY >= 0 && closestY < GRID_SIZE)
            TryPlaceStoneAndChagneTurn(closestX, closestY);
        else
            Debug.Log("�ű⿣ �� �� ���ٳ�.");
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
    /// <summary>
    /// �ٵϵ��� ������ ���� �����մϴ�
    /// ������ �������� �޼����� ��µ˴ϴ�.
    /// </summary>
    private void ChangeTurn()
    {
        currentPlayerTurn = currentPlayerTurn == 1 ? 2 : 1;
        photonView.RPC("UpdateCurrentTurn", RpcTarget.All, currentPlayerTurn);
        string currentPlayerNickname = PhotonNetwork.CurrentRoom.Players[currentPlayerTurn].NickName;
        UIManager.Instance.photonView.RPC("UpdateTurnText", RpcTarget.All, currentPlayerNickname);
        UIManager.Instance.photonView.RPC("ResetTimerRPC", RpcTarget.All, 60f);
    }

    /// <summary>
    /// �¸� ������ ������ GameOver ���� ��ȯ
    /// �¸� ���� ��� �� ���� �ʱ�ȭ
    /// </summary>
    private void SetGameOver()
    {
        SetGameOverState();
        photonView.RPC("SetGameState", RpcTarget.All, GameState.GAMEOVER);
        UIManager.Instance.ToggleStartButtons();
        StartCoroutine(RestartGameDelay(3f));
    }

    private IEnumerator RestartGameDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        UIManager.Instance.photonView.RPC("RemoveResultPanelRPC", RpcTarget.All);
        InitializePlayerReadyStatus();
        InitializeGame();
        SetReadyState();
    }

    #region Game Rules
    /// <summary>
    /// ���� �¸� ����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="playerNumber"></param>
    /// <returns></returns>
    private bool CheckHorizontalWin(int x, int y, int playerNumber)
    {
        //���� ���� ���� ī��Ʈ�� ������
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

        // �밢�� [/] ��� �˻�
        for (int i = x - 1, j = y - 1; i >= 0 && j >= 0 && placedStones[i, j] == playerNumber; i--, j--)
            countDown++;

        for (int i = x + 1, j = y + 1; i < GRID_SIZE && j < GRID_SIZE && placedStones[i, j] == playerNumber; i++, j++)
            countDown++;

        // �밢�� [\] ��� �˻�
        for (int i = x + 1, j = y - 1; i < GRID_SIZE && j >= 0 && placedStones[i, j] == playerNumber; i++, j--)
            countUp++;

        for (int i = x - 1, j = y + 1; i >= 0 && j < GRID_SIZE && placedStones[i, j] == playerNumber; i--, j++)
            countUp++;

        return countDown >= 5 || countUp >= 5;
    }

    /// <summary>
    /// ���ھ� ���� ������Ʈ ����
    /// </summary>
    private void ResetAllPlayersScoreUpdateStatus()
    {
        foreach (var playerInfo in FindObjectsOfType<PlayerInfo>())
        {
            playerInfo.ResetScoreUpdateStatus();
        }
    }
    #endregion

    #region PunRPC
    /// <summary>
    /// ���� ��Ʈ��ũ
    /// ���� ���¸� ��� �÷��̾�� �˸�
    /// </summary>
    /// <param name="state"></param>
    [PunRPC] public void SetGameState(GameState state) => currentState = state;
    [PunRPC] public void UpdateCurrentTurn(int newTurn) => currentPlayerTurn = newTurn;

    /// <summary>
    /// �غ� ���� �������� �˸�
    /// ���� �����͸� ���� ���� ����
    /// </summary>
    /// <param name="playerID"></param>
    [PunRPC]
    public void PlayerReady(int playerID)
    {
        playerReadyStatus[playerID] = true;

        if (PhotonNetwork.IsMasterClient)
            CheckAllPlayersReady();
    }

    /// <summary>
    /// �ٵϾ� ���� �� �¸����� ����
    /// </summary>
    /// <param name="x"></param>
    /// <param name="y"></param>
    /// <param name="playerNumber"></param>
    [PunRPC]
    public void PlaceStone(int x, int y, int playerNumber)
    {
        string stonePath = playerNumber == 1 ? "Prefabs/Ingame_Slime" : "Prefabs/Ingame_Yeti";
        GameObject stonePrefab = Resources.Load<GameObject>(stonePath);
        GameObject newStone = Instantiate(stonePrefab, gridPosition[x, y], Quaternion.identity);
        SoundManager.Instance.PlayButtonAndClickSound();
        placedStones[x, y] = playerNumber;
        stoneObjects[x, y] = newStone;

        if (CheckHorizontalWin(x, y, playerNumber) || CheckVerticalWin(x, y, playerNumber) || CheckDiagonalWin(x, y, playerNumber))
        {
            photonView.RPC("UpdateScoreRPC", RpcTarget.All, playerNumber);
            UIManager.Instance.photonView.RPC("UpdateResultPanelRPC", RpcTarget.All, playerNumber);
            SoundManager.Instance.PlayVictorySound();
            SetGameOver();
        }
    }

    /// <summary>
    /// �¸� �� �й� ���ھ� ������Ʈ
    /// </summary>
    /// <param name="winningPlayer"></param>
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
}