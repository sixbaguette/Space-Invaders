using UnityEngine;

public class UFOManager : MonoBehaviour
{
    [SerializeField] private UFOPool ufoPool;

    [Header("Spawn Settings")]
    [SerializeField] private float yPosition = 4.5f;
    [SerializeField] private float xOffset = 12f;

    private readonly int[][] ufoSpawnShotsPerWave =
    {
        new int[] { 22, 14, 14, 14, 14, 14 },
        new int[] { 22, 14, 14, 14 },
        new int[] { 22, 14, 14, 14 },
        new int[] { 22, 29, 14 }
    };

    private int currentWave = 0;
    private int nextUFOIndex = 0;

    private int lastSpawnShot = 0;

    private void OnEnable()
    {
        if (PlayerShotCounter.Instance != null)
            PlayerShotCounter.Instance.RegisterShotEvent += OnPlayerShot;
    }

    private void OnDisable()
    {
        if (PlayerShotCounter.Instance != null)
            PlayerShotCounter.Instance.RegisterShotEvent -= OnPlayerShot;
    }

    private void OnPlayerShot(int totalShots)
    {
        if (nextUFOIndex >= ufoSpawnShotsPerWave[currentWave].Length)
            return;

        int shotsSinceLastSpawn = totalShots - lastSpawnShot;

        int targetShots = ufoSpawnShotsPerWave[currentWave][nextUFOIndex];

        if (shotsSinceLastSpawn >= targetShots)
        {
            SpawnUFO();
            nextUFOIndex++;
            lastSpawnShot = totalShots;
        }
    }

    private void SpawnUFO()
    {
        bool fromLeft = Random.value > 0.5f;

        float x = fromLeft ? -xOffset : xOffset;
        Vector3 spawnPos = new Vector3(x, yPosition, 0f);

        GameObject ufo = ufoPool.SpawnUFO(spawnPos);
        if (ufo == null) return;

        Vector2 dir = fromLeft ? Vector2.right : Vector2.left;
        ufo.GetComponent<UFOController>().SetDirection(dir);
    }

    public void OnWaveStarted(int wave)
    {
        currentWave = wave % 4;
        nextUFOIndex = 0;
        lastSpawnShot = PlayerShotCounter.Instance != null ? PlayerShotCounter.Instance.TotalShots : 0;
    }
}