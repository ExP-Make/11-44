using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public string gameSceneName = "BootScene"; // �̵��� ���� �� �̸�

    public void OnClickStart()
    {
        Debug.Log("New Game");
        GameManager.Instance.ignoreSaveData = true;
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickLoad()
    {
        // TODO : �Ȱ��� BootScene���� �̵��ϰ�, BootScene���� ���̺� ������ ���� �Ǻ�
        GameManager.Instance.ignoreSaveData = false;
        SceneManager.LoadScene(gameSceneName);
    }
    public void OnClickExit()
    {
        Debug.Log("���� ����!");
        Application.Quit(); // ����� ���¿����� �۵�
    }
}