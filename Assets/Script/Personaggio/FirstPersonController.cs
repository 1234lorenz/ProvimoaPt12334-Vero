using System.Collections;
using System.Collections.Generic;
using Unity.Properties;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


/*
 * 
 Cosa fa il personaggio?"
Legge i valori dal primo script
Gestisce movimento, salto, rotazione, gravità
Non sa nulla di tastiera o input
 */
public class FirstPersonController : Personaggio
{
    [Header("Movement Speed")]
    [SerializeField] private float walkSpeed = 3.0f;
    [SerializeField] private float sprintMultipler = 3.0f;

    [Header("Jump Parameters")]
    [SerializeField] private float JumpForce = 5.0f;
    [SerializeField] private float grevityMultipler = 1.0f;

    [Header("Look Parameters")]
    [SerializeField] private float mouseSesitivity = 0.1f;
    [SerializeField] private float upDownLookRange = 80f;

    [Header("Reference")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputHandler playerInputHandler;
    [SerializeField] private CameraSheke camera_shake;


    private Vector3 currentMovement;
    private float verticalRotation;
    float original_difesa;

    [Header("Reference Volume")]
    [SerializeField] private Volume volume;
    private Vignette effetti_danno;

    //colore orginale
    private Color original_value;
    private float CurrentSpeed => walkSpeed * (playerInputHandler.SprintTrigger ? sprintMultipler : 1);
    //[SerializeField] private Animator Player_animator;


    void Awake()
    {
        volume.profile.TryGet<Vignette>(out effetti_danno);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        original_difesa = Defense;
        original_value = effetti_danno.color.value;


    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
      
    }

    private Vector3 CalculateWorldDirection()//calcola solo la direzione -- non muove nulla.

    /*
     Il risultato viene poi usato in HandleMovement dove lo moltiplichi per la velocità e lo metti dentro currentMovement. Lì avviene il movimento reale!

     */
    {
        // L'input del giocatore arriva come Vector2 (x = sinistra/destra, y = avanti/indietro)
        // Lo convertiamo in Vector3 perché il mondo di gioco è 3D.
        // L'asse Y lo mettiamo a 0 perché WASD non controlla il su/giù.

        Vector3 inputDirection = new Vector3(//rende i valori grezzi dalla tastiera (WASD) e li mette in un Vector3 quindi per il reale movimento 
            playerInputHandler.MovementInput.x,   // sinistra / destra
            0f,                                    // su / giù (sempre 0)
            playerInputHandler.MovementInput.y    // avanti / indietro
        );

        // TransformDirection converte la direzione da "locale" a "mondiale".
        // Senza questa riga, premere W andrebbe sempre verso il Nord della scena.
        // Con questa riga, il giocatore va nella direzione verso cui sta guardando.
        Vector3 worldDirection = transform.TransformDirection(inputDirection);//onverte quella direzione in base a dove stai guardando

        // normalized ridimensiona il vettore in modo che abbia sempre lunghezza 1.
        // Senza questo, muoversi in diagonale (es. W + D) sarebbe più veloce
        // che muoversi dritto, perché la combinazione dei due assi darebbe un vettore più lungo.
        return worldDirection.normalized;//fa sì che la velocità sia sempre uguale in tutte le direzioni
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

        HandleJumping();//gestisco y separatamnete 
        characterController.Move(currentMovement * Time.deltaTime);

        //characterController.Move() è un metodo di Unity che muove fisicamente il player nella scena — senza questo il player non si muoverebbe mai!

        //currentMovement è il Vector3 che abbiamo costruito:
    }

    private void ApplyHorizontalRotation(float rotationAmount)
    {
        transform.Rotate(0, rotationAmount, 0);//lo abbaimo messo su Y perche la rotazione a destra e a sinsitra vien fatta con Y 
    }

    private void ApplyVeritcalRotation(float rotationAmount)
    {
        verticalRotation = Mathf.Clamp(verticalRotation - rotationAmount, -upDownLookRange, upDownLookRange);
        //Mathf.Clamp serve a bloccare un valore dentro un range — non lo lascia andare oltre un minimo e un massimo
        mainCamera.transform.localRotation = Quaternion.Euler(verticalRotation, 0, 0);
    }

    private void HandleRotation()
    {
        float mouseXRotation = playerInputHandler.RotationInput.x * mouseSesitivity;
        float mouseYRotation = playerInputHandler.RotationInput.y * mouseSesitivity;

        ApplyHorizontalRotation(mouseXRotation);
        ApplyVeritcalRotation(mouseYRotation);
    }
    private void OnControllerColliderHit(ControllerColliderHit hit)
    {


        if (hit.gameObject.tag == "Difesa")
        {
            Defense = original_difesa;
            effetti_danno.color.value = original_value;
            effetti_danno.intensity.value = 0f;
            effetti_danno.smoothness.value = 0f;

        }
    }

    public void SubireDanno(float danno)
    {
        float difesa = Defense;

        if (Defense > 0)
        {
            Defense -= danno;
            if (Defense >= 10 && Defense <= 20)
            {
                effetti_danno.color.value = new Color32(92, 21, 21, 255);
                effetti_danno.intensity.value = 0.352f;
                effetti_danno.smoothness.value = 0.433f;

            }
            if (Defense <= 0)
            {
                camera_shake.StartCoroutine(camera_shake.Shake());

                float accumolo = danno - difesa;
                HP -= accumolo;
            }


        }
        else
        {
            HP -= danno;
            effetti_danno.intensity.value = 0.617f;
            effetti_danno.smoothness.value = 0.734f;

        }

        //metodo pe ril game over 
        GameOver();
    }

    private void GameOver()
    {
        if (HP <= 0)
        {
            Time.timeScale = 0;
            Cursor.lockState = CursorLockMode.None;
            this.enabled = false;//per disabilitare questo codice 
            playerInputHandler.enabled = false;//per disabilitare il playerinput
            characterController.enabled = false;//per disabilitare il characyter controller
            Debug.Log("Game over");

        }
    }



}
