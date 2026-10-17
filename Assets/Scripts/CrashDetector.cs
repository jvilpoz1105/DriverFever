using UnityEngine;
using UnityEngine.SceneManagement;

public class CrashDetector : MonoBehaviour
{
    [SerializeField] private float crashDelay = 1f;
    [SerializeField] private ParticleSystem crashEffect;

    PlayerController playerController;

    void Start()
    {
        playerController = FindAnyObjectByType<PlayerController>();
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            // Solo se considera choque mortal si el coche está boca abajo (volcado)
            // transform.up.y < 0 significa que el coche está invertido
            if (transform.up.y < 0f)
            {
                if (playerController != null)
                {
                    playerController.CanControlPlayer = false;
                }
                
                if (crashEffect != null)
                {
                    crashEffect.Play();
                }
                
                Invoke(nameof(ReloadScene), crashDelay);
            }
        }
    }

    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void TriggerCrash()
    {
        if (playerController != null)
        {
            playerController.CanControlPlayer = false;
        }
        
        if (crashEffect != null)
        {
            crashEffect.Play();
        }
        
        Invoke(nameof(ReloadScene), crashDelay);
    }
}