using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
    public List<PoliceControl> enemies;
    public QTEControl qte;
    public CoinCountControl coinCount;

    public PauseMenuControl pauseMenu;
    public FailedMenuControl failedMenu;
    public ClearMenuControl clearMenu;
    void Start()
    {
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
            for (int i = 0; i < Blackbroad.policeIntPositions.Count; i++)
            {
                var police = enemies[i];
                if (police.isOnIntPoint)
                {
                    Blackbroad.policeIntPositions[i] = police.curIntPoint;
                    police.behaviorControl.UpdateBehavior(ref police.nextIntPoint, police.curIntPoint);
                }
            }
            // manage game ending
            if (player.isCaught)
            {
                LoseGame();
            }
            else if (coinCount.currentCount >= levelTargetCoinCount)
            {
                WinGame();
            }
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
                        qte.ShowTip("RUN");
                        StartCoroutine(WaitThenHideTip(BankControl.maxLockedTime));
                        player.GetOutofBank();
                        qte.state = QTEState.Inactive;
                    }
                    if (!bank.isLocked && !bank.isBeingDamaged) // Start QTE
                    {
                        isQTE = true;
                        var times = new List<float>() { 2f, 4f };
                        var keys = new List<KeyCode>() { KeyCode.Z, KeyCode.X };
                        qte.StartQTE(5f, times, keys, 0.2f);
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
                            qte.ShowTip("RUN");
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
                            qte.ShowTip("RUN");
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
                qte.ShowTip("SPACE");
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    qte.HideTip();
                    player.bankFacing.Visited();
                    player.GetIntoBank(player.bankFacing);
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
    }
    public void InitiateGame(int levelIndex)
    {
        isPausing = false;
        Time.timeScale = 1f;
        _blackbroad = new Blackbroad(levelIndex);
        builder.Initiate(); // build map objects
        enemies = builder.CreateAllEnemiesfromMapData(); // create enemies
        levelTargetCoinCount = Map.targetCoinCount;
        Blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        foreach (var enemy in enemies)
        {
            Blackbroad.policeIntPositions.Add(enemy.curIntPoint);
        }
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
        levelIndex++;
        RestartGame();
    }
    public void LoseGame()
    {

        StartCoroutine(PlayerGetCaught());
    }
    IEnumerator PlayerGetCaught()
    {
        yield return new WaitForSecondsRealtime(3f);
        failedMenu.ShowFailedMenu();
        isFailed = true;
    }
    public void WinGame()
    {
        StartCoroutine(BanksAllClear());
    }
    IEnumerator BanksAllClear()
    {
        isClear = true;
        foreach(var enemy in enemies)
        {
            enemy.canCatchThief = false;
        }
        player.Win();
        yield return new WaitForSecondsRealtime(3f);
        clearMenu.ShowClearMenu();
    }
}
