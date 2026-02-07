using TMPro;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance;

    [Header("Enemy Manager")]
    [SerializeField] private EnemyManager enemyManager;

    private int currentWave = 0;
    private const int MAX_WAVES = 4;

    private readonly int[][] waveRows =
    {
        new int[] { 22, 14, 14, 14, 14, 14 },
        new int[] { 22, 14, 14, 14 },
        new int[] { 22, 14, 14, 14 },
        new int[] { 22, 29, 14 }
    };

    private readonly int[] waveBonus =
    {
        3000,
        1200,
        1200,
        900
    };

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        StartWave();
    }

    public void OnWaveCompleted()
    {
        GameManager.Instance.AddScore(waveBonus[currentWave]);

        currentWave++;

        if (currentWave >= MAX_WAVES)
        {
            currentWave = 0;
            return;
        }

        Invoke(nameof(StartWave), 2f);
    }

    private void StartWave()
    {
        enemyManager.rows = 5;
        enemyManager.cols = 11;

        enemyManager.startPosition = new Vector2(-6.5f, 7.5f);

        enemyManager.enabled = false;
        enemyManager.enabled = true;

        UFOManager ufoManager = FindFirstObjectByType<UFOManager>();
        if (ufoManager != null)
            ufoManager.OnWaveStarted(currentWave);
    }
}