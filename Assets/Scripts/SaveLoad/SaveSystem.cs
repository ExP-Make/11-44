using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SaveSystem
{
    private static string savePath => Path.Combine(Application.persistentDataPath, "save.json");

    public static void SaveGame(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("게임 저장 완료");
    }

    public static SaveData LoadGame()
    {
        if (!File.Exists(savePath))
        {
            Debug.LogWarning("세이브 파일 없음.");
            return null;
        }

        string json = File.ReadAllText(savePath);
        return JsonUtility.FromJson<SaveData>(json);
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("세이브 파일 삭제 완료");
        }
    }

    public static bool SaveFileExists()
    {
        return File.Exists(savePath);
    }
}
