using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float speed = 6f;
    public float maxHeight = -10f;

    private SpriteRenderer spriteRenderer;

    public Sprite laserDeath;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < maxHeight)
        {
            StartCoroutine(LaserDeath());
        }
    }

    public IEnumerator LaserDeath()
    {
        if (MenuPause.IsPaused)
        {
            yield return null;
        }
        else
        {
            int duration = 25;

            speed = 0;

            while (duration > 0)
            {
                spriteRenderer.sprite = laserDeath;

                duration--;
                yield return new WaitForEndOfFrame();
            }

            ResetMissile();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.LoseLives();

            ResetMissile();
        }
        else if (collision.CompareTag("Missile"))
        {
            ResetMissile();
        }
    }

    public void ResetMissile()
    {
        gameObject.SetActive(false);
    }
}

