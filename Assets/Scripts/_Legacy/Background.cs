using UnityEngine;

public class MultiLayerBackground : MonoBehaviour
{
    [Header("씬 번호 (0부터 시작)")]
    public int sceneIndex = 0;

    [Header("각 씬마다 레이어당 배경 이미지 (씬당 5개씩 순서대로)")]
    public Sprite[] layeredSprites;

    private const int layerCount = 5;
    private GameObject[] layerObjects = new GameObject[layerCount];

    void Start()
    {
        // Layer_0 ~ Layer_4 오브젝트 찾기
        for (int i = 0; i < layerCount; i++)
        {
            layerObjects[i] = GameObject.Find("Layer_" + i);
            if (layerObjects[i] == null)
            {
                Debug.LogWarning($"Layer_{i} 오브젝트가 씬에 없습니다!");
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
                layerObjects[i].GetComponent<SpriteRenderer>().sortingOrder = i * 10; // 겹치지 않게 정렬
            }
        }
    }
}