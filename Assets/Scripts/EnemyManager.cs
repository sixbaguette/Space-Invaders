
using System.Collections;
using System.Collections.Generic;
using System.Data;
using TMPro;
using UnityEditor.ShaderGraph.Internal;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    [SerializeField]
    private GameObject player;
    private float playerBoundaryX;
    public int explosionDuration = 17;

    public EnemyPool enemyPool;
    public int rows = 5;
    public int cols = 11;
    public float spacing = 1.5f;
    public float _stepDistance = 0.5f;
    public float stepDistanceVertical = 1f;

    public Vector2 startPosition = new Vector2 (-6.5f, 7.5f);

    private GameObject[,] enemies;
    private int reverseGrave;

    private bool isPaused = false;

    private enum MoveState { MoveRight, MoveLeft }
    private MoveState currentState = MoveState.MoveRight;

    public GameObject missilePrefab;
    public GameObject laserPrefab;
    public GameObject bulletPrefab;
    public Transform missilePoint;
    public float missileInterval = 2.0f;

    public TextMeshProUGUI textScrore;

    void Start()
    {
        playerBoundaryX = player.GetComponent<PlayerScript>().boundary;
        enemies = new GameObject[rows, cols];

        SpawnEnemies();

        StartCoroutine(HandleEnemyMovement());

        StartCoroutine(EnemyShooting());
    }

    void Update()
    {
        TextScore();
    }

    private void SpawnEnemies()
    {
        var enemyTypes = enemyPool.GetEnemyTypes();

        for (int row = 0; row < rows; row++)
        {
            var enemyType = GetEnemyTypeForRow(row, enemyTypes);
            for (int col = 0; col < cols; col++)
            {
                GameObject enemy = enemyPool.GetEnemy(enemyType.prefab);

                if (enemy != null)
                {
                    float xPos = startPosition.x + (col * spacing);
                    float yPos = startPosition.y - (row * spacing);

                    Debug.Log($"[EnemyManager] {enemy.name} est à la position X: {xPos}; Y: {yPos}");

                    enemy.transform.position = new Vector3(xPos, yPos, 0);

                    EnemyScript enemyScript = enemy.GetComponent<EnemyScript>();
                    if (enemyScript != null)
                    {
                        enemyScript.enemyType = enemyType;
                        enemyScript.ScoreData = enemyType.points;
                    }

                    enemies[row, col] = enemy;

                    reverseGrave++;
                }
            }
        }
    }

    IEnumerator HandleEnemyMovement()
    {
        while (reverseGrave > 0)
        {
            bool boundaryReached = false;

            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (MenuPause.IsPaused || GameManager.Instance.isExploding)
                    {
                        yield return new WaitUntil(() => !MenuPause.IsPaused && !GameManager.Instance.isExploding);
                    }

                    if (enemies[row, col] != null && enemies[row, col].activeSelf)
                    {
                        Vector3 direction = currentState == MoveState.MoveRight ? Vector3.right : Vector3.left;

                        MoveEnemy(enemies[row, col], direction, _stepDistance);

                        if (enemies[row, col] == null) continue;

                        EnemyScript enemyScript = enemies[row, col].GetComponent<EnemyScript>();
                        if (enemyScript != null) enemyScript.ChangeSprite();

                        if (ReachedBoundery(enemies[row, col])) boundaryReached = true;

                        yield return null;
                    }
                }
            }

            if (boundaryReached)
            {
                yield return MoveAllEnemiesDown();
                currentState = currentState == MoveState.MoveRight ? MoveState.MoveLeft : MoveState.MoveRight;
            }
        }
    }

    IEnumerator MoveAllEnemiesDown()
    {
        if (MenuPause.IsPaused)
        {
            yield return null;
        }
        else
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                for (int col = 0; col < cols; col++)
                {
                    if (enemies[row, col] != null && enemies[row, col].activeSelf)
                    {
                        Vector3 direction = Vector3.down;

                        MoveEnemy(enemies[row, col], direction, _stepDistance);

                        yield return null;
                    }
                }
            }
        }
    }

    IEnumerator EnemyShooting()
    {
        if (MenuPause.IsPaused)
        {
            yield return null;
        }
        else
        {
            while (true)
            {
                yield return new WaitUntil(() => !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding);

                yield return new WaitForSeconds(Random.Range(missileInterval, missileInterval * 2));

                List<GameObject> shooters = GetBottomEnemies();

                if (shooters.Count > 0 && !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
                {
                    GameObject shooter = shooters[Random.Range(0, shooters.Count)];

                    FireMissile(shooter);
                }

                yield return new WaitForSeconds(Random.Range(missileInterval, missileInterval * 2));

                if (shooters.Count > 0 && !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
                {
                    GameObject shooter = shooters[Random.Range(0, shooters.Count)];

                    FireLaser(shooter);
                }

                yield return new WaitForSeconds(Random.Range(missileInterval, missileInterval * 2));

                if (shooters.Count > 0 && !GameManager.Instance.IsPaused && !GameManager.Instance.isExploding)
                {
                    GameObject shooter = shooters[Random.Range(0, shooters.Count)];

                    FireBullet(shooter);
                }
            }
        }
    }

    private List<GameObject> GetBottomEnemies()
    {
        List<GameObject> bottomeEnemies = new List<GameObject>();

        for (int col = 0; col < cols; col++)
        {
            for (int row = rows - 1; row >= 0; row--)
            {
                if (enemies[row, col] != null && enemies[row, col].activeSelf)
                {
                    bottomeEnemies.Add(enemies[row, col]);
                    break;
                }
            }
        }

        return bottomeEnemies;
    }

    private void FireMissile(GameObject shooter)
    {
        Transform firePoint = shooter.transform.Find("FirePoint");

        if (firePoint != null)
        {
            Instantiate(missilePrefab, firePoint.position, Quaternion.identity);
        }
        else
        {
            Debug.Log($"FirePoint non trouvé pour l'ennemi : {shooter.name}");
        }
    }

    private void FireLaser(GameObject shooter)
    {
        Transform firePoint = shooter.transform.Find("FirePoint");

        if (firePoint != null)
        {
            Instantiate(laserPrefab, firePoint.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("FirePoint non trouvé");
        }
    }

    private void FireBullet(GameObject shooter)
    {
        Transform firePoint = shooter.transform.Find("FirePoint");

        if (firePoint != null)
        {
            Instantiate(bulletPrefab, firePoint.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("FirePoint non trouvé");
        }
    }

    private void MoveEnemy(GameObject enemy, Vector3 direction, float stepDistance)
    {
        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position + direction * stepDistance;

        newPosition.x = Mathf.Round(newPosition.x * 100f) / 100f;
        newPosition.y = Mathf.Round(newPosition.y * 100f) / 100f;
        newPosition.z = Mathf.Round(newPosition.z * 100f) / 100f;

        enemy.transform.position = newPosition;

        EnemyBottom(enemy, direction);
    }

    private void EnemyBottom(GameObject enemy, Vector3 direction)
    {
        if (enemy == null) return;

        Vector3 newPosition = enemy.transform.position;

        if (newPosition.y <= -9)
        {
            GameManager.Instance.GameOver();
        }
    }

    public void ReturnEnemy(GameObject enemy, GameObject prefab)
    {
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < cols; col++)
            {
                if (enemies[row, col] == enemy)
                {
                    enemies[row, col] = null;
                }
            }
        }

        GameManager.Instance.AddScore(enemy.GetComponent<EnemyScript>().ScoreData);

        enemy.GetComponent<EnemyScript>().ChangeSpriteDeadEnemy();

        reverseGrave--;
        if (reverseGrave <= 0)
        {
            GameManager.Instance.CompletedLevel();
        }

        StartCoroutine(ExplosionCoroutine(enemy, prefab));
    }

    IEnumerator ExplosionCoroutine(GameObject enemy, GameObject prefab)
    {
        if (MenuPause.IsPaused)
        {
            yield return null;
        }
        else
        {
            GameManager.Instance.isExploding = true;

            int duration = 17;

            while (duration > 0)
            {
                duration--;
                yield return new WaitForEndOfFrame();
            }

            enemyPool.ReturnToPool(enemy, prefab);

            GameManager.Instance.isExploding = false;
        }
    }

    private bool ReachedBoundery(GameObject enemy)
    {
        float xPos = enemy.transform.position.x;

        if (currentState == MoveState.MoveRight && xPos >= playerBoundaryX)
        {
            return true;
        }

        if (currentState == MoveState.MoveLeft && xPos <= -playerBoundaryX)
        {
            return true;
        }

        return false;
    }

    private EnemyData.EnemyType GetEnemyTypeForRow(int row, List<EnemyData.EnemyType> enemyTypes)
    {
        if (row == 0)
        {
            return enemyTypes[2];
        }
        else if (row <= 2)
        {
            return enemyTypes[1];
        }
        else
        {
            return enemyTypes[0];
        }
    }

    public void TextScore()
    {
        textScrore.text = ("Enemy Left : ") + reverseGrave.ToString();
    }
}
