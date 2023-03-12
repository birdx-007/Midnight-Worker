using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ManagerControl : MonoBehaviour
{
    public bool isPausing = false;
    public bool isQTE = false;
    public Blackbroad blackbroad;
    public BuilderControl builder;
    public PlayerControl player;
    public PoliceControl police;
    public QTEControl qte;
    public CoinCountControl coinCount;
    
    public PauseMenuControl pauseMenu;
    void Start()
    {
        builder.Initiate(blackbroad.map);
        blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        blackbroad.policeIntPosition.Set(Mathf.RoundToInt(police.transform.position.x), Mathf.RoundToInt(police.transform.position.y));
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
        // manage movement
        if (player.isOnIntPoint)
        {
            blackbroad.playerIntPosition.Set(Mathf.RoundToInt(player.transform.position.x), Mathf.RoundToInt(player.transform.position.y));
        }
        if (police.isOnIntPoint)
        {
            blackbroad.policeIntPosition = police.curIntPoint;
            var path = blackbroad.FindWayTo(blackbroad.policeIntPosition, blackbroad.playerIntPosition);
            if (path != null && path.Count > 1)
            {
                police.nextIntPoint.Set(path[1].x, path[1].y);
            }
        }
        // manage QTE
        if (player.bankVisiting != null && !isQTE)
        {
            isQTE = true;
            var times = new List<float>() { 2f, 4f };
            var keys = new List<KeyCode>() { KeyCode.Z, KeyCode.X };
            qte.Initialize(5f, times, keys, 0.2f);
            InitiateQTE();
        }
        if (player.bankVisiting == null && isQTE)
        {
            isQTE = false;
            qte.state = QTEState.Failed;
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
                player.bankVisiting = null;
            }
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
        //Time.timeScale = 1f;
    }
    public void BackToMenu()
    {

    }
    public void InitiateQTE()
    {

    }
}
