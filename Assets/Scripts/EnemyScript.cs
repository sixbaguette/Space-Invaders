using UnityEngine;

public class EnemyScript : MonoBehaviour
{
    public EnemyData.EnemyType enemyType;
    public int ScoreData;

    public Sprite sprite01;
    public Sprite sprite02;
    public Sprite explosionEnemy;
    private SpriteRenderer spriteRenderer;

    private bool isSprite01;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprite01;
            isSprite01 = true;
        }
        else
        {
            Debug.Log("[EnemyScipt] SpriteRenderer is not assigned");
        }
    }

    public void ChangeSprite()
    {
        if (MenuPause.IsPaused) return;
        isSprite01 = !isSprite01;
        // spriteRenderer.sprite = (condition) ? (si vrai) : (si faux);
        spriteRenderer.sprite = isSprite01 ? sprite01 : sprite02;
    }

    public void ChangeSpriteDeadEnemy()
    {
        if (MenuPause.IsPaused) return;
        spriteRenderer.sprite = explosionEnemy;
    }
}
