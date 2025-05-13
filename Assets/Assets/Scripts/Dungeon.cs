using UnityEngine;

/// <summary>
/// 플레이어가 조작하는 던전 본체. 체력과 공격력을 DungeonStatsManager에서 가져와 사용합니다.
/// </summary>
public class Dungeon : MonoBehaviour
{
    [Header("스탯 관리자")]
    [Tooltip("Dungeon의 능력치를 관리하는 스크립트입니다.")]
    [SerializeField] private DungeonStatsManager statsManager;

    public float AttackSpeed => statsManager.GetStatValue("ASPD");

    private float currentHp;
    private bool isDead = false;

    private void Start()
    {
        currentHp = MaxHp;
    }

    public float MaxHp => statsManager.GetStatValue("HP");
    public float AttackPower => statsManager.GetStatValue("ATK");

    /// <summary>
    /// 용사에게 피해를 받았을 때 호출됩니다.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead)
        {
            Debug.Log("🛑 데미지 무시됨: 던전 이미 사망 상태");
            return;
        }

        currentHp -= damage;
        Debug.Log($"💢 데미지 {damage} 입음! 현재 HP: {currentHp}/{MaxHp}");

        if (currentHp <= 0)
        {
            isDead = true;
            currentHp = 0;
            Debug.Log("☠️ Dungeon 사망! 초기화 예정");
            Invoke(nameof(ResetDungeonState), 1f);
        }
    }

    /// <summary>
    /// 던전 체력을 복원하고 게임 상태를 초기화합니다.
    /// </summary>
    private void ResetDungeonState()
    {
        currentHp = MaxHp;
        isDead = false;
        Debug.Log($"✅ Dungeon 리셋 완료! currentHp={currentHp}, isDead={isDead}");

        // 모든 히어로 제거
        foreach (var hero in GameObject.FindGameObjectsWithTag("Hero"))
        {
            Destroy(hero);
        }

        // 히어로 스폰 재시작
        HeroSpawner spawner = FindFirstObjectByType<HeroSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawner();
        }

        // TODO: 리셋 시 페이드 연출 추가 예정
    }

    // 공격력을 직접 증가시키는 방식은 제거됨
    // 모든 스탯은 DungeonStatsManager에서 관리
}
