using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    public void ExplodeAndDestroy()
    {
        Debug.Log("¡La trampa ha explotado!");

        if (explosionPrefab != null)
        {
            // 1. Instanciamos la explosión
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            // 2. Forzamos la reproducción de las partículas por código para que salgan sí o sí al nacer
            ParticleSystem ps = explosionInstance.GetComponentInChildren<ParticleSystem>();
            if (ps != null)
            {
                ps.Play();
            }
        }

        // Ocultar sprite y colisionador
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        Destroy(gameObject, 0.05f);
    }
}