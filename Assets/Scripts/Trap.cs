using UnityEngine;

public class Trap : MonoBehaviour
{
    [SerializeField] private GameObject explosionPrefab;

    public void ExplodeAndDestroy()
    {
        Debug.Log("¡La trampa ha explotado!");

        if (explosionPrefab != null)
        {
            // 1. Instanciamos la explosión en la posición de la trampa
            GameObject explosionInstance = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            
            // 2. Opcional por código: si prefieres asegurarte mediante GetComponent explícito en la raíz o hijos:
            ParticleSystem ps = explosionInstance.GetComponent<ParticleSystem>();
            if (ps == null)
            {
                ps = explosionInstance.GetComponentInChildren<ParticleSystem>();
            }

            if (ps != null)
            {
                ps.Play();
            }
        }

        // Ocultar sprite y colisionador de la trampa
        SpriteRenderer sprite = GetComponentInChildren<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // Destruir la trampa
        Destroy(gameObject, 0.05f);
    }
}