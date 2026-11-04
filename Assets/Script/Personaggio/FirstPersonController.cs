using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

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

    [Header("UI Element")]
    public GameObject UiPanelCancello_apri;
    public TextMeshProUGUI testoCancelloUI; // Riferimento al testo del pannello

    [Header("AnimationCancello (Automatico)")]
    public Animator gateAnimator;

    void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        mouseSesitivity = PlayerPrefs.GetFloat("Sensibilita", 0.1f);

        UpdateCameraState();

        if (UiPanelCancello_apri != null)
        {
            UiPanelCancello_apri.SetActive(false);
            // Ricerca automatica del componente TextMeshPro se non assegnato a mano
            if (testoCancelloUI == null)
            {
                testoCancelloUI = UiPanelCancello_apri.GetComponentInChildren<TextMeshProUGUI>();
            }
        }
    }

    void Update()
    {
        if (MinigameBase.AnyRunning) return;
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

        // GESTIONE INTEGRATA DEL SUONO E DEL TESTO SULLA PRESSIONE DEL TASTO T
        if (Input.GetKeyDown(KeyCode.T) && gateAnimator != null)
        {
            bool state = gateAnimator.GetBool("isOpen");

            // Invertiamo lo stato logico (se era aperto ora si chiude, e viceversa)
            bool nuovoStato = !state;
            gateAnimator.SetBool("isOpen", nuovoStato);

            // Aggiorna subito la scritta a schermo
            AggiornaTestoCancello();

            // Richiamo dei suoni in base alla nuova direzione dell'animazione
            if (AudioManager.Instance != null)
            {
                if (nuovoStato)
                {
                    AudioManager.Instance.PlayCancelloApri();
                }
                else
                {
                    AudioManager.Instance.PlayCancelloChiudi();
                }
            }
        }
    }

    private void UpdateCameraState()
    {
        if (thirdPersonCam != null) thirdPersonCam.SetActive(!isFirstPerson);
        if (firstPersonCam != null) firstPersonCam.SetActive(isFirstPerson);
    }

    private void HandleJumping()
    {
        if (animator != null) animator.SetBool("isGrounded", characterController.isGrounded);

        if (characterController.isGrounded)
        {
            currentMovement.y = -0.5f;

            if (playerInputHandler.JumpTrigger)
            {
                currentMovement.y = JumpForce;
                if (animator != null) animator.SetTrigger("Jump");
            }
        }
        else
        {
            currentMovement.y += Physics.gravity.y * grevityMultipler * Time.deltaTime;
        }
    }

    private void HandleThirdPersonLogic()
    {
        if (Time.timeScale == 0f) return;

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
        if (animator != null) animator.SetFloat("Speed", currentSpeed);

        characterController.Move(currentMovement * Time.deltaTime);

        if (worldDirection.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(worldDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 15f);
        }
    }

    private void HandleFirstPersonLogic()
    {
        if (Time.timeScale == 0f) return;

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
        if (animator != null) animator.SetFloat("Speed", currentSpeed);

        characterController.Move(currentMovement * Time.deltaTime);
    }

    public void Teletrasporta(Vector3 nuovaPosizione)
    {
        characterController.enabled = false;
        Vector3 deltaSpostamento = nuovaPosizione - transform.position;
        transform.position = nuovaPosizione;
        Cinemachine.CinemachineCore.Instance.OnTargetObjectWarped(transform, deltaSpostamento);
        characterController.enabled = true;
    }

    private void AggiornaTestoCancello()
    {
        // Se non è stato assegnato a mano nell'Inspector, lo cerca automaticamente nel pannello UI
        if (testoCancelloUI == null && UiPanelCancello_apri != null)
        {
            testoCancelloUI = UiPanelCancello_apri.GetComponentInChildren<TextMeshProUGUI>();
        }

        if (testoCancelloUI == null || gateAnimator == null) return;

        bool isOpen = gateAnimator.GetBool("isOpen");
        if (isOpen)
        {
            testoCancelloUI.text = "Premi [T] per chiudere";
        }
        else
        {
            testoCancelloUI.text = "Premi [T] per aprire";
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Cancello"))
        {
            gateAnimator = other.GetComponentInParent<Animator>();
            if (UiPanelCancello_apri != null) UiPanelCancello_apri.SetActive(true);

            // Imposta il testo corretto appena ci si avvicina al cancello
            AggiornaTestoCancello();
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Cancello"))
        {
            if (UiPanelCancello_apri != null) UiPanelCancello_apri.SetActive(false);
            gateAnimator = null;
        }
    }
}