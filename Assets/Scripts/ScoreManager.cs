using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText; 
    [SerializeField] private PlayerController playerController;

    private int currentScore = 0;
    private float timeCounter = 0f;
    private bool isGameStarted = false; // Controla si el juego ha arrancado

    void Start()
    {
        // Mostrar mensaje inicial antes de empezar a jugar
        if (scoreText != null)
        {
            scoreText.text = "Ready, folks?";
        }
    }

    void Update()
    {
        // Si el juego no ha empezado, no sumamos puntos por tiempo
        if (!isGameStarted) return;

        // Sumar 10 puntos automáticamente cada 1 segundo de supervivencia
        timeCounter += Time.deltaTime;
        if (timeCounter >= 1f)
        {
            AddScore(10);
            timeCounter = 0f;
        }
    }

    // Método para que el PlayerController active los puntos al pulsar espacio
    public void StartGameScoring()
    {
        isGameStarted = true;
        currentScore = 0; // Reseteamos por si acaso
        scoreText.text = currentScore.ToString("00000");
    }

    public void AddScore(int amount)
    {
        if (!isGameStarted) return;

        int previousScore = currentScore;
        currentScore += amount;
        scoreText.text = currentScore.ToString("00000"); 

        // Comprobar si hemos alcanzado o superado un múltiplo de 1000 para dar el Nitro
        if (currentScore / 1000 > previousScore / 1000)
        {
            if (playerController != null)
            {
                playerController.GrantNitro();
            }
        }
    }
}