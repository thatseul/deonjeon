using System.Collections.Generic;
using UnityEngine;
using System.Collections;


/// <summary>
/// 전투가 끝날 때마다 점점 더 강한 용사를 생성하는 스폰 시스템.
/// 특정 단계마다 용사 수가 증가하며, 이후 강한 보스가 등장하는 구조.
/// </summary>
public class HeroSpawner : MonoBehaviour
{
    [Header("Hero 설정")]
    [Tooltip("생성할 용사 프리팹")]
    [SerializeField] private GameObject heroPrefab;

    [Tooltip("용사 생성 위치")]
    [SerializeField] private Vector3 spawnPosition = new Vector3(4f, 3.5f, 0);

    [Header("웨이브 설정")]
    [Tooltip("한 웨이브에 최대 등장하는 용사 수")]
    [SerializeField] private int maxConcurrentHeroes = 5;

    [Tooltip("보스가 등장할 때 추가되는 HP")]
    [SerializeField] private int bossHpBoost = 5;

    private bool isResetting = false;
    private int currentHeroMaxHp = 1;
    private int currentHeroCount = 1;
    private List<GameObject> aliveHeroes = new List<GameObject>();


    private IEnumerator Start()
{
    // 던전이 완전히 생성되고 Start()까지 실행
    yield return new WaitUntil(() => FindFirstObjectByType<Dungeon>() != null);

    // 추가로 한 프레임만 더
    yield return null;

    SpawnCurrentWave();
}

    /// <summary>
    /// 현재 웨이브에 맞는 용사들을 생성합니다.
    /// </summary>
    private void SpawnCurrentWave()
    {
        Debug.Log($"🛡 Hero 웨이브 생성! 수: {currentHeroCount}, Max HP: {currentHeroMaxHp}");
        aliveHeroes.Clear();

        float xGap = 0.8f;     // 좌우 간격
        float yGap = 0.6f;     // 상하 간격
        int columns = 2;       // 한 줄에 배치할 용사 수

        for (int i = 0; i < currentHeroCount; i++)
        {
            int col = i % columns;           // 0, 1, 0, 1 ...
            int row = i / columns;           // 0, 0, 1, 1 ...

            Vector3 offset = new Vector3(col * xGap, -row * yGap, 0f);
            Vector3 finalPosition = spawnPosition + offset;

            GameObject hero = Instantiate(heroPrefab, finalPosition, Quaternion.identity);
            Hero heroScript = hero.GetComponent<Hero>();

            if (heroScript != null)
            {
                int heroHp = CalculateHpForHero(i);
                heroScript.Initialize(heroHp, this);
            }

            aliveHeroes.Add(hero);
        }
    }

    /// <summary>
    /// 각 용사의 HP를 계산합니다.
    /// 보스 구조를 위한 분산 HP 계산.
    /// </summary>
    private int CalculateHpForHero(int index)
    {
        // 예: 보스 등장 시 단일 강한 용사
        if (currentHeroCount == 1 && currentHeroMaxHp > maxConcurrentHeroes)
        {
            return currentHeroMaxHp + bossHpBoost;
        }

        // 일반 분산 구조
        return Mathf.Max(1, currentHeroMaxHp - index);
    }

    /// <summary>
    /// Hero.cs에서 사망 시 호출됨
    /// </summary>
    public void OnHeroDeath(GameObject hero)
    {
        Debug.Log("📢 OnHeroDeath 호출됨");

        if (isResetting) return;

        if (aliveHeroes.Contains(hero))
        {
            aliveHeroes.Remove(hero);
        }

        if (aliveHeroes.Count == 0)
        {
            UpdateWaveLogic();
            SpawnCurrentWave();
        }
    }

    /// <summary>
    /// 웨이브 수에 따라 용사 수/강함을 조절합니다.
    /// </summary>
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

    /// <summary>
    /// 던전이 초기화될 때 호출됨
    /// </summary>
    public void ResetSpawner()
    {
        Debug.Log("🌀 HeroSpawner 초기화 시작!");

        isResetting = true;

        foreach (var hero in aliveHeroes)
        {
            if (hero != null)
                Destroy(hero);
        }

        StartCoroutine(ClearAndRespawn()); // ✅ 코루틴으로 넘김
    }

    private IEnumerator ClearAndRespawn()
    {
        yield return null; // 한 프레임 기다림

        aliveHeroes.Clear();
        SpawnCurrentWave();

        isResetting = false;
    }
}
