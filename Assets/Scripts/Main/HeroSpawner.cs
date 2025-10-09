using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class HeroSpawner : MonoBehaviour
{
    [Header("Hero 설정")]
    [SerializeField] private GameObject heroPrefab;
    [SerializeField] private Vector3 spawnPosition = new Vector3(4f, 3.5f, 0);

    [Header("웨이브 설정")]
    [SerializeField] private int maxConcurrentHeroes = 5;
    [SerializeField] private int bossHpBoost = 5;

    [Header("동작 옵션")]
    [Tooltip("활성화될 때 자동으로 웨이브 시작")]
    [SerializeField] private bool autoStartOnEnable = true;
    [Header("Economy Estimate (추정값)")]
    [SerializeField] private int goldPerKill = 10;                 // 히어로 1명 처치 보상(= Hero.goldReward와 맞추기)
    [SerializeField, Min(0.1f)] private float estimatedWaveClearTimeSec = 6f; // 한 웨이브 평균 소요 시간(초)
    [SerializeField] private int maxTargetsSimultaneously = 1;     // 던전/몬스터가 동시에 타격 가능한 타깃 수(보통 1)

    public int GoldPerKill => goldPerKill;
    public int MaxConcurrentHeroes => maxConcurrentHeroes; // 기존 private 필드 공개용 Getter
    public int CurrentHeroCount => currentHeroCount;       // 현재 웨이브 동시 등장 수
    public int CurrentHeroMaxHp => currentHeroMaxHp;       // 현재 웨이브 기준 상한 HP(분산 계산에 사용)

    /// <summary> 한 웨이브를 평균 몇 초 안에 비우는지(추정). 밸런싱으로 조절. </summary>
    public float EstimatedWaveClearTimeSec => Mathf.Max(0.1f, estimatedWaveClearTimeSec);

    private bool isResetting = false;
    private bool isRunning = false;          // ✅ 중복 실행 방지
    private int currentHeroMaxHp = 1;
    private int currentHeroCount = 1;
    private readonly List<GameObject> aliveHeroes = new List<GameObject>();

    // ✅ Start 대신 OnEnable에서 시작
    private void OnEnable()
    {
        // 씬 로드시/오브젝트 재활성화 시 항상 재가동 보장
        if (autoStartOnEnable)
        {
            BeginWaveIfNeeded();
        }

        // 씬 로드시 한 번 더 안전 장치
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        // 코루틴/참조 정리
        StopAllCoroutines();
        isRunning = false;

        // 필요 시: 씬 떠날 때 잔존 히어로 정리(선택)
        // ClearAllHeroes();

        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene sc, LoadSceneMode mode)
    {
        // 메인 씬에 들어올 때 스폰 보장
        if (sc.name == "deonjeon2020" && gameObject.activeInHierarchy)
        {
            // 씬 재진입 시 상태 초기화가 필요하면 ResetSpawner 호출
            // ResetSpawner();  // ← 완전 초기화가 필요할 때 사용
            BeginWaveIfNeeded(); // ← 현 웨이브 지속이 괜찮으면 이걸로 충분
        }
    }

    private void BeginWaveIfNeeded()
    {
        if (isRunning) return;
        isRunning = true;

        // 활성화 직후 프레임 정리 후 스폰 시작
        StartCoroutine(CoStartWave());
    }

    private IEnumerator CoStartWave()
    {
        yield return null; // 한 프레임 대기(씬 활성화/카메라/UI 안정화 대기)
        // aliveHeroes가 비어 있으면 새 웨이브 생성
        if (aliveHeroes.Count == 0)
        {
            SpawnCurrentWave();
        }
    }

    // ===== 기존 로직 유지 =====
    private void SpawnCurrentWave()
    {
        Debug.Log($"🛡 Hero 웨이브 생성! 수: {currentHeroCount}, Max HP: {currentHeroMaxHp}");
        aliveHeroes.Clear();

        float xGap = 0.8f;
        float yGap = 0.6f;
        int columns = 2;

        for (int i = 0; i < currentHeroCount; i++)
        {
            int col = i % columns;
            int row = i / columns;

            Vector3 offset = new Vector3(col * xGap, -row * yGap, 0f);
            Vector3 finalPosition = spawnPosition + offset;

            // ✅ 스폰된 히어로를 이 스포너의 자식으로 두면,
            //    스포너 비활성화시 함께 비활성화되어 관리가 쉬움
            GameObject hero = Instantiate(heroPrefab, finalPosition, Quaternion.identity, this.transform);

            var heroScript = hero.GetComponent<Hero>();
            if (heroScript != null)
            {
                int heroHp = CalculateHpForHero(i);
                heroScript.Initialize(heroHp, this);
            }

            aliveHeroes.Add(hero);
        }
    }

    private int CalculateHpForHero(int index)
    {
        if (currentHeroCount == 1 && currentHeroMaxHp > maxConcurrentHeroes)
        {
            return currentHeroMaxHp + bossHpBoost;
        }
        return Mathf.Max(1, currentHeroMaxHp - index);
    }
  /// <summary> 평균 HP(현재 웨이브 기준) — 분산 HP를 평균낸 값 </summary>
    public float GetAverageHeroHpEstimate()
    {
        if (currentHeroCount <= 0) return 1f;
        float sum = 0f;
        for (int i = 0; i < currentHeroCount; i++)
            sum += CalculateHpForHero(i);
        return sum / currentHeroCount;
    }

    /// <summary> 웨이브 전체 처치 수 / 클리어 시간 = 초당 처치 수 추정 </summary>
    public float GetKillsPerSecondWaveBased()
    {
        return CurrentHeroCount / Mathf.Max(0.1f, EstimatedWaveClearTimeSec);
    }

    /// <summary> 동시 타겟 제한(던전/스킬 특성)에 따른 상한선 </summary>
    public int GetMaxTargetsSimultaneously() => Mathf.Max(1, maxTargetsSimultaneously);
    
    public void OnHeroDeath(GameObject hero)
    {
        Debug.Log("📢 OnHeroDeath 호출됨");
        if (isResetting) return;

        if (aliveHeroes.Contains(hero))
            aliveHeroes.Remove(hero);

        if (aliveHeroes.Count == 0)
        {
            UpdateWaveLogic();
            SpawnCurrentWave();
        }
    }

    private void UpdateWaveLogic()
    {
        if (currentHeroCount < maxConcurrentHeroes)
        {
            currentHeroCount++;
            currentHeroMaxHp++;
        }
        else
        {
            currentHeroCount = 1;
            currentHeroMaxHp += bossHpBoost;
        }
    }

    public void ResetSpawner()
    {
        Debug.Log("🌀 HeroSpawner 초기화 시작!");
        isResetting = true;

        ClearAllHeroes();

        StartCoroutine(ClearAndRespawn());
    }

    private void ClearAllHeroes()
    {
        // 자식으로 붙여놨다면 루프도 간단
        foreach (var hero in aliveHeroes)
        {
            if (hero != null)
                Destroy(hero);
        }
        aliveHeroes.Clear();
    }

    private IEnumerator ClearAndRespawn()
    {
        yield return null; // 한 프레임 대기
        isResetting = false;
        isRunning = false; // 재가동 허용
        BeginWaveIfNeeded();
    }
}
