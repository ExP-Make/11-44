using UnityEngine;
using UnityEngine.SceneManagement;

public class StartManager : MonoBehaviour
{
    public string gameSceneName = "Scene1"; // �̵��� ���� �� �̸�

    public void OnClickStart()
    {
        Debug.Log("���� ����!");
        SceneManager.LoadScene(gameSceneName);
    }

    public void OnClickExit()
    {
        Debug.Log("���� ����!");
        Application.Quit(); // ����� ���¿����� �۵�
    }
}