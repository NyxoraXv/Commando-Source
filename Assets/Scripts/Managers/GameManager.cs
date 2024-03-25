// This script is a Manager that controls the the flow and control of the game. It keeps
// track of player data (score, total game time) and interfaces with
// the UI Manager. All game commands are issued through the static methods of this class

using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //This class holds a static reference to itself to ensure that there will only be
    //one in existence. This is often referred to as a "singleton" design pattern. Other
    //scripts access this one through its public static methods
    static GameManager current;
    //static CurrencyManager currency;

    public enum Difficulty {Easy = 1, Medium = 2, Hard = 3 }
    public enum Missions { Home = 0, Mission1, Mission2, Mission3, Mission3Boss }

    float totalGameTime;                        //Length of the total game time
    public bool isGameOver;                            //Is the game currently over?
    int score = 0;
    int initialBombs = 10;
    int bombs;
    int ammo;
    Difficulty difficulty = Difficulty.Medium;
    float bgmAudio = 1f;
    float sfxAudio = 1f;
    Missions currentMission = Missions.Home;
    float mission1Points = 0f;
    float mission2Points = 0f;
    float mission3Points = 0f;
    private float frg;
    private float lunc;

    [Header("Layers")]
    public LayerMask enemyLayer;
    public LayerMask buildingLayer;
    public LayerMask walkableLayer;
    public LayerMask playerLayer;

    void Awake()
    {
        current = this;
    }

    void Start()
    {
        LoadSettings();
        LoadRecords();
        SaveRecords();
        //PlayerWin();
    }

    void Update()
    {
        //If the game is over, exit
        if (isGameOver)
            return;
        //Update the total game time and tell the UI Manager to update
        totalGameTime += Time.deltaTime;
        // UIManager.UpdateTimeUI(totalGameTime); // todo implement or delete
    }

    public static float getFRG()
    {
        return current.frg;
    }

    public static float getLUNC() {
        return current.lunc;
    }

    private void SaveSettings()
    {
        Settings settings = SaveManagerPlayer.GetSettings();
        if (settings == null)
            settings = new Settings();
        settings.bgmVolume = GetBgmAudio();
        settings.sfxVolume = GetSfxAudio();
        SaveManagerPlayer.SetSettings(settings);
    }

    private void LoadSettings()
    {
        Settings settings = SaveManagerPlayer.GetSettings();
        if (settings != null)
        {
            SetBgmAudio(settings.bgmVolume);
            SetSfxAudio(settings.sfxVolume);
            AudioManager.RefreshAudioVolume();
        }
    }

    private void SaveRecords()
    {
        Records records = SaveManagerPlayer.GetRecords();
        if (records == null)
            records = new Records();
        records.mission1Points = GetMission1Points();
        records.mission2Points = GetMission2Points();
        records.mission3Points = GetMission3Points();
        SaveManagerPlayer.SetRecords(records);
    }

    private void LoadRecords()
    {
        Records records = SaveManagerPlayer.GetRecords();
        if (records != null)
        {
            SetMission1Points(records.mission1Points);
            SetMission2Points(records.mission2Points);
            SetMission3Points(records.mission3Points);
            AudioManager.RefreshAudioVolume();
        }
    }

    public static bool ToggleGodMode()
    {
        var player = GetPlayer();
        if (player)
        {
            var health = player.GetComponent<Health>();
            health.immortal = !health.immortal;
            if (health.immortal)
            {
                SetBombs(200);
                return true;
            }
            else
            {
                SetBombs();
                return false;
            }
        }
        return false;
    }

    public static void AddScore(float amount)
    {
        AddRewardAll((int)amount);
    }

    public static void AddRewardAll(int amountScore = 0, float FRG = 0f, float LUNC = 0f, int xp = 0)
    {
        //If there is no current Game Manager, exit
        if (current == null)
            return;

        current.score += amountScore;
        //UIManager.DisplayCurrency();
        UIManager.UpdateScoreUI();
        AddReward(FRG, LUNC, xp);
        UIManager.refreshCurrency();
    }

    public static int GetScore()
    {
        //If there is no current Game Manager, return 0
        if (current == null)
            return 0;

        //Return the state of the game
        return current.score;
    }

    public static void AddReward(float FRG = 0F, float LUNC = 0f, int XP = 0)
    {
        if (SaveManager.Instance.isLogin)
        {
            current.lunc += LUNC;
            current.frg += FRG;
        }
        LevelManager.Instance.addXP(XP);
    }

    public static int GetBombs()
    {
        //If there is no current Game Manager, return 0
        if (current == null)
            return 10;

        //Return the state of the game
        return current.bombs;
    }

    public static void RemoveBomb()
    {
        //If there is no current Game Manager, exit
        if (current == null)
            return;

        current.bombs--;
        UIManager.UpdateBombsUI();
    }



    public static void SetBombs(int bombs = 10)
    {
        if (current == null)
            return;
        current.bombs = bombs;
        UIManager.UpdateBombsUI();
    }

    public static void addAmmo(int ammo)
    {
        current.ammo += ammo;
        UIManager.UpdateAmmoUI();
    }

    public static int getAmmo()
    {
        return current.ammo;
        UIManager.UpdateAmmoUI();
    }
    
    public static void spendAmmo(int ammo)
    {
        current.ammo -= ammo;
        UIManager.UpdateAmmoUI();
    }

    public static bool IsGameOver()
    {
        //If there is no current Game Manager, return false
        if (current == null)
            return false;

        //Return the state of the game
        return current.isGameOver;
    }

    public static void PlayerDied(string causeOfDeath)
{
    if (current == null)
        return;

    current.isGameOver = true;

    if (causeOfDeath == "Water Dead")
    {
        UIManager.DisplayGameOverText();
        UIManager.Home();
        UIManager.Restart();
    }
    else
    {
        UIManager.DisplayGameOverText();
        UIManager.Home();
        UIManager.Restart();
        UIManager.Continue();
    }

    // Play game over audio
    AudioManager.PlayGameOverAudio();
}


    public static void PlayerWin()
    {
        Debug.Log("Player Win");
        //If there is no current Game Manager, exit
        if (current == null)
            return;

        UIManager.DisplayWinUI();
        AudioManager.PlayLevelCompleteAudio();
        AudioManager.PlayGameOverAudio();
        if (MissionManager.Instance.onLoaded >= SaveManager.Instance.playerData.playerInformation.PlayerLastLevel) {
            SaveManager.Instance.playerData.playerInformation.PlayerLastLevel = MissionManager.Instance.onLoaded+1;
            SaveManager.Instance.playerData.playerInformation.PlayerScore = SaveManager.Instance.playerData.playerInformation.PlayerScore+current.score;
            CurrencyManager.Instance.addFRG(current.frg);
            CurrencyManager.Instance.addLUNC(current.lunc);
            SaveManager.Instance.Save();
        }
        current.isGameOver = true;
    }

    public static LayerMask GetBuildingLayer()
    {
        //If there is no current Game Manager, exit
        if (current == null)
            return 0;

        return current.buildingLayer;
    }

    public static GameObject GetPlayer() // not cached
    {
        if (current == null)
            return null;
        return GameObject.FindGameObjectWithTag("Player");
    }

    public static GameObject GetPlayer(GameObject player)
    {
        if (current == null)
            return null;
        if (player.GetComponent<MainPlayer>()) // return itself
            return player;
        else if (player.transform.parent.gameObject.GetComponent<MainPlayer>()) // return parent
            return player.transform.parent.gameObject;
        return GameObject.FindGameObjectWithTag("Player"); // return uncached finded by tag
    }

    public static GameObject GetPlayer(Collider2D collider)
    {
        return GetPlayer(collider.gameObject);
    }

    public static GameObject GetPlayer(Collision2D collision)
    {
        return GetPlayer(collision.collider);
    }

    public static GameObject GetRunningTarget() // not cached
    {
        //If there is no current Game Manager, exit
        if (current == null)
            return null;

        return GameObject.FindGameObjectWithTag("RunningTarget");
    }

    public static bool IsPlayer(GameObject player)
    {
        //return (GetPlayerLayer() & (1<<player.layer)) != 0;
        return GetPlayerLayer() == (1<<player.layer);
    }

    public static bool IsPlayer(Collider2D collider)
    {
        return IsPlayer(collider.gameObject);
    }

    public static bool IsPlayer(Collision2D collision)
    {
        return IsPlayer(collision.collider);
    }

    public static LayerMask GetEnemyLayer()
    {
        if (current == null)
            return LayerMask.NameToLayer("Enemy");
        return current.enemyLayer;
    }

    public static LayerMask GetWalkableLayer()
    {
        if (current == null)
            return LayerMask.NameToLayer("Walkable");
        return current.walkableLayer;
    }

    public static int GetPlayerLayer()
    {
        if (current == null)
            return LayerMask.NameToLayer("Player");
        return current.playerLayer.value;
    }

    public static LayerMask GetDestructibleLayer()
    {
        if (current == null)
            return 0;

        return GetEnemyLayer() + GetBuildingLayer();
    }

    public static bool CanTriggerThrowable(Collider2D collider)
    {
        if (current == null)
            return false;
        var tag = collider.tag;
        return tag == "Enemy" || tag == "Building" || tag == "Walkable" || IsPlayer(collider) || tag == "Roof" || tag == "Bridge" || tag == "EnemyBomb";
    }

    public void SetDifficultyMode(int difficulty)
    {
        SetDifficultyMode((Difficulty)difficulty);
    }

    public static void SetDifficultyMode(Difficulty difficulty)
    {
        if (current == null)
            return;
        current.difficulty = difficulty;
    }

    public static Difficulty GetDifficultyMode()
    {
        if (current == null)
            return 0;
        return current.difficulty;
    }

    public static void SetBgmAudio(float bgmAudio, bool save = false)
    {
        if (current == null)
            return;
        current.bgmAudio = bgmAudio;
        if (save)
            current.SaveSettings();
    }

    public static float GetBgmAudio()
    {
        if (current == null)
            return 0f;
        return current.bgmAudio;
    }

    public static void SetSfxAudio(float sfxAudio, bool save = false)
    {
        if (current == null)
            return;
        current.sfxAudio = sfxAudio;
        if (save)
            current.SaveSettings();
    }

    public static float GetSfxAudio()
    {
        if (current == null)
            return 0f;
        return current.sfxAudio;
    }

    public static void SetMission1Points(float points)
    {
        if (current == null)
            return;
        current.mission1Points = points;
    }

    public static float GetMission1Points()
    {
        if (current == null)
            return 0f;
        return current.mission1Points;
    }

    public static void SetMission2Points(float points)
    {
        if (current == null)
            return;
        current.mission2Points = points;
    }

    public static float GetMission2Points()
    {
        if (current == null)
            return 0f;
        return current.mission2Points;
    }

    public static void SetMission3Points(float points)
    {
        if (current == null)
            return;
        current.mission3Points = points;
    }

    public static float GetMission3Points()
    {
        if (current == null)
            return 0f;
        return current.mission3Points;
    }

    public static void GameReset()
    {
        if (!current)
            return;
        
        // reset values
        Time.timeScale = 1;
        current.isGameOver = false;
        current.score = 0;
        current.totalGameTime = 0;
        current.bombs = current.initialBombs;
        // refresh if mission directly started from editor
        UIManager.UpdateBombsUI();
        UIManager.UpdateAmmoUI();

        ReloadCurrentScene();
    }

    private static void ReloadCurrentScene()    
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;

        HeliSpawner heliSpawner = FindObjectOfType<HeliSpawner>();
        if (heliSpawner != null)
        {
            DontDestroyOnLoad(heliSpawner.gameObject);
        }
        
        SceneManager.LoadScene(currentSceneIndex);
    }


    public static void PauseExit()
    {
        if (!current)
            return;
        LoadHome();
        SaveManager.Instance.Save();
    }

    public static void LoadHome()
    {
        if(current == null)
        return;
        
        current.StartCoroutine(current.GoToHome());
    }

    IEnumerator GoToHome()
    {
        if(!SaveManager.Instance.isSetAchievement)
        {
            yield return new WaitForSeconds(0.2f);
        }

        Debug.Log("Home button pressed!");
        SaveManager.Instance.isSetAchievement = false;
        LoadScene((int)Missions.Home);
    }

    public static void LoadAfterWinMission()
    {
        // currentMission is updated in the PlayerWin method

        
        LoadScene((int)Missions.Home);
    }

    public static bool CanTriggerEnemyBombs(string tag)
    {
        //If there is no current Game Manager, exit
        if (current == null)
            return false;

        return tag == "Player" || tag == "Walkable" || tag == "Marco Boat" || tag == "Bridge";
    }

    private IEnumerator WaitNextMission()
    {
        yield return new WaitForSeconds(10f);
        LoadAfterWinMission();
    }

    public static void LoadScene(int id, bool skipReset = false)
    {
        if (!skipReset)
            GameReset();

        SceneManager.LoadScene(id);
    }

    public static void Revive()
    {
        current.isGameOver = false;
    }
}
