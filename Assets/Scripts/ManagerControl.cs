using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class ManagerControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;
    public int levelIndex = 1;
    private int levelTargetCoinCount;
    public bool isFailed = false;
    public bool isClear = false;
    public bool isPausing = false;
    public bool isQTE = false;
    public bool isAnyBankBeingLocked = false;
    private Blackbroad _blackbroad;
    public BuilderControl builder;
    public PlayerControl player;
    public List<EnemyControl> enemies;
    public QTEControl qte;
    public CoinCountControl coinCount;

    public PauseMenuControl pauseMenu;
    public FailedMenuControl failedMenu;
    public ClearMenuControl clearMenu;
    void Start()
    {
        if (GlobalTerminal.Instance != null)
        {
            levelIndex = GlobalTerminal.Instance.Global_LevelIndex;
        }
        InitiateGame(levelIndex);
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPausing)
        {
            PauseGame();
        }
        if (Input.GetKeyDown(KeyCode.Space) && isPausing)
        {
            ContinueGame();
        }
        if (Input.GetKeyDown(KeyCode.Space) && isFailed)
        {
            RestartGame();
        }
        if(Input.GetKeyDown(KeyCode.Space) && isClear)
        {
            GoToNextLevel();
        }
        if (!isPausing && !builder.isEditing)
        {
            // manage player movement
            if (player.isOnIntPoint)
            {
                Blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
            }
            // manage enemies
            for (int i = 0; i < Blackbroad.enemyIntPositions.Count; i++)
            {
                var enemy = enemies[i];
                //Debug.DrawLine((Vector3Int)Blackbroad.enemyIntPositions[i], (Vector3Int)police.curIntPoint);
                if (enemy.isOnIntPoint)
                {
                    Blackbroad.enemyIntPositions[i] = enemy.curIntPoint;
                    enemy.UpdateOnIntPoint();
                }
            }
            // manage game ending
            if (player.isCaught && !isFailed)
            {
                LoseGame();
            }
            else if (coinCount.currentCount >= levelTargetCoinCount && !isClear)
            {
                WinGame();
            }
            GameLogicUpdate();
        }
    }
    public void GameLogicUpdate()
    {
        // manage QTE
        if (player.bankVisiting != null)
        {
            IEnumerator WaitThenHideTip(float waitTime)
            {
                yield return new WaitForSeconds(waitTime);
                qte.HideTip();
                isAnyBankBeingLocked = false;
            };
            var bank = player.bankVisiting;
            if (qte.state != QTEState.InProgress)
            {
                if (qte.state == QTEState.Failed)
                {
                    bank.Lock();
                    isAnyBankBeingLocked = true;
                    qte.ShowTip("FAILED!");
                    SFXPlayer.Instance.PlayQTEFailSFX();
                    StartCoroutine(WaitThenHideTip(BankControl.maxLockedTime));
                    player.GetOutofBank();
                    qte.state = QTEState.Inactive;
                }
                if (!bank.isLocked && !bank.isBeingDamaged) // Start QTE
                {
                    isQTE = true;
                    qte.GenerateQTE();
                }
            }
            else // QTE in progress
            {
                if (Input.anyKeyDown && !builder.isEditing && !player.isInputingMotion
                && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Space))
                {
                    qte.HitCheckPoint();// QTE may succeed or fail
                    if (qte.state == QTEState.Succeeded)
                    {
                        isQTE = false;
                        bank.Damage();
                        isAnyBankBeingLocked = true;
                        coinCount.AddOne();
                        qte.ShowTip("GREAT!");
                        SFXPlayer.Instance.PlayQTESucceedSFX();
                        StartCoroutine(WaitThenHideTip(BankControl.maxLockedTime));
                        player.UnfaceBank();
                        player.GetOutofBank();
                        qte.state = QTEState.Inactive;
                    }
                    else if (qte.state == QTEState.Failed)
                    {
                        isQTE = false;
                        bank.Lock();
                        isAnyBankBeingLocked = true;
                        qte.ShowTip("FAILED!");
                        SFXPlayer.Instance.PlayQTEFailSFX();
                        StartCoroutine(WaitThenHideTip(BankControl.maxLockedTime));
                        player.UnfaceBank();
                        player.GetOutofBank();
                        qte.state = QTEState.Inactive;
                    }
                }
            }
        }
        else if (player.bankFacing != null) // face but not visit
        {
            var bank = player.bankFacing;
            qte.ShowTip("SPACE");
            if (Input.GetKeyDown(KeyCode.Space))
            {
                qte.HideTip();
                bank.Visited();
                player.GetIntoBank(bank);
            }
        }
        else // player leaves bank
        {
            isQTE = false;
            if (qte.state == QTEState.InProgress)
            {
                qte.HitFail();
            }
            if (!isAnyBankBeingLocked)
            {
                qte.HideTip();
            }
        }
    }
    public void ApplyWeatherInfluences()
    {
        float speedFactor = 1f;
        switch (Blackbroad.map.weather)
        {
            case WeatherState.Sunny:
                speedFactor = WeatherFactor.SUNNY_SPEED_FACTOR;
                break;
            case WeatherState.Windy:
                speedFactor = WeatherFactor.WINDY_SPEED_FACTOR;
                break;
            case WeatherState.Stormy:
                speedFactor = WeatherFactor.STORMY_SPEED_FACTOR;
                break;
            default:
                break;
        }
        player.speed = player.standardSpeed * speedFactor;
        foreach (var enemy in enemies)
        {
            enemy.speed = enemy.standardSpeed * speedFactor;
            enemy.UpdateMoveable();
        }
    }
    public void InitiateGame(int levelIndex)
    {
        isPausing = false;
        Time.timeScale = 1f;
        _blackbroad = new Blackbroad(levelIndex);
        builder.Initiate(); // build map objects
        enemies = builder.CreateAllEnemiesfromMapData(); // create enemies
        levelTargetCoinCount = Blackbroad.map.GetTargetCoinCount();
        Blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        foreach (var enemy in enemies)
        {
            Blackbroad.enemyIntPositions.Add(enemy.curIntPoint);
        }
        ApplyWeatherInfluences();
    }
    public void PauseGame()
    {
        isPausing = true;
        pauseMenu.ShowPauseMenu();
        Time.timeScale = 0f;
    }
    public void ContinueGame()
    {
        isPausing = false;
        pauseMenu.HidePauseMenu();
        //Time.timeScale = 1f; ��һ������ͣ�˵���hide������β��eventִ��
    }
    public void BackToMenu()
    {
        sceneLoader.LoadSceneWithName("MainMenu");
    }
    public void RestartGame()
    {
        sceneLoader.LoadSceneWithIndex(SceneManager.GetActiveScene().buildIndex);
    }
    public void GoToNextLevel()
    {
        if (GlobalTerminal.Instance != null)
        {
            if(GlobalTerminal.Instance.Global_LevelIndex == GlobalTerminal.Instance.Global_MaxLevelIndex)
            {
                sceneLoader.LoadSceneWithName("CongratulationsMenu");
                return;
            }
            GlobalTerminal.Instance.Global_LevelIndex = levelIndex + 1;
        }
        RestartGame();
    }
    public void LoseGame()
    {
        StartCoroutine(PlayerGetCaught());
    }
    IEnumerator PlayerGetCaught()
    {
        isFailed = true;
        SFXPlayer.Instance.PlayPlayerCaughtSFX();
        yield return new WaitForSecondsRealtime(3f);
        failedMenu.ShowFailedMenu();
    }
    public void WinGame()
    {
        StartCoroutine(BanksAllClear());
    }
    IEnumerator BanksAllClear()
    {
        isClear = true;
        GlobalTerminal.Instance.UpdateUnlockedLevelIndex_OnWin();
        SaveSystem.Instance.Save();
        foreach (var enemy in enemies)
        {
            enemy.canCatchThief = false;
        }
        player.Win();
        yield return new WaitForSecondsRealtime(3f);
        clearMenu.ShowClearMenu();
    }
}
