using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum QTEState
{
    Inactive = 0,
    InProgress,
    Succeeded,
    Failed
}

public class QTEControl : MonoBehaviour
{
    public QTEState state = QTEState.Inactive;
    public List<float> checkPointsTime;
    public List<KeyCode> checkPointsKey;
    public List<RectTransform> checkPointsTransform;
    private float _maxTimeError;
    public float maxTime;
    private float _currentTime;
    private int _currentTargetCheckPoint; // index starts with 0

    public AnimationCurve knockbackCurve;
    private float _knockbackAnimationDuration=0.5f;
    private float _knockbackAnimationTimer=0f;

    public RectTransform qteBar;
    public RectTransform qteProgressSign;
    public GameObject qteCheckPoint;
    public void Initialize(float mTime, List<float> times, List<KeyCode> keys, float maxTimeError)
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
        for(int i = 0; i < checkPointsTime.Count; i++)
        {
            GameObject checkPoint = Instantiate(qteCheckPoint, qteBar);
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
        qteProgressSign.anchoredPosition = new Vector2(0f, 0f);
    }
    void Update()
    {
        if (state == QTEState.InProgress)
        {
            _currentTime = Mathf.Clamp(_currentTime + Time.deltaTime, 0f, maxTime);
            qteProgressSign.anchoredPosition = new Vector2(_currentTime / maxTime * 445f, 0f);
            if (_currentTime > checkPointsTime[_currentTargetCheckPoint] + _maxTimeError)
            {
                HitFail();
            }
        }
        else if (state == QTEState.Failed || state == QTEState.Succeeded)
        {
            float t = _knockbackAnimationTimer / _knockbackAnimationDuration;
            float newX = 445f * Mathf.LerpUnclamped(_currentTime / maxTime, 0f, knockbackCurve.Evaluate(t));
            qteProgressSign.anchoredPosition = new Vector2(newX, 0f);
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
        Destroy(checkPointsTransform[_currentTargetCheckPoint].gameObject);
        checkPointsTransform[_currentTargetCheckPoint] = null;
        _currentTargetCheckPoint++;
        if (_currentTargetCheckPoint == checkPointsTime.Count)
        {
            state = QTEState.Succeeded;
        }
    }
    public void HitFail()
    {
        Debug.LogWarning("Hit failed!");
        foreach (RectTransform trans in checkPointsTransform)
        {
            if (trans != null)
            {
                Destroy(trans.gameObject);
            }
        }
        checkPointsTransform.Clear();
        state = QTEState.Failed;
    }
}
