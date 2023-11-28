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
            Debug.LogError("�ű⿣ �� �� �����ϴ�.");
            return false;
        }

        if (board[x, y] != 0)
        {
            Debug.LogError("�ش� ĭ�� �̹� ��� ���Դϴ�.");
            return false;
        }

        board[x, y] = player;
        InstantiateStone(x, y, player);

        return true;
    }

    private void InstantiateStone(int x, int y, int player)
    {
        //�� ���� ��ġ ���
        Vector3 position = GetWorldPosition(x, y);

        GameObject stonePrefab = (player == 1) ? blackStonePrefab : whiteStonePrefab;
        GameObject stone = Instantiate(stonePrefab, position, Quaternion.identity);
        stone.transform.parent = transform;

        if (player == 1)
        {
            //�÷��̾� 1�� �� �ܰ� ����
        }

        else if (player == 2)
        {
            //�÷��̾� 2�� �� �ܰ� ����
        }

    }

    private Vector3 GetWorldPosition(int x, int y)
    {
        //���� ��ǥ ���� ��ġ ��ȯ
        return new Vector3(x, y, 0);
    }
}
