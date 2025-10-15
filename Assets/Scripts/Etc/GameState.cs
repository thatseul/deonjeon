using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameState : MonoBehaviour
{
    public static GameState I { get; private set; }

    [Header("통합 재화")]
    public double Gold;                          // 두 씬이 공유해서 쓰는 골드

    [Header("보스 씬에 있는 동안 적용할 메인 수익/초")]
    public double PassiveIncomePerSecond;        // 메인에서 계산해서 넘겨줌

    DateTime _lastTickUtc;
    bool _isInMain;

    [Header("몬스터 인벤 수용량")]
    public int monsterCapacityUnlocked = 1;      // 초기 1
    public const int monsterCapacityMax = 8;     // 상한 8

    void Awake()
    {
        if (I != null) { Destroy(gameObject); return; }
        I = this;
        DontDestroyOnLoad(gameObject);
        _lastTickUtc = DateTime.UtcNow;

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Update()
    {
        // 메인 씬이 아닐 때만 "가상 수익" 누적 (보스 씬 등)
        if (!_isInMain && PassiveIncomePerSecond > 0)
        {
            var now = DateTime.UtcNow;
            var dt = (now - _lastTickUtc).TotalSeconds;
            if (dt > 0)
            {
                Gold += PassiveIncomePerSecond * dt;
                _lastTickUtc = now;
            }
        }
        else
        {
            _lastTickUtc = DateTime.UtcNow;
        }
    }

    void OnSceneLoaded(Scene sc, LoadSceneMode mode)
    {
        _isInMain = sc.name == "deonjeon2020";
        _lastTickUtc = DateTime.UtcNow;
    }

    // 메인/보스 공용 API
    public void AddGold(double amount) => Gold += amount;
    public bool TrySpend(double amount)
    {
        if (Gold < amount) return false;
        Gold -= amount; return true;
    }
}
