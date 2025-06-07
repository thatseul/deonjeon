using UnityEngine;
using System.Collections.Generic;

public class MonsterPoolManager : MonoBehaviour
{
    [Header("뽑을 몬스터 리스트 (ScriptableObject)")]
    [SerializeField] private List<MonsterItem> monsterList;

    /// <summary>
    /// 확률 기반으로 몬스터 프리팹 반환
    /// </summary>
    public GameObject GetRandomMonsterPrefab()
    {
        float totalProbability = 0f;
        foreach (var monster in monsterList)
            totalProbability += monster.probability;

        float rand = Random.Range(0f, totalProbability);
        float cumulative = 0f;

        foreach (var monster in monsterList)
        {
            cumulative += monster.probability;
            if (rand <= cumulative)
            {
                Debug.Log($"🎯 뽑힌 몬스터: {monster.monsterName}");
                return monster.monsterPrefab;
            }
        }

        Debug.LogWarning("⚠️ 뽑기 실패! 리스트 또는 확률 확인 필요");
        return null;
    }

    /// <summary>
    /// 뽑힌 몬스터의 MonsterItem 정보 반환
    /// </summary>
    public MonsterItem GetRandomMonsterItem()
    {
        float totalProbability = 0f;
        foreach (var monster in monsterList)
            totalProbability += monster.probability;

        float rand = Random.Range(0f, totalProbability);
        float cumulative = 0f;

        foreach (var monster in monsterList)
        {
            cumulative += monster.probability;
            if (rand <= cumulative)
            {
                return monster;
            }
        }

        return null;
    }
}
