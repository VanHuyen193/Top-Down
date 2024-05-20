using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [System.Serializable]
    public class Wave
    {
        public string waveName;
        public List<EnemyGroup> enemyGroups; // List nhóm enemy được spawn trong đợt
        public int waveQuota; // Tổng số enemy xuất hiện trong đợt
        public float spawnInterval; // Khoảng thời gian xuất hiện của enemy
        public int spawnCount; // Theo dõi số enemy đã được spawn ra trong đợt
    }

    [System.Serializable]
    public class EnemyGroup
    {
        public string enemyName;
        public int enemyCount; // Tổng số enemy xuất hiện trong đợt
        public int spawnCount; // Số enemy loại này đã được spawn
        public GameObject enemyPrefab;
    }

    public List<Wave> waves; // List gồm các đợt enemy xuất hiện
    public int currentWaveCount; // Vị trí của wave hiện tại

    [Header("Spawner Attributes")]
    float spawnTimer; // Thời gian spawn ra enemy tiếp theo
    public int enemiesAlive = 0; // Số enemy còn sống
    public int enemiesDead = 0; // Số enemy đã chết
    public int maxEnemiesAllowed; // Số enemy tối đa được xuất hiện trên map
    public bool maxEnemiesReached = false;
    public float waveInterval; // Khoảng thời gian giữa mỗi đợt
    bool isWaveActive = false;

    [Header("Spawn Positions")]
    public List<Transform> relativeSpawnPoints;

    [Header("UI")]
    public TMPro.TextMeshProUGUI enemiesDeadText;

    Transform player;

    // Start is called before the first frame update
    void Start()
    {
        player = FindObjectOfType<PlayerStats>().transform;
        CalculateWaveQuota();
    }

    // Update is called once per frame
    void Update()
    {
        // Chuyển sang wave tiếp theo nếu wave hiện tại kết thúc
        if(currentWaveCount < waves.Count && waves[currentWaveCount].spawnCount == 0 && !isWaveActive)
        {
            StartCoroutine(BeginNextWave());
        }

        spawnTimer += Time.deltaTime;

        if(spawnTimer >= waves[currentWaveCount].spawnInterval)
        {
            spawnTimer = 0f;
            SpawnEnemies();
        }
    }

    IEnumerator BeginNextWave()
    {
        isWaveActive = true;

        yield return new WaitForSeconds(waveInterval);

        if(currentWaveCount < waves.Count - 1)
        {
            isWaveActive = false;
            currentWaveCount++;
            CalculateWaveQuota();
        }
    }


    // Tính toán tổng số enemy cho từng wave (waveQuota)
    void CalculateWaveQuota()
    {
        int currentWaveQuota = 0;
        foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
        {
            currentWaveQuota += enemyGroup.enemyCount;
        }

        waves[currentWaveCount].waveQuota = currentWaveQuota;
        Debug.LogWarning(currentWaveQuota);
    }

    void SpawnEnemies()
    {
        // Check if the minimum number of enemies in the wave have been spawned
        if (waves[currentWaveCount].spawnCount < waves[currentWaveCount].waveQuota && !maxEnemiesReached)
        {
            // Spawn each type of enemy until the quota is filled
            foreach (var enemyGroup in waves[currentWaveCount].enemyGroups)
            {
                // Check ig the minimum number of enemies of this type have been spawned
                if(enemyGroup.spawnCount < enemyGroup.enemyCount)
                {
                    // Spawn the enemy at a random position close to the player
                    Instantiate(enemyGroup.enemyPrefab, player.position + relativeSpawnPoints[Random.Range(0, relativeSpawnPoints.Count)].position, Quaternion.identity);

                    enemyGroup.spawnCount++;
                    waves[currentWaveCount].spawnCount++;
                    enemiesAlive++;

                    // Limit the number of enemies that can be spawn at once
                    if (enemiesAlive >= maxEnemiesAllowed)
                    {
                        maxEnemiesReached = true;
                        return;
                    }
                }
            }
        }
    }

    public void OnEnemyKilled()
    {
        enemiesAlive--;
        enemiesDead++;

        enemiesDeadText.text = enemiesDead.ToString();

        // Reset the maxEnemiesReached flag if the number of enemies alive has dropped below the maximum amount
        if (enemiesAlive < maxEnemiesAllowed)
        {
            maxEnemiesReached = false;
        }
    }
}
