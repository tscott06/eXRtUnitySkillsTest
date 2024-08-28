using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviourSingleton<GameManager>
{
    [SerializeField] InputActionProperty pauseAction;

    [Header("Main components")]
    [SerializeField] WorldGenerator worldGenerator;

    [Header("UI")]

    [SerializeField] Canvas crossHair;
    [SerializeField] Canvas pauseMenu;
    [SerializeField] Canvas deathScreen;

    bool isPaused;
    bool canPause = true;


    private void Start()
    {
      

        worldGenerator.GenerateGrid();

        Unpause();
        deathScreen.gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        pauseAction.action.started += PauseInputHandler;
    }

    private void OnDisable()
    {
        pauseAction.action.started -= PauseInputHandler;
    }

    private void PauseInputHandler(InputAction.CallbackContext obj)
    {
        TogglePause();
    }

    public void TogglePause()
    {
        if (!canPause)
            return;

        if (!isPaused)
            Pause();
        else
            Unpause();
    }



    public void Pause()
    {
        if (!canPause)
            return;

        isPaused = true;

        crossHair.gameObject.SetActive(false);
        pauseMenu.gameObject.SetActive(true);

        Time.timeScale = 0;

        ShowCursor();
    }

    public void Unpause()
    {
        isPaused = false;

        crossHair.gameObject.SetActive(true);
        pauseMenu.gameObject.SetActive(false);

        Time.timeScale = 1;

        HideCursor();
    }

    public void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void DeathScreen()
    {
        canPause = false;

        Time.timeScale = .8f;
        deathScreen.gameObject.SetActive(true);

        ShowCursor();
    }

    public void ResetGame()
    {
        SceneManager.LoadScene("BaseScene", LoadSceneMode.Single);
    }
}
