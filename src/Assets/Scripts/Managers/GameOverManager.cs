﻿using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Nightmare
{
    public class GameOverManager : MonoBehaviour
    {
        [SerializeField] TextMeshProUGUI playerName;
        public SceneLevelManager sceneLevelManager;
        private bool loadStatistics;
        // private PlayerHealth playerHealth;
        // Animator anim;

        // LevelManager lm;
        // private UnityEvent listener;

        // void Awake ()
        // {
        //     playerHealth = FindObjectOfType<PlayerHealth>();
        //     anim = GetComponent <Animator> ();
        //     lm = FindObjectOfType<LevelManager>();
        //     EventManager.StartListening("GameOver", ShowGameOver);
        // }

        // void OnDestroy()
        // {
        //     EventManager.StopListening("GameOver", ShowGameOver);
        // }

        // void ShowGameOver()
        // {
        //     anim.SetBool("GameOver", true);
        // }

        // private void ResetLevel()
        // {
        //     ScoreManager.score = 0;
        //     LevelManager lm = FindObjectOfType<LevelManager>();
        //     lm.LoadInitialLevel();
        //     anim.SetBool("GameOver", false);
        //     playerHealth.ResetPlayer();
        // }

        public PlayerHealth playerHealth;
        public float restartDelay = 5f;

        Animator anim;
        float restartTimer;

        void Awake()
        {
            playerName.text = DataPersistenceManager.Instance.GetGameData().username;
            anim = GetComponent<Animator>();
            loadStatistics = false;
        }

        void Update()
        {
            if(playerHealth.getIsDead())
            {
                anim.SetTrigger("GameOver");
                restartTimer += Time.deltaTime;
                if(restartTimer >= restartDelay && !loadStatistics)
                {
                    loadStatistics = true;
                    Cursor.lockState = CursorLockMode.None;
                    sceneLevelManager.loadScene(6); // Statistics
                }
            }
        }

    }
}