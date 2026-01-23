using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawn : MonoBehaviour
{
    public static EnemySpawn Instance { get; private set; }

    [SerializeField] List<EnemyCount> enemyCounts;



    private List<GameObject> spawnedEnemies = new List<GameObject>(); // 生成された敵のリスト

    void Start()
    {
        //SpawnEnemies();
    }

    private void SpawnEnemies()
    {
        for (int i = 0; i < enemyCounts.Count; i++)
        {
            EnemyGenerator(enemyCounts[i]);
        }

    }

    private void EnemyGenerator(EnemyCount data)
    {
        if (data.isSpawn)
        {
            foreach (Vector2 p in data.pos)
            {
                if (data.enemyType == EnemyCount.EnemyType.Enemy0)
                {
                    // EnemySpawnオブジェクトの位置を基準にした相対位置でスポーン
                    Vector3 spawnPos = transform.position + (Vector3)p;
                    GameObject enemyObj = Instantiate(data.enemyPrefab, spawnPos, data.rot);
                    enemyObj.tag = "Enemy";
                    spawnedEnemies.Add(enemyObj);
                    enemyObj.SetActive(true);
                }


                if (data.enemyType == EnemyCount.EnemyType.Boss)
                {
                    // EnemySpawnオブジェクトの位置を基準にした相対位置でスポーン
                    Vector3 spawnPos = transform.position + (Vector3)p;
                    GameObject enemyObj = Instantiate(data.enemyPrefab, spawnPos, data.rot);
                    enemyObj.tag = "BossEnemy";
                    spawnedEnemies.Add(enemyObj);
                    enemyObj.SetActive(true);
                    // プレイヤーのTransformを取得
                    GameObject player = GameObject.FindGameObjectWithTag("Player");
                    if (player != null)
                    {
                        MainCameraScript.Instance.SetDefaultTarget(player.transform);
                        MainCameraScript.Instance.FocusOn(enemyObj.transform, 3f); // 3秒間ボスにフォーカス
                    }
                }
            }

        }

    }
    private IEnumerator FocusCameraToBoss(Transform bossTransform)
    {
        // プレイヤーを取得
        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // メインカメラ制御にプレイヤーを登録
        if (MainCameraScript.Instance != null)
        {
            MainCameraScript.Instance.SetDefaultTarget(player.transform);
            yield return new WaitForSeconds(0.5f); // 少し待つとスムーズ

            MainCameraScript.Instance.FocusOn(bossTransform);
        }
    }
    // 手動で呼び出して敵をスポーンさせる
    public void SpawnEnemiesManually()
    {
        SpawnEnemies();
    }

    // 生成された敵のリストを取得
    public List<GameObject> GetSpawnedEnemies()
    {
        return spawnedEnemies;
    }
    public void DestroyAllEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        foreach (GameObject enemy in enemies)
        {
            Destroy(enemy);
        }
    }
    public void DestroyAllBossEnemies()
    {
        GameObject[] bossEnemies = GameObject.FindGameObjectsWithTag("BossEnemy");

        foreach (GameObject enemy in bossEnemies)
        {
            Destroy(enemy);
        }
    }


    [System.Serializable]
    public class EnemyCount
    {
        public enum EnemyType
        {
            Enemy0,
            Enemy1,
            Enemy2,
            Enemy3,
            Boss,
        }

        public EnemyType enemyType;            // ラベル的な用途
        public List<Vector2> pos;              // EnemySpawn位置からの相対配置位置のリスト
        public Quaternion rot;                 // 回転
        public GameObject enemyPrefab;         // 敵のプレハブ
        public string ID;                      // 任意のID
        public bool isSpawn;                   // この敵を生成するかどうか
    }

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this)
        {
            Instance = null;
        }
    }
}
