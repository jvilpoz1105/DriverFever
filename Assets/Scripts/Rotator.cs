using UnityEngine;

public class Rotator : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 200f; // Velocidad de giro

    void Update()
    {
        // Rota el objeto sobre su propio eje Z (en 2D) cada fotograma
        transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
    }
}