using UnityEngine;

public class LevelSettings : MonoBehaviour
{
    [SerializeField] private float gravityY = -9.81f; // Ajustas esto desde el Inspector según el planeta

    void Awake()
    {
        // Aplica la gravedad específica para este nivel nada más empezar
        Physics2D.gravity = new Vector2(0f, gravityY);
    }
}