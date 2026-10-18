using UnityEngine;
public class SelfDestruct : MonoBehaviour
{
    void Start()
    {
        // Se destruye automáticamente a los 2 segundos (o lo que dure tu efecto)
        Destroy(gameObject, 2f); 
    }
}
