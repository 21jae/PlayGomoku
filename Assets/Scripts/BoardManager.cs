using UnityEngine;

public class BoardManager : Singleton<BoardManager>
{
    public int width = 15;
    public int height = 15;
    //public GameObject stonePrefab;

    private int[,] board;

    private GameObject blackStonePrefab;
    private GameObject whiteStonePrefab;

    private void Start()
    {
        blackStonePrefab = Resources.Load<GameObject>("Prefabs/R_Black");
        whiteStonePrefab = Resources.Load<GameObject>("Prefabs/R_White");

        UIManager.GetInstance().CreateGomokuBoard(width, height);
    }


    public bool PlaceStone(int x, int y, int player)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
        {
            Debug.LogError("거기엔 둘 수 없습니다.");
            return false;
        }

        if (board[x, y] != 0)
        {
            Debug.LogError("해당 칸은 이미 사용 중입니다.");
            return false;
        }

        board[x, y] = player;
        InstantiateStone(x, y, player);

        return true;
    }

    private void InstantiateStone(int x, int y, int player)
    {
        //돌 넣을 위치 계산
        Vector3 position = GetWorldPosition(x, y);

        GameObject stonePrefab = (player == 1) ? blackStonePrefab : whiteStonePrefab;
        GameObject stone = Instantiate(stonePrefab, position, Quaternion.identity);
        stone.transform.parent = transform;

        if (player == 1)
        {
            //플레이어 1의 돌 외관 설정
        }

        else if (player == 2)
        {
            //플레이어 2의 돌 외관 설정
        }

    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        //보드 좌표 월드 위치 변환
        return new Vector3(x, y, 0);
    }
}
