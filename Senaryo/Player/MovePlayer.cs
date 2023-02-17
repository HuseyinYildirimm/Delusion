using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class MovePlayer : MonoBehaviour
{

    #region Move
    [Header("Movement")]
     CharacterController characterController;
    Vector3 moveVector;
    public float Speed = 15f;
    public bool isMoving;
    public bool isRunning;
    bool isRun = true;
    #endregion

    [Space()]

    #region Jump And Gravity
    [Header("JumpAndGravity")]
    public Transform groundCheck;
    public float groundDistance = 5f;
    public LayerMask groundMask;
    public float gravity = -9.80f;
    public float jumpHeight = 3f;
    Vector3 velocity;
    public bool isGrounded;

    #endregion

    [Space()]

    #region Crouch
    [Header("Crouch")]
    public float originalHeight;
    public float crouchHeight;
    public bool isCrouch;
    #endregion

    [Space()]

    #region Audio&&Steps
    [Header("Audio && Steps")]
    AudioSource walkingAS;
    public AudioClip[] stepSoundsAC;
    public float timeBeetweenSteps;
    float timer;
    #endregion

    [HideInInspector] public StaminaController staminaController;
    [HideInInspector] Pistol.Gun gunRun;
    public Camera cam;

    private void Start()
    {
        walkingAS = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        staminaController = GetComponent<StaminaController>();
        gunRun = GameObject.FindGameObjectWithTag("Gun").GetComponentInChildren<Pistol.Gun>();
       
    }


    void Update()
    {
        MoveCharacter();
    }

    public void SetRunSpeed(float rSpeed)
    {
        Speed = rSpeed;
    }

    public void MoveCharacter()
    {
        #region Movement

        moveVector = (Input.GetAxis("Horizontal") * transform.right + Input.GetAxis("Vertical") * transform.forward);
        characterController.Move(moveVector * Speed * Time.deltaTime);

        isRunning = Input.GetKey(KeyCode.LeftShift); 
        if (!isRunning)
        {
            staminaController.isShift = false;

        }
        if (isRunning && isMoving)
        {

            if (staminaController.playerstamina > 0 && isGrounded )
            {

                staminaController.isShift = true;
                staminaController.Sprinting();
               

                if (isRunning)
                {
                    Run();
                   
                }
                else return;

            }
            else
            {
                isRunning = false;

            }
        }

        #endregion

        #region Gravity 
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        
        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -15f;
        }
        velocity.y += gravity * Time.deltaTime;

        characterController.Move(velocity * Time.deltaTime);

        #endregion

        #region Jump
        if (Input.GetButtonDown("Jump") && isGrounded && !isCrouch)
        {

            walkingAS.Stop();
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
     
        #endregion

        #region Crouch
        if (Input.GetKeyDown(KeyCode.LeftControl) && isGrounded) //çömelme
        {
            characterController.height = crouchHeight;
           
            isCrouch = true;
            Speed = 10;

            groundCheck.transform.localPosition = new Vector3(-0.02f, -3.40f, 0.017f);

        }

        if (Input.GetKeyUp(KeyCode.LeftControl))
        {
            characterController.height = originalHeight;
            Speed = 15;
            isCrouch = false;

            groundCheck.transform.localPosition = new Vector3(-0.02f, -5.28f, 0.017f);
        }
        #endregion

        #region FootSteps
        if (moveVector.x != 0 || moveVector.y != 0)
            isMoving = true;
        else
            isMoving = false;

        if (isMoving && !isCrouch)
        {
            timer -= Time.deltaTime;
            if (timer <= 0)
            {
                timer = timeBeetweenSteps;

                walkingAS.clip = stepSoundsAC[Random.Range(0, stepSoundsAC.Length)];


                walkingAS.Play();

            }
        }
        else
        {
            walkingAS.Stop();
            timer = timeBeetweenSteps;
        }
        #endregion
        
    }

    public void Run()
    {
        isRun = Input.GetKeyUp(KeyCode.LeftShift);
        isRun = true;
    }
}

