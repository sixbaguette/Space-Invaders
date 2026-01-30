using Unity.VisualScripting;
using UnityEngine;

public class EnemyLaser : MonoBehaviour
{
    public float speed = 6f;
    public float maxHeight = -10f;

    void Update()
    {
        transform.Translate(Vector3.down * speed * Time.deltaTime);
        if (transform.position.y < maxHeight) ResetMissile();
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

