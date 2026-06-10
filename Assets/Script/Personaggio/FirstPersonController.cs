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
    public GameObject thirdPersonCam;
    public GameObject firstPersonCam;
    public Transform headTarget;
    public bool isFirstPerson = false;

    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private Animator animator;

    private Vector3 currentMovement;
    private float currentSpeed = 0f;
    private float verticalRotation;

    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTrigger ? sprintMultipler : 1) * moltiplicatoreVelocita;
    public GameObject UiPanelCancello_apri;
    [Header("AnimationCancello")]
    public Animator gateAnimator;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSesitivity = PlayerPrefs.GetFloat("Sensibilita", 0.1f);

        UpdateCameraState();

        // Evita movimenti strani della telecamera all'avvio della scena
        Cinemachine.CinemachineCore.Instance.OnTargetObjectWarped(transform, Vector3.zero);
        UiPanelCancello_apri.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.V))
        {
            isFirstPerson = !isFirstPerson;
            UpdateCameraState();
        }

        if (isFirstPerson)
        {
            HandleFirstPersonLogic();
        }
        else
        {
            HandleThirdPersonLogic();
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            bool state = gateAnimator.GetBool("isOpen");
            gateAnimator.SetBool("isOpen", !state);
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

    private void HandleFirstPersonLogic()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSesitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSesitivity;

        transform.Rotate(0, mouseXRotation, 0);

        verticalRotation -= mouseYRotation;
        verticalRotation = Mathf.Clamp(verticalRotation, -upDownLookRange, upDownLookRange);
        if (headTarget != null)
        {
            headTarget.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
        }

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

    // --- NUOVA FUNZIONE PER IL CARICAMENTO DELLA PARTITA ---
    public void Teletrasporta(Vector3 nuovaPosizione)
    {
        // Disabilita temporaneamente il controller fisico
        characterController.enabled = false;

        // Calcola la distanza tra dove sei e dove devi andare (serve a Cinemachine)
        Vector3 deltaSpostamento = nuovaPosizione - transform.position;

        // Sposta fisicamente il giocatore
        transform.position = nuovaPosizione;

        // Ordina a Cinemachine di saltare istantaneamente per evitare l'effetto "volo"
        Cinemachine.CinemachineCore.Instance.OnTargetObjectWarped(transform, deltaSpostamento);

        // Riattiva il controller fisico
        characterController.enabled = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Cancello"))
        {
            UiPanelCancello_apri.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cancello"))
        {
            UiPanelCancello_apri.SetActive(false);
        }
    }
}