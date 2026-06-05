using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Cosa fa il personaggio?
 Legge i valori dal primo script
 Gestisce movimento, salto, rotazione, gravità
 Non sa nulla di tastiera o input
 *
 */
public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultipler = 3.0f;
    [HideInInspector] public float moltiplicatoreVelocita = 1f; // 1 significa velocità normale

    [Header("Jump Parameters")]
    [SerializeField] private float JumpForce = 5.0f;
    [SerializeField] private float grevityMultipler = 1.0f;

    [Header("Look Parameters")]
    public float mouseSesitivity = 0.1f; // Resa pubblica per OpzioniManager
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;

    private Vector3 currentMovement;
    private float verticalRotation;

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTrigger ? sprintMultipler : 1) * moltiplicatoreVelocita;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSesitivity = PlayerPrefs.GetFloat("Sensibilita", 0.1f);
    }

    void Update()
    {
        HandleMovement();
        HandleRotation();
    }

    private Vector3 CalculateWorldDirection() // Calcola solo la direzione — non muove nulla.
    {
        // L'input del giocatore arriva come Vector2 (x = sinistra/destra, y = avanti/indietro)
        // Lo convertiamo in Vector3 perché il mondo di gioco è 3D.
        // L'asse Y lo mettiamo a 0 perché WASD non controlla il su/giù.
        Vector3 inputDirection = new Vector3(
            playerInputHandler.MovementInput.x,   // sinistra / destra
            0f,                                   // su / giù (sempre 0)
            playerInputHandler.MovementInput.y    // avanti / indietro
        );

        // TransformDirection converte la direzione da "locale" a "mondiale".
        // Senza questa riga, premere W andrebbe sempre verso il Nord della scena.
        // Con questa riga, il giocatore va nella direzione verso cui sta guardando.
        Vector3 worldDirection = transform.TransformDirection(inputDirection);

        // normalized ridimensiona il vettore in modo che abbia sempre lunghezza 1.
        // Senza questo, muoversi in diagonale (es. W + D) sarebbe più veloce.
        return worldDirection.normalized;
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (playerInputHandler.JumpTrigger)
            {
                currentMovement.y = JumpForce;
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * grevityMultipler * Time.deltaTime;
        }
    }

    private void HandleMovement()
    {
        Vector3 wordlDirection = CalculateWorldDirection();

        currentMovement.x = wordlDirection.x * CurrentSpeed;
        currentMovement.z = wordlDirection.z * CurrentSpeed;

        HandleJumping(); // Gestisco la Y separatamente
        characterController.Move(currentMovement * Time.deltaTime);
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0); // Asse Y per la rotazione destra/sinistra
    }

    private void ApplyVeritcalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSesitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSesitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVeritcalRotation(mouseYRotation);
    }
}