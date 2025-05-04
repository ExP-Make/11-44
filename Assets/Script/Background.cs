using UnityEngine;

public class MultiLayerBackground : MonoBehaviour
{
    [Header("�� ��ȣ (0���� ����)")]
    public int sceneIndex = 0;

    [Header("�� ������ ���̾�� ��� �̹��� (���� 5���� �������)")]
    public Sprite[] layeredSprites;

    private const int layerCount = 5;
    private GameObject[] layerObjects = new GameObject[layerCount];

    void Start()
    {
        // Layer_0 ~ Layer_4 ������Ʈ ã��
        for (int i = 0; i < layerCount; i++)
        {
            layerObjects[i] = GameObject.Find("Layer_" + i);
            if (layerObjects[i] == null)
            {
                Debug.LogWarning($"Layer_{i} ������Ʈ�� ���� �����ϴ�!");
            }
        }

        ApplySpritesForScene();
    }

    void ApplySpritesForScene()
    {
        for (int i = 0; i < layerCount; i++)
        {
            int spriteIndex = sceneIndex * layerCount + i;

            if (spriteIndex >= 0 && spriteIndex < layeredSprites.Length && layerObjects[i] != null)
            {
                layerObjects[i].GetComponent<SpriteRenderer>().sprite = layeredSprites[spriteIndex];
                layerObjects[i].GetComponent<SpriteRenderer>().sortingOrder = i * 10; // ��ġ�� �ʰ� ����
            }
        }
    }
}