using UnityEngine;

public class BossBattleManager : MonoBehaviour
{
    private static BossBattleManager _instance;
    public static BossBattleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject("BossBattleManager");
                _instance = obj.AddComponent<BossBattleManager>();
            }
            return _instance;
        }
    }

    private UniversalEnemyController currentBoss;
    private GameObject bossUIInstance;
    private AudioSource bgmSource;
    private AudioClip originalBGM;

    void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        GameObject bgmObj = GameObject.FindWithTag("BGM");
        if (bgmObj != null)
        {
            bgmSource = bgmObj.GetComponent<AudioSource>();
            if (bgmSource != null)
            {
                originalBGM = bgmSource.clip;
            }
        }
    }

    public void StartBoss(UniversalEnemyController boss)
    {
        if (currentBoss != null) return;
        currentBoss = boss;

        EnemyData data = boss.enemyData;

        if (data.bossUIPrefab != null)
        {
            bossUIInstance = Instantiate(data.bossUIPrefab);
            BossUI ui = bossUIInstance.GetComponent<BossUI>();
            if (ui != null)
            {
                ui.Initialize(boss);
            }
        }

        if (bgmSource != null && data.bossMusic != null)
        {
            originalBGM = bgmSource.clip;
            bgmSource.clip = data.bossMusic;
            bgmSource.Play();
        }
    }

    public void EndBoss()
    {
        if (currentBoss == null) return;

        if (bossUIInstance != null)
        {
            Destroy(bossUIInstance);
        }

        if (bgmSource != null && originalBGM != null)
        {
            bgmSource.clip = originalBGM;
            bgmSource.Play();
        }

        currentBoss = null;
    }
} 