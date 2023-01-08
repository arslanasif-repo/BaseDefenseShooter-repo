using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;
    
    [SerializeField] private LevelManager LevelManager;
    [SerializeField] private UIManager UIhandler;

    [Header("Players properties")] 
    public PlayerController mainPlayer;
    public GameObject playerCam;
    public bool isGameStarted;
    public bool playerReady;





    [Header("Settings")]
    public bool GameOverCheck = false;
    public string RestartScene;
    public int DesiredLevel, DesiredItem;

    private int killedEnemies;
    private int totalEnemies;
    private bool allEnemiesKilled;
    
    public GameObject confettiParticles;


    public void LoadLevel()
    {
        LevelManager.LoadLevel(PlayerPrefs.GetInt("CurrentLevel", 0));
       
        UIhandler.currentLevel.text = (PlayerPrefs.GetInt("DummyLevel", 0) + 1).ToString();
        
        UIhandler.nextLevel.text = (PlayerPrefs.GetInt("DummyLevel", 0) + 2).ToString();

        totalEnemies = GameObject.FindGameObjectsWithTag("Enemy").Length;
    }

    public void Pause()
    {
        Debug.Log("Pause");
        if (!GameOverCheck)
        {
            Time.timeScale = 0;
        }
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
    }

    public void StartGame()
    {
        if (!isGameStarted)
        {
            if(UIhandler.PlayButton)
                UIhandler.PlayButton.SetActive(false);
            
            if (UIhandler.Gameplay)
                UIhandler.Gameplay.SetActive(true);
            
            playerCam.SetActive(true);
            ResumeGame();
            isGameStarted = true;
        }
    }


    private void Awake()
    {
        Time.timeScale = 1.0f;
        instance = this;
        LoadLevel();
    }


    public void InitializePlayer()
    {
        mainPlayer.gameObject.SetActive(true);
        playerReady = true;
        UIhandler.PlayButton.SetActive(true);
    }


    public void MissionComplete()
    {
        allEnemiesKilled = true;
    }

    public void LevelComplete()
    {
        if (!GameOverCheck && allEnemiesKilled)
        {
            if(confettiParticles)
                confettiParticles.SetActive(true);
            GameOverCheck = true;
            if (UIhandler.CompleteText)
                UIhandler.CompleteText.text = "LEVEL " + (PlayerPrefs.GetInt("CurrentLevel") + 1);
            PlayerPrefs.SetInt("CurrentLevel", PlayerPrefs.GetInt("CurrentLevel") + 1);
            PlayerPrefs.SetInt("DummyLevel", PlayerPrefs.GetInt("DummyLevel") + 1);

            if (PlayerPrefs.GetInt("CurrentLevel") >= LevelManager.Levels.Length)
            {
                if (UIhandler.CompleteText)
                    UIhandler.CompleteText.text =
                        "Game complete! Play again";
                PlayerPrefs.SetInt("CurrentLevel", 0);
            }

            StartCoroutine(GameOver(0.5f, true));
        }
    }

    IEnumerator GameOver(float timer, bool complete)
    {
        yield return new WaitForSecondsRealtime(timer);

        UIhandler.Gameplay.SetActive(false);
        if (complete)
        {
            UIhandler.CompletePanel.SetActive(true);
            HapptinManager.instance.SuccesVibrate();
        }
        else
        {
            UIhandler.FailPanel.SetActive(true);
            HapptinManager.instance.FailVibrate();
        }

        yield return new WaitForSeconds(timer);
        Time.timeScale = 0.0f;
    }

    public void LevelFail()
    {
        if (!GameOverCheck)
        {
            GameOverCheck = true;
            StartCoroutine(GameOver(0.5f, false));
        }
    }
    


    public bool CheckCompletion()
    {
        killedEnemies++;
        UIhandler.progressBar.fillAmount = (float) killedEnemies / totalEnemies;
        HapptinManager.instance.LowVibrate();
        if (killedEnemies == totalEnemies)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    


    public void RestartGame()
    {
        StartCoroutine(restartGame(0.5f));
    }

    IEnumerator restartGame(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        SceneManager.LoadScene(RestartScene);
    }
    
    
    [ContextMenu("SetDesiredLevel")]
    public void SetDesiredLevel(){
        PlayerPrefs.SetInt("CurrentLevel", DesiredLevel);
    }

}

[Serializable]
public class LevelDescription
{
    public GameObject level;
    public Vector3 spawn;
}



