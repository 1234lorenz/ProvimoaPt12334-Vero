using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultipler = 2.0f;
    [HideInInspector] public float moltiplicatoreVelocita = 1f;

    [Header("Jump Parameters")]
    [SerializeField] private float JumpForce = 5.0f;
    [SerializeField] private float grevityMultipler = 1.0f;

    [Header("Look Parameters")]
    public float mouseSesitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("View Switch Setup")]
    public GameObject thirdPersonCam; // Trascina qui Cm_freeLook1
    public GameObject firstPersonCam; // Trascina qui la nuova VCam della 1a persona
    public Transform headTarget;      // Trascina qui il tuo CameraTarget
    public bool isFirstPerson = false; // Stato iniziale

    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Animator animator;

    private Vector3 currentMovement;
    private float currentSpeed = 0f;
    private float verticalRotation;

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTrigger ? sprintMultipler : 1) * moltiplicatoreVelocita;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSesitivity = PlayerPrefs.GetFloat("Sensibilita", 0.1f);

        // Assicuriamoci che all'avvio le telecamere siano nello stato giusto
        UpdateCameraState();
    }

    void Update()
    {
        // Switch della telecamera premendo 'V'
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
            UpdateCameraState();
        }

        // Scegli quale logica di movimento usare
        if (isFirstPerson)
        {
            HandleFirstPersonLogic();
        }
        else
        {
            HandleThirdPersonLogic();
        }
    }

    private void UpdateCameraState()
    {
        if (thirdPersonCam != null) thirdPersonCam.SetActive(!isFirstPerson);
        if (firstPersonCam != null) firstPersonCam.SetActive(isFirstPerson);
    }

    private void HandleJumping()
    {
        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;
            if (playerInputHandler.JumpTrigger)
            {
                currentMovement.y = JumpForce;
                animator.SetTrigger("Jump");
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * grevityMultipler * Time.deltaTime;
        }
    }

    private void ApplyGravity()
    {
        if (!characterController.isGrounded)
        {
            currentMovement.y += Physics.gravity.y * grevityMultipler * Time.deltaTime;
        }
    }

    // --- LOGICA TERZA PERSONA (Quella che abbiamo già sistemato) ---
    private void HandleThirdPersonLogic()
    {
        Vector3 camForward = mainCamera.transform.forward;
        Vector3 camRight = mainCamera.transform.right;
        camForward.y = 0;
        camRight.y = 0;
        camForward.Normalize();
        camRight.Normalize();

        Vector3 worldDirection = (camForward * playerInputHandler.MovementInput.y) + (camRight * playerInputHandler.MovementInput.x);
        worldDirection.Normalize();

        currentMovement.x = worldDirection.x * CurrentSpeed;
        currentMovement.z = worldDirection.z * CurrentSpeed;

        HandleJumping();

        float targetSpeed = worldDirection.sqrMagnitude > 0.01f ? (playerInputHandler.SprintTrigger ? 2f : 1f) : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);
        animator.SetFloat("Speed", currentSpeed);

        characterController.Move(currentMovement * Time.deltaTime);

        if (worldDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    // --- LOGICA PRIMA PERSONA ---
    private void HandleFirstPersonLogic()
    {
        // 1. Rotazione con il Mouse
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSesitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSesitivity;

        // Gira il corpo a destra/sinistra
        transform.Rotate(0, mouseXRotation, 0);

        // Inclina la testa (headTarget) su/giù
        verticalRotation -= mouseYRotation;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownLookRange, upDownLookRange);
        if (headTarget != null)
        {
            headTarget.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }

        // 2. Movimento
        Vector3 moveDirection = (transform.forward * playerInputHandler.MovementInput.y) + (transform.right * playerInputHandler.MovementInput.x);
        moveDirection.Normalize();

        currentMovement.x = moveDirection.x * CurrentSpeed;
        currentMovement.z = moveDirection.z * CurrentSpeed;

        HandleJumping();

        float targetSpeed = moveDirection.sqrMagnitude > 0.01f ? (playerInputHandler.SprintTrigger ? 2f : 1f) : 0f;
        currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * 10f);
        animator.SetFloat("Speed", currentSpeed);

        characterController.Move(currentMovement * Time.deltaTime);
    }
}