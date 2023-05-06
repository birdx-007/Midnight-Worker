using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public enum QTEState
{
    Inactive = 0,
    InProgress,
    Succeeded,
    Failed
}

public class QTEControl : MonoBehaviour
{
    private List<KeyCode> allowedQTEKeys;
    public QTEState state = QTEState.Inactive;
    private List<float> checkPointsTime = new List<float>();
    private List<KeyCode> checkPointsKey = new List<KeyCode>();
    private List<RectTransform> checkPointsTransform = new List<RectTransform>();
    private float _maxTimeError = 1f;
    private float maxTime = 5f;
    private float _currentTime = 0f;
    private int _currentTargetCheckPoint; // index starts with 0

    public AnimationCurve knockbackCurve;
    private float _knockbackAnimationDuration = 0.5f;
    private float _knockbackAnimationTimer = 0f;

    public bool isShowingTip = false;
    public GameObject qteCheckPointPrefab;
    private RectTransform _qteBar;
    private RectTransform _qteProgressSign;
    private GameObject _qteTip;
    private Text _qteTipInfo;
    public void StartQTE(float mTime, List<float> times, List<KeyCode> keys, float maxTimeError)
    {
        if (checkPointsTransform.Count > 0)
        {
            foreach (RectTransform trans in checkPointsTransform)
            {
                if (trans != null)
                {
                    Destroy(trans.gameObject);
                }
            }
            checkPointsTransform.Clear();
        }
        state = QTEState.InProgress;
        checkPointsTime = times;
        checkPointsKey = keys;
        _maxTimeError = maxTimeError;
        maxTime = mTime;
        _currentTime = 0f;
        _currentTargetCheckPoint = 0;
        _knockbackAnimationTimer = 0;
        for (int i = 0; i < checkPointsTime.Count; i++)
        {
            GameObject checkPoint = Instantiate(qteCheckPointPrefab, _qteBar);
            RectTransform pointTransform = checkPoint.GetComponent<RectTransform>();
            pointTransform.anchoredPosition = new Vector2(checkPointsTime[i] / maxTime * 445f, 5f);
            pointTransform.SetAsFirstSibling();
            CheckPointControl pointControl = checkPoint.GetComponent<CheckPointControl>();
            pointControl.Initiate(checkPointsKey[i].ToString());
            checkPointsTransform.Add(pointTransform);
        }
    }
    void Start()
    {
        _qteBar = GetComponent<RectTransform>();
        _qteProgressSign = _qteBar.Find("QTEProgressSign").GetComponent<RectTransform>();
        _qteProgressSign.anchoredPosition = new Vector2(0f, 0f);
        _qteTip = _qteProgressSign.Find("Tip").gameObject;
        _qteTipInfo = _qteProgressSign.Find("Tip").Find("TipInfo").GetComponent<Text>();
        allowedQTEKeys = new List<KeyCode>()
        {KeyCode.A,KeyCode.B,KeyCode.C,KeyCode.D,KeyCode.E,KeyCode.F,KeyCode.G,KeyCode.H
        ,KeyCode.I,KeyCode.J,KeyCode.K,KeyCode.L,KeyCode.M,KeyCode.N,KeyCode.O,KeyCode.P
        ,KeyCode.Q,KeyCode.R,KeyCode.S,KeyCode.T,KeyCode.U,KeyCode.V,KeyCode.W,KeyCode.X
        ,KeyCode.Y,KeyCode.Z};
        HideTip();
    }
    void Update()
    {
        if (state == QTEState.InProgress)
        {
            _currentTime = Mathf.Clamp(_currentTime + Time.deltaTime, 0f, maxTime);
            _qteProgressSign.anchoredPosition = new Vector2(_currentTime / maxTime * 445f, 0f);
            if (_currentTime > checkPointsTime[_currentTargetCheckPoint] + _maxTimeError)
            {
                HitFail();
            }
        }
        else // knockback
        {
            float t = _knockbackAnimationTimer / _knockbackAnimationDuration;
            float newX = 445f * Mathf.LerpUnclamped(_currentTime / maxTime, 0f, knockbackCurve.Evaluate(t));
            _qteProgressSign.anchoredPosition = new Vector2(newX, 0f);
            _knockbackAnimationTimer = Mathf.Clamp(_knockbackAnimationTimer + Time.deltaTime, 0f, _knockbackAnimationDuration);
        }
    }
    public bool HitCheckPoint()
    {
        if (state == QTEState.InProgress)
        {
            if (_currentTime >= checkPointsTime[_currentTargetCheckPoint] - _maxTimeError
                && _currentTime <= checkPointsTime[_currentTargetCheckPoint] + _maxTimeError
                && Input.GetKeyDown(checkPointsKey[_currentTargetCheckPoint]))
            {
                HitSucceed();
                return true;
            }
            else
            {
                HitFail();
                return false;
            }
        }
        return false;
    }
    public void HitSucceed()
    {
        Debug.Log("Hit successful!");
        //Destroy(checkPointsTransform[_currentTargetCheckPoint].gameObject);
        checkPointsTransform[_currentTargetCheckPoint].GetComponent<CheckPointControl>().GetHit();
        checkPointsTransform[_currentTargetCheckPoint] = null;
        _currentTargetCheckPoint++;
        if (_currentTargetCheckPoint == checkPointsTime.Count)
        {
            state = QTEState.Succeeded;
        }
        else
        {
            SFXPlayer.Instance.PlayQTEHitSFX();
        }
    }
    public void HitFail()
    {
        Debug.LogWarning("Hit failed!");
        foreach (RectTransform trans in checkPointsTransform)
        {
            if (trans != null)
            {
                //Destroy(trans.gameObject);
                trans.GetComponent<CheckPointControl>().GetHit();
            }
        }
        checkPointsTransform.Clear();
        state = QTEState.Failed;
    }
    public void ShowTip(string tip)
    {
        if (isShowingTip)
            return;
        isShowingTip = true;
        _qteTip.GetComponent<Animator>().SetBool("isShowing", true);
        _qteTipInfo.text = tip;
    }
    public void HideTip()
    {
        isShowingTip = false;
        _qteTip.GetComponent<Animator>().SetBool("isShowing", false);
    }
    public void GenerateQTE()
    {
        float mTime = Random.Range(3f, 5f);
        float maxTimeError = 0.2f;
        List<float> times=new List<float>();
        List<KeyCode> keys=new List<KeyCode>();
        float deltaTime = Random.Range(mTime * 0.25f, mTime * 0.4f);
        float time = deltaTime;
        while (time < mTime * 0.9f)
        {
            times.Add(time);
            keys.Add(allowedQTEKeys[Random.Range(0,allowedQTEKeys.Count)]);
            deltaTime = Random.Range(mTime * 0.2f, mTime * 0.5f);
            time += deltaTime;
        }
        StartQTE(mTime, times, keys, maxTimeError);
    }
    public bool isAllowedKeyDown()
    {
        foreach(KeyCode allowedKey in allowedQTEKeys)
        {
            if(Input.GetKeyDown(allowedKey))
            {
                return true;
            }
        }
        return false;
    }
}
