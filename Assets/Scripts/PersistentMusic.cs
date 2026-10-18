using UnityEngine;

public class PersistentMusic : MonoBehaviour
{
    private static PersistentMusic instance;

    void Awake()
    {
        // Comprobamos si ya existe una música reproduciéndose
        if (instance == null)
        {
            instance = this;
            // Hacemos que este objeto no se destruya al cargar o reiniciar escenas
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            // Si ya había uno anterior (por ejemplo, si vuelves al menú principal y reentras), destruimos este duplicado
            Destroy(gameObject);
        }
    }
}