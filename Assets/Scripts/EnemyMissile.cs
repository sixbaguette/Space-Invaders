using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class EnemyMissile : MonoBehaviour
{
    public float speed = 10f;
    public float maxHeight = -10f;

    private SpriteRenderer spriteRenderer;

    public Sprite missileDeath;

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < maxHeight)
        {
            StartCoroutine(MissileDeath());            
        }
    }

    public IEnumerator MissileDeath()
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
                spriteRenderer.sprite = missileDeath;

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

            StartCoroutine(MissileDeath());
        }
    }

    public void ResetMissile()
    {
        gameObject.SetActive(false);
    }
}

