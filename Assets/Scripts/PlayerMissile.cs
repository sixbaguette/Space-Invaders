using System.Collections;
using UnityEngine;

public class PlayerMissile : MonoBehaviour
{
    public float speed = 10f;
    public float maxHeight = 10f;

    private SpriteRenderer spriteRenderer;

    public Sprite playerMissileDeath;

    public Sprite missileSprite;

    private bool isExploding = false;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isExploding) return;

        transform.Translate(Vector3.up * speed * Time.deltaTime);

        if (transform.position.y > maxHeight)
        {
            StartCoroutine(PlayerMissileDeath());
        }
    }

    public IEnumerator PlayerMissileDeath()
    {
        if (isExploding) yield break;
        isExploding = true;

        speed = 0;
        int duration = 25;

        while (duration > 0)
        {
            spriteRenderer.sprite = playerMissileDeath;
            duration--;
            yield return new WaitForEndOfFrame();
        }

        ResetMissile();
        isExploding = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
        {
            EnemyManager enemyManager = FindFirstObjectByType<EnemyManager>();
            if (enemyManager != null)
            {
                GameObject go = collision.GetComponent<EnemyScript>().enemyType.prefab;
                enemyManager.ReturnEnemy(collision.gameObject, go);

                ResetMissile();
            }
        }
        else if (collision.CompareTag("EnemyMissile") || collision.CompareTag("EnemyLaser") || collision.CompareTag("EnemyBullet"))
        {
            ResetMissile();
        }
    }

    public void ResetMissile()
    {
        spriteRenderer.sprite = missileSprite;
        speed = 25f;
        isExploding = false;
        gameObject.SetActive(false);
    }
}
