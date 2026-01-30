using System;
using UnityEngine;

public class MissileScript : MonoBehaviour
{
    public Sprite[] sprites;

    private int index = 0;

    private SpriteRenderer spriteRenderer;

    private bool isSprite01;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            spriteRenderer.sprite = sprites[0];
        }
        else
        {
            Debug.Log("[EnemyScipt] SpriteRenderer is not assigned");
        }
    }

    public void ChangeMissileSprite()
    {
        switch (index)
        {
            case 0:
                spriteRenderer.sprite = sprites[0];
                break;
            case 1:
                spriteRenderer.sprite = sprites[1];
                break;
            case 2:
                spriteRenderer.sprite = sprites[2];
                break;
            case 3:
                spriteRenderer.sprite = sprites[3];
                break;
        }

        index++;
        index = index % 4;
    }

    private void FixedUpdate()
    {
        ChangeMissileSprite();
    }
}
