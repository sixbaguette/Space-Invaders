using System.Collections;
using System.Threading;
using TMPro;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;

    private Vector2 direction;
    private int spawnShotIndex;

    private SpriteRenderer spriteRenderer;

    public Sprite UFODeathSprite;

    public TextMeshProUGUI scoreUFODeathTMP;

    private int scoreUFODeath;

    public Sprite UFOSprite;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    public void Initialize(int currentShots)
    {
        spawnShotIndex = currentShots;
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        if (MenuPause.IsPaused || GameManager.Instance.isExploding) return;
        transform.Translate(direction * speed * Time.deltaTime);

        spriteRenderer.sprite = UFOSprite;

        if (Mathf.Abs(transform.position.x) > 12f)
        {
            UFOManager manager = FindFirstObjectByType<UFOManager>();
            if (manager != null)
                manager.OnUFODespawned();

            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Missile")) return;

        int shotsAfterSpawn = PlayerShotCounter.Instance.TotalShots - spawnShotIndex;
        scoreUFODeath = shotsAfterSpawn <= 1 ? 300 : (shotsAfterSpawn == 2 ? 150 : 100);

        GameManager.Instance.AddScore(scoreUFODeath);

        other.gameObject.SetActive(false);

        UFOManager manager = FindFirstObjectByType<UFOManager>();
        if (manager != null)
            manager.OnUFODespawned();

        StartCoroutine(UFODeath());
    }

    private IEnumerator UFODeath()
    {
        GameManager.Instance.isExploding = true;

        int explosion = 150;
        for (int i = 0; i < explosion; i++)
        {
            spriteRenderer.sprite = UFODeathSprite;
            yield return new WaitForEndOfFrame();
        }

        int score = 100;
        scoreUFODeathTMP.text = scoreUFODeath.ToString();
        spriteRenderer.sprite = null;
        for (int i = 0; i < score; i++)
        {
            yield return new WaitForEndOfFrame();
        }

        scoreUFODeathTMP.text = null;
        gameObject.SetActive(false);
        GameManager.Instance.isExploding = false;
    }
}