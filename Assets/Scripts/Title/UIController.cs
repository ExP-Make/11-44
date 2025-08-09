using UnityEngine;

public class UIController : MonoBehaviour
{
    public GameObject UIPanel;
    public GameObject TitleUI;

    public void OpenUI()
    {
        if (UIPanel != null)
        {
            UIPanel.SetActive(true); // 설정창 열기
            TitleUI.SetActive(false); // 타이틀 UI 비활성화
        }
    }

    public void CloseUI()
    {
        if (UIPanel != null)
        {
            UIPanel.SetActive(false); // 설정창 닫기
            TitleUI.SetActive(true); // 타이틀 UI 활성화
        }
    }
}
