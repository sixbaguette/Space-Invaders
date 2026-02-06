using UnityEngine;

public class PixelPerfectCollision : MonoBehaviour
{
    public SpriteRenderer shelterSprite; // spriterend du bouclier 
    private Texture2D shelterTexture;    // texture associé
    public GameObject maskPrefabMissile;        // prefab du missilesplash 
    public GameObject maskPrefabLaser;
    public GameObject maskPrefabBullet;
    public GameObject maskPrefabPlayerMissile;
    public float yOffset = 0f;           // offset vertical en espace monde 

    private void Start()
    {
        shelterTexture = Instantiate(shelterSprite.sprite.texture);

        shelterSprite.sprite = Sprite.Create(shelterTexture, shelterSprite.sprite.rect, new Vector2(0.5f, 0.5f), shelterSprite.sprite.pixelsPerUnit);

        if (!shelterTexture.isReadable)
        {
            Debug.LogError("La texture du shelter doit être lisible");
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Missile"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le missile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiatePlayerMissileMaskAtPosition(worldImpactPoint);
                
                // resetMissile
                StartCoroutine(collision.gameObject.GetComponent<PlayerMissile>()?.PlayerMissileDeath());
            }
        }

        if (collision.CompareTag("EnemyMissile"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le missile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateEnemyMissileMaskAtPosition(worldImpactPoint);

                // resetMissile
                StartCoroutine(collision.gameObject.GetComponent<EnemyMissile>()?.MissileDeath());
            }
        }

        if (collision.CompareTag("EnemyLaser"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le missile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateEnemyLaserMaskAtPosition(worldImpactPoint);

                // resetMissile
                StartCoroutine(collision.gameObject.GetComponent<EnemyLaser>()?.LaserDeath());
            }
        }

        if (collision.CompareTag("EnemyBullet"))
        {
            BoxCollider2D missileCollider = collision.GetComponent<BoxCollider2D>();
            if (missileCollider == null)
            {
                Debug.Log("Le missile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateEnemyBulletMaskAtPosition(worldImpactPoint);

                // resetMissile
                StartCoroutine(collision.gameObject.GetComponent<EnemyBullet>()?.BulletDeath());
            }
        }

        if (collision.CompareTag("Enemy"))
        {
            BoxCollider2D enemyCollider = collision.GetComponent<BoxCollider2D>();
            if (enemyCollider == null)
            {
                Debug.Log("Le missile n'a pas de Collider2D");
                return;
            }

            if (IsPixelHitAndModify(enemyCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint))
            {
                InstantiateEnemyBulletMaskAtPosition(worldImpactPoint);
            }
        }
    }

    private bool IsPixelHitAndModify(BoxCollider2D missileCollider, out Vector2 worldImpactPoint, out Vector2 uvImpactPoint)
    {
        worldImpactPoint = Vector2.zero;
        uvImpactPoint = Vector2.zero;

        // .bounds vous permet de récupéré les limite d'un collider
        Bounds missileBounds = missileCollider.bounds;

        Vector3 bottomLeft = shelterSprite.transform.InverseTransformPoint(missileBounds.min);

        Vector3 topRight = shelterSprite.transform.InverseTransformPoint(missileBounds.max);

        bottomLeft.y += yOffset;
        topRight.y += yOffset;

        Bounds spriteBounds = shelterSprite.sprite.bounds;

        // prendre en compte les dimension de la texture et le rect du sprite 
        Rect textureRect = shelterSprite.sprite.textureRect;
        
        // normaliser les coordonnée du missile dans l'espace UV
        float uMin = (bottomLeft.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMin = (bottomLeft.y - spriteBounds.min.y) / spriteBounds.size.y;
        float uMax = (topRight.x - spriteBounds.min.x) / spriteBounds.size.x;
        float vMax = (topRight.y - spriteBounds.min.y) / spriteBounds.size.y;

        // vérifier si les UV du missile sont dans les limite de la texture
        uMin = Mathf.Clamp01(uMin);
        vMin = Mathf.Clamp01(vMin);
        uMax = Mathf.Clamp01(uMax);
        vMax = Mathf.Clamp01(vMax);

        // déterminer le point d'impact UV 
        uvImpactPoint = new Vector2((uMin + uMax) / 2f, (vMin + vMax) / 2f);

        worldImpactPoint = shelterSprite.transform.TransformPoint(new Vector3(spriteBounds.min.x + uvImpactPoint.x * spriteBounds.size.x, spriteBounds.min.y + uvImpactPoint.y * spriteBounds.size.y, 0));

        bool pixelModified = false;
        for (float u = uMin; u <= uMax; u += 1.0f / shelterTexture.width)
        {
            for (float v = vMin; v <= vMax; v += 1.0f / shelterTexture.height)
            {
                int x = Mathf.FloorToInt(textureRect.x + u * textureRect.width);
                int y = Mathf.FloorToInt(textureRect.y + v * textureRect.height);

                // vérifier si les coordonnée de la texture sont valide aka "actif"
                if (x >= 0 && x < shelterTexture.width && y >= 0 && y < shelterTexture.height)
                {
                    Color pixel = shelterTexture.GetPixel(x, y);

                    if (pixel.a > 0)
                    {
                        shelterTexture.SetPixel(x, y, new Color(0, 0, 0, 0));
                        pixelModified = true;
                    }
                }
            }
        }

        if (pixelModified)
        {
            shelterTexture.Apply();
        }

        return pixelModified;
    }

    private void InstantiatePlayerMissileMaskAtPosition(Vector2 worldPosition)
    {
        GameObject maskInstance = Instantiate(maskPrefabPlayerMissile, worldPosition, Quaternion.identity);
        maskInstance.transform.position = new Vector3(worldPosition.x, worldPosition.y, shelterSprite.transform.position.z);
    }

    private void InstantiateEnemyMissileMaskAtPosition(Vector2 worldPosition)
    {
        GameObject maskInstance = Instantiate(maskPrefabMissile, worldPosition, Quaternion.identity);
        maskInstance.transform.position = new Vector3(worldPosition.x, worldPosition.y, shelterSprite.transform.position.z);
    }

    private void InstantiateEnemyLaserMaskAtPosition(Vector2 worldPosition)
    {
        GameObject maskInstance2 = Instantiate(maskPrefabLaser, worldPosition, Quaternion.identity);
        maskInstance2.transform.position = new Vector3(worldPosition.x, worldPosition.y, shelterSprite.transform.position.z);
    }

    private void InstantiateEnemyBulletMaskAtPosition(Vector2 worldPosition)
    {
        GameObject maskInstance3 = Instantiate(maskPrefabBullet, worldPosition, Quaternion.identity);
        maskInstance3.transform.position = new Vector3(worldPosition.x, worldPosition.y, shelterSprite.transform.position.z);
    }
}
