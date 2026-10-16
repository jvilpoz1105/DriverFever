using UnityEngine;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI scoreText; 
    [SerializeField] private PlayerController playerController; // Arrastra aquí al Player desde el Inspector

    private int currentScore = 0;
    private float timeCounter = 0f;

    void Update()
    {
        // Sumar 10 puntos automáticamente cada 1 segundo de supervivencia
        timeCounter += Time.deltaTime;
        if (timeCounter >= 1f)
        {
            AddScore(10);
            timeCounter = 0f;
        }
    }

    public void AddScore(int amount)
    {
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