using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class ManagerControl : MonoBehaviour
{
    public SceneLoaderControl sceneLoader;
    public bool isFailed = false;
    public bool isPausing = false;
    public bool isQTE = false;
    public bool isAnyBankBeingLocked = false;
    private Blackbroad _blackbroad;
    public BuilderControl builder;
    public PlayerControl player;
    public PoliceControl police;
    public QTEControl qte;
    public CoinCountControl coinCount;
    
    public PauseMenuControl pauseMenu;
    public FailedMenuControl failedMenu;
    void Start()
    {
        InitiateGame();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && !isPausing)
        {
            PauseGame();
        }
        if(Input.GetKeyDown(KeyCode.Space) && isPausing)
        {
            ContinueGame();
        }
        if (Input.GetKeyDown(KeyCode.Space) && isFailed)
        {
            RestartGame();
        }
        if (!isPausing)
        {
            // manage movement
            if (player.isOnIntPoint)
            {
                _blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
            }
            if (police.isOnIntPoint)
            {
                _blackbroad.policeIntPosition = police.curIntPoint;
                var path = _blackbroad.FindWayTo(_blackbroad.policeIntPosition, _blackbroad.playerIntPosition);
                if (path != null && path.Count > 1)
                {
                    police.nextIntPoint.Set(path[1].x, path[1].y);
                }
            }
            // manage catch
            if(player.isCaught)
            {
                LoseGame();
            }
            // manage QTE
            if(player.bankVisiting != null)
            {
                IEnumerator WaitAndHideTip(float waitTime)
                {
                    yield return new WaitForSeconds(waitTime);
                    qte.HideTip();
                    isAnyBankBeingLocked = false;
                };
                var bank = player.bankVisiting;
                if (qte.state != QTEState.InProgress)
                {
                    if(qte.state == QTEState.Failed)
                    {
                        bank.Lock();
                        isAnyBankBeingLocked = true;
                        qte.ShowTip("RUN");
                        StartCoroutine(WaitAndHideTip(BankControl.maxLockedTime));
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
                            StartCoroutine(WaitAndHideTip(BankControl.maxLockedTime));
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
                            StartCoroutine(WaitAndHideTip(BankControl.maxLockedTime));
                            player.UnfaceBank();
                            player.GetOutofBank();
                            qte.state = QTEState.Inactive;
                        }
                    }
                }
            }
            else if(player.bankFacing != null) // face but not visit
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
                if(!isAnyBankBeingLocked)
                {
                    qte.HideTip();
                }
            }
            /*
            if (player.bankVisiting != null && !isQTE)
            {
                isQTE = true;
                var times = new List<float>() { 2f, 4f };
                var keys = new List<KeyCode>() { KeyCode.Z, KeyCode.X };
                qte.Initialize(5f, times, keys, 0.2f);
            }
            if (player.bankVisiting == null && isQTE)
            {
                isQTE = false;
                qte.HitFail();
            }
            if (isQTE && !isPausing)
            {
                if (qte.state == QTEState.InProgress && Input.anyKeyDown && !builder.isEditing
                    && !Input.GetKeyDown(KeyCode.Escape) && !Input.GetKeyDown(KeyCode.Space))
                {
                    qte.HitCheckPoint();
                }
                if (qte.state == QTEState.Succeeded && player.bankVisiting != null)
                {
                    player.bankVisiting.Damage();
                    coinCount.AddOne();
                    player.GetOutofBank();
                }
            }//*/
        }
    }
    public void InitiateGame()
    {
        isPausing = false;
        Time.timeScale = 1f;
        _blackbroad = new Blackbroad();
        builder.Initiate(_blackbroad.map);
        _blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        _blackbroad.policeIntPosition.Set(Mathf.RoundToInt(police.transform.position.x), Mathf.RoundToInt(police.transform.position.y));
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
}
