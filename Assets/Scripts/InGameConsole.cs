using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InGameConsole : MonoBehaviour
{
    public Text consoleText; // �ν����Ϳ��� �Ҵ�
    private Queue<string> logQueue = new Queue<string>();

    private void OnEnable()
    {
        Application.logMessageReceived += HandleLog;
    }

    private void OnDisable()
    {
        Application.logMessageReceived -= HandleLog;
    }

    void HandleLog(string logString, string stackTrace, LogType type)
    {
        // �� �α� �޽����� ť�� �߰�
        logQueue.Enqueue(logString);

        // �ֿܼ� �ʹ� ���� �ؽ�Ʈ�� ������ ������ �ͺ��� ����
        while (logQueue.Count > 50) // 50�� ������ �α��� �ִ� ��
        {
            logQueue.Dequeue();
        }

        consoleText.text = string.Join("\n", logQueue.ToArray());
    }

    // Player2�� ���� ���� �� �ִ��� Ȯ���ϴ� �޼��� �߰� (����)
    public void CheckPlayer2CanPlaceStone(bool canPlace)
    {
        string message = canPlace ? "Player 2 can place a stone" : "Player 2 cannot place a stone";
        Debug.Log(message); // �� �޽����� �ΰ��� �ֿܼ� ǥ�õ˴ϴ�.
    }
}
