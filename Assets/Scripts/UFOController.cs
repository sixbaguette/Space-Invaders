using UnityEngine;

public class UFOController : MonoBehaviour
{
    [SerializeField] private float speed = 3f;
    [SerializeField] public AudioSource ufoSound;

    private Vector2 direction;
    private int spawnShotIndex;

    private void OnEnable()
    {
        spawnShotIndex = PlayerShotCounter.Instance.TotalShots;
        ufoSound.Play();
    }

    private void OnDisable()
    {
        ufoSound.Stop();
    }

    public void SetDirection(Vector2 dir)
    {
        direction = dir.normalized;
    }

    private void Update()
    {
        transform.Translate(direction * speed * Time.deltaTime);

        if (Mathf.Abs(transform.position.x) > 12f)
        {
            gameObject.SetActive(false);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("PlayerBullet")) return;

        int shotsAfterSpawn =
            PlayerShotCounter.Instance.TotalShots - spawnShotIndex;

        int score = GetScore(shotsAfterSpawn);
        GameManager.Instance.AddScore(score);

        other.gameObject.SetActive(false);
        gameObject.SetActive(false);
    }

    private int GetScore(int shots)
    {
        if (shots <= 1) return 300;
        if (shots == 2) return 150;
        return 100;
    }
}