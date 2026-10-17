using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{

    //Variables hiperturbo
    [Header("Hiperturbo (Tecla S)")]
    [SerializeField] private float hyperTurboDuration = 3f; // Lo que dura activo
    private bool isHyperTurboActive = false;
    private float hyperTurboTimer = 0f;
    private float hyperTurboCooldownTimer = 0f;

    //Variables despegue
    [Header("Propulsor Vertical / Cohete (Tecla W)")]
    [SerializeField] private float verticalThrustForce = 150f; // Fuerza del impulso hacia arriba
    [SerializeField] private float nitroDuration = 2f;       // Cuánto tiempo dura el impulso por uso
    private bool hasNitro = false;                            // Se activa cada 1000 puntos
    private bool isThrusting = false;
    private float nitroTimer = 0f;

    [SerializeField] private float torqueAmount = 1f;
    [SerializeField] private float bootsSpeed = 35f;
    [SerializeField, Min(0f)] private float decelerationRate = 10f;
    [SerializeField] private ParticleSystem snowEffect;
    [SerializeField] private ScoreManager scoreManager;

    SurfaceEffector2D surfaceEffector2D;
    Rigidbody2D rb;

    float baseSpeed;
    InputAction moveAction;
    Vector2 moveInput;
    float previousRotation; // Store the previous rotation of the player
    float totalRotation; // Store the total rotation of the player
    int flipCount; // Store the number of flips performed by the player

    int activePowerUpsCount; // Track the number of active power-ups

    bool hasStartedMoving;

    private bool canControlPlayer = true; // Flag to control player input

    public bool CanControlPlayer { get => canControlPlayer; set => canControlPlayer = value; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Activate the selected character model 
        transform.GetChild(0).GetChild(PlayerPrefs.GetInt("SelectedCharacter", 0)).gameObject.SetActive(true);

        moveAction = InputSystem.actions.FindAction("Move");
        rb = GetComponent<Rigidbody2D>();
        surfaceEffector2D = FindAnyObjectByType<SurfaceEffector2D>();
        baseSpeed = surfaceEffector2D.speed; // Store the initial speed of the SurfaceEffector2D
        surfaceEffector2D.speed = 0f;

        rb.simulated = false;
    }


    // Update is called once per frame
    void Update()
    {
        if (!canControlPlayer)
        {
            surfaceEffector2D.speed = 0f;
            return; // If player input is disabled, exit the method
        }

        PlayerTorque();
        BoostPlayer();
        CalculateFlips();

        HandleHyperTurbo();
        HandleVerticalThrust();

    }

    /// <summary>
    /// Calculates the number of flips the player has performed based on their rotation 
    /// </summary>
    private void CalculateFlips()
    {
        // Get the current rotation of the player in degrees
        float currentRotation = transform.rotation.eulerAngles.z;
        // Calculate the change in rotation since the last frame
        totalRotation += Mathf.DeltaAngle(previousRotation, currentRotation);

        if (Math.Abs(totalRotation) > 340)
        {
            flipCount++; // Increment the flip count if the total rotation exceeds 360 degrees

            scoreManager.AddScore(flipCount * 100);

            totalRotation = 0; // Reset the total rotation for the next flip            
        }

        previousRotation = currentRotation; // Update the previous rotation for the next frame

    }

    /// <summary>
    /// Applies torque to the player based on the horizontal input from the Move action.
    /// </summary>
    void PlayerTorque()
    {
        moveInput = moveAction.ReadValue<Vector2>();
        if (moveInput.x < 0)
        {
            rb.AddTorque(torqueAmount);
        }
        else if (moveInput.x > 0)
        {
            rb.AddTorque(-torqueAmount);
        }

    }

    void BoostPlayer()
    {
        if (IsSpacePressed())
        {
            if (!hasStartedMoving)
            {
                rb.simulated = true;
                hasStartedMoving = true;
            }
            surfaceEffector2D.speed = moveInput.y > 0 ? bootsSpeed : baseSpeed;
        }
        else if (hasStartedMoving)
        {
            float coastingSpeed = baseSpeed * 0.1f;
            surfaceEffector2D.speed = Mathf.MoveTowards(
                surfaceEffector2D.speed,
                coastingSpeed,
                decelerationRate * Time.deltaTime);
        }
        else
        {
            surfaceEffector2D.speed = 0f;
        }
    }

    private bool IsSpacePressed()
    {
        return Keyboard.current != null && Keyboard.current.spaceKey.isPressed;
    }

    // Detects when the player collides with the floor and plays the snow effect
    void OnCollisionEnter2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            snowEffect.Play();
        }
    }

    // Detects when the player exits the collision with the floor and stops the snow effect
    void OnCollisionExit2D(Collision2D collision)
    {
        int layerIndex = LayerMask.NameToLayer("Floor");
        if (collision.gameObject.layer == layerIndex)
        {
            snowEffect.Stop();
        }
    }

    public void ApplyPowerUp(PowerUpScriptableObject powerUpData)
    {
        activePowerUpsCount++; // Increment the count of active power-ups
        if (powerUpData.PowerUpType == "Speed")
        {
            baseSpeed += powerUpData.PowerUpValue; // Increase the base speed by the power-up value
            bootsSpeed += powerUpData.PowerUpValue; // Increase the boots speed by the power-up value
        }
    }

    public void DeactivatePowerUp(PowerUpScriptableObject powerUpData)
    {
        activePowerUpsCount--; // Decrement the count of active power-ups
        if (activePowerUpsCount == 0)
        {
            if (powerUpData.PowerUpType == "Speed")
            {
                baseSpeed -= powerUpData.PowerUpValue; // Decrease the base speed by the power-up value
                bootsSpeed -= powerUpData.PowerUpValue; // Decrease the boots speed by the power-up value
            }
        }
    }

    // Gestiona la activación del Hiperturbo cada 10 segundos con la tecla S
    void HandleHyperTurbo()
    {
        // Reducimos el tiempo de recarga si está en cooldown
        if (hyperTurboCooldownTimer > 0)
        {
            hyperTurboCooldownTimer -= Time.deltaTime;
        }

        // Activar con la tecla S si ha pasado el tiempo de recarga
        if (Keyboard.current != null && Keyboard.current.sKey.wasPressedThisFrame && hyperTurboCooldownTimer <= 0)
        {
            StartCoroutine(ActivateHyperTurboRoutine());
            hyperTurboCooldownTimer = 10f; // 10 segundos de recarga
        }

        // Gestionar duración del hiperturbo
        if (isHyperTurboActive)
        {
            hyperTurboTimer -= Time.deltaTime;
            if (hyperTurboTimer <= 0)
            {
                isHyperTurboActive = false;
            }
        }
    }

    System.Collections.IEnumerator ActivateHyperTurboRoutine()
    {
        isHyperTurboActive = true;
        hyperTurboTimer = hyperTurboDuration;
        Debug.Log("¡HIPERTURBO ACTIVADO!");
        yield return new WaitForSeconds(hyperTurboDuration);
    }

    // Detecta el choque contra las trampas
    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Trap"))
        {
            if (isHyperTurboActive)
            {
                // ¡Destruye la trampa y suma 100 puntos!
                Destroy(collision.gameObject);
                if (scoreManager != null)
                {
                    scoreManager.AddScore(100);
                }
            }
            else
            {
                // Si no llevas el hiperturbo... ¡Muerte igual que al volcar!
                CrashDetector detector = FindAnyObjectByType<CrashDetector>();
                if (detector != null)
                {
                    detector.TriggerCrash();
                }
                else
                {
                    // Plan B por si acaso no encuentra el detector
                    canControlPlayer = false;
                    surfaceEffector2D.speed = 0f;
                }
            }
        }
    }
    void HandleVerticalThrust()
    {
        // Comprobamos directamente si se pulsa la W en el teclado y tenemos nitro
        bool pressW = Keyboard.current != null && Keyboard.current.wKey.wasPressedThisFrame;

        if (pressW && hasNitro && !isThrusting)
        {
            isThrusting = true;
            hasNitro = false; // Consumimos la carga de Nitro

            // Usamos IMPULSE para que sea un cohete instantáneo hacia arriba
            rb.AddForce(Vector2.up * verticalThrustForce, ForceMode2D.Impulse);

            Debug.Log("¡COHETE ACTIVADO CON LA W!");
        }
    }

    // Método público para conceder Nitro desde el ScoreManager (cada 1000 puntos)
    public void GrantNitro()
    {
        hasNitro = true;
        Debug.Log("¡Nitro recargado! Listo para despegar con W.");
    }

    void FixedUpdate()
    {
        // Ampliamos el radio a 1.8f para que cubra todo el coche de un extremo a otro (morro y trasera)
        // Quitamos el filtro de capa para asegurarnos de que detecta cualquier trampa por su Tag
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 1.8f);

        foreach (Collider2D hit in hits)
        {
            if (hit != null && hit.CompareTag("Trap"))
            {
                if (isHyperTurboActive)
                {
                    // Si llevas el hiperturbo, destruye la trampa y suma puntos
                    Destroy(hit.gameObject);
                    if (scoreManager != null)
                    {
                        scoreManager.AddScore(100);
                    }
                }
                else
                {
                    // Si no, activamos la muerte oficial
                    CrashDetector detector = FindAnyObjectByType<CrashDetector>();
                    if (detector != null)
                    {
                        detector.TriggerCrash();
                    }
                    else
                    {
                        canControlPlayer = false;
                        surfaceEffector2D.speed = 0f;
                    }
                }
                break; // Salimos del bucle al detectar la trampa
            }
        }
    }
}


