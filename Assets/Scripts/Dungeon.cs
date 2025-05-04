using UnityEngine;

/// <summary>
/// 플레이어가 조작하는 던전 오브젝트.  
/// 체력이 0이 되면 상태를 초기화하고, 반복적인 게임 루프를 유지함.
/// </summary>
public class Dungeon : MonoBehaviour
{
    [Header("던전 체력 설정")]
    [Tooltip("던전의 최대 체력입니다. 반복 루프 시 항상 이 수치로 복원됩니다.")]
    [SerializeField] private int maxHp = 10;

    private int currentHp;

    private void Start()
    {
        currentHp = maxHp;
    }

    /// <summary>
    /// 던전이 용사에게 공격받을 때 호출됩니다.
    /// </summary>
    /// <param name="damage">받는 피해량</param>
    public void TakeDamage(int damage)
    {
        currentHp -= damage;
        Debug.Log($"Dungeon HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            DieAndRespawn();
        }
    }

    /// <summary>
    /// 던전 체력이 0이 되면, 1초 후에 던전과 관련된 상태를 리셋합니다.
    /// </summary>
    private void DieAndRespawn()
    {
        Debug.Log("💥 Dungeon HP 0! Resetting dungeon state...");
        Invoke(nameof(ResetDungeonState), 1f);
    }

    /// <summary>
    /// 던전 체력을 복원하고, 모든 용사와 전투 상황을 초기화합니다.
    /// </summary>
    private void ResetDungeonState()
    {
        currentHp = maxHp;
        Debug.Log($"Dungeon HP restored to {currentHp}/{maxHp}");

        // TODO: 몬스터, 트랩 등 던전 내부 상태 초기화 로직 추가 예정

        // 화면에 있는 모든 Hero 제거
        foreach (var hero in GameObject.FindGameObjectsWithTag("Hero"))
        {
            Destroy(hero);
        }

        // Hero 스폰 시스템 초기화
        HeroSpawner spawner = FindObjectOfType<HeroSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawner(); // TODO: 이 함수는 AutoHeroSpawner에서 구현 필요
        }

        // TODO: Unity 6 업그레이드 시, 리셋 시점에 화면 페이드 연출 추가 예정
    }
}
