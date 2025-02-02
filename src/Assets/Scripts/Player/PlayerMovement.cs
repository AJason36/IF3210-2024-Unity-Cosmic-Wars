﻿using UnityEngine;
using UnitySampleAssets.CrossPlatformInput;
using System.Collections;

namespace Nightmare
{
    public class PlayerMovement : MonoBehaviour
    {
        // Managers
        private StatisticsManager statisticsManager;

        public CharacterController controller;
        PlayerHealth playerHealth;
        Attack playerAttack;

        public float speed = 12f;
        public float initialSpeed;
        public float gravity = -9.81f * 4;
        public float jumpHeight = 3f;

        public Transform groundCheck;
        public float groundDistance = 0.4f;
        public LayerMask groundMask;
        public Animator animator;

        Vector3 velocity;
        bool isGrounded;
        bool isRunning;
        float actualSpeed;

        bool isSpeedBoosted = false;
        float speedBoostDuration = 15f;
        float speedBoostTimer = 0f;

        void Awake()
        {
            playerHealth = GetComponent<PlayerHealth>();
            playerAttack = GetComponent<Attack>();
            initialSpeed = speed; // Store the initial speed
        }

        void Start()
        {
            statisticsManager = StatisticsManager.Instance;
        }

        void Update()
        {
            isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            if (isGrounded && velocity.y < 0)
            {
                velocity.y = -2f;
                animator.SetBool("Jumping", false);
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            Vector3 move = transform.right * x + transform.forward * z;

            isRunning = Input.GetButton("Run");
            actualSpeed = isRunning ? speed : speed / 2;
            animator.SetBool("Running", isRunning);
            animator.SetBool("Walking", move.sqrMagnitude > 0.01f);

            // Actial moving
            controller.Move(move * actualSpeed * Time.deltaTime);

            // Distance traveled
            float distance = move.magnitude * actualSpeed * Time.deltaTime;
            statisticsManager.RecordDistanceTraveled(distance / 1000);

            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
                animator.SetBool("Jumping", true);
            }

            velocity.y += gravity * Time.deltaTime;
            controller.Move(velocity * Time.deltaTime);

            // Check for speed boost timer
            if (isSpeedBoosted)
            {
                speedBoostTimer -= Time.deltaTime;
                if (speedBoostTimer <= 0)
                {
                    speed = initialSpeed; // Reset speed to initial value
                    isSpeedBoosted = false;
                }
            }
        }

        public void ActivateBoost()
        {
            speed = 24f;
            initialSpeed = 24f;

        }

        public void ActivateSlowDown()
        {
            speed = 12f;
            initialSpeed = 12f;

        }

        void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("HealthOrb"))
            {
                playerHealth.DrinkHealthOrb();
                Destroy(other.gameObject); // Destroy the orb after use
            }
            else if (other.CompareTag("SpeedOrb"))
            {
                Destroy(other.gameObject); // Destroy the orb after use
                if (!isSpeedBoosted) // If speed boost is not already active, start it
                {
                    StartCoroutine(TemporarySpeedBoost());
                }
                else
                {
                    Debug.Log("Reeset timer");
                    speedBoostTimer = speedBoostDuration;
                }
            }else if(other.CompareTag("AttackOrb"))
            {
                Destroy(other.gameObject);
                playerAttack.DrinkAttackOrbs();
            }
        }

        IEnumerator TemporarySpeedBoost()
        {
            isSpeedBoosted = true;
            speed += 0.2f * initialSpeed;
            speedBoostTimer = speedBoostDuration;
            yield return new WaitForSeconds(speedBoostDuration);
            speed = initialSpeed; // Reset speed to initial value after duration
            isSpeedBoosted = false;
        }
    }
}