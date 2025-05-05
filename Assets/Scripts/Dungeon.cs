// Dungeon.cs
using UnityEngine;

/// <summary>
/// 플레이어가 조작하는 던전 오브젝트. 
/// 체력이 0이 되면 상태를 초기화하고, 반복적인 게임 루프를 유지함.
/// 앞으로 공격력 시스템 추가 예정.
/// </summary>
public class Dungeon : MonoBehaviour
{
    [Header("던전 체력 설정")]
    [Tooltip("던전의 최대 체력입니다. 반복 루프 시 항상 이 수치로 복원됩니다.")]
    [SerializeField] private int maxHp = 30;

    [Header("던전 공격력 설정")]
    [Tooltip("던전이 초당 입히는 데미지. 몬스터 대신 용사의 체력을 깎는 데 사용됩니다.")]
    [SerializeField] private int attackPowerPerSecond = 1;

    public int AttackPower => attackPowerPerSecond;

    /// <summary>
/// Dungeon의 공격력을 증가시킵니다. (예: 업그레이드 시 호출)
/// </summary>
    public void IncreaseAttackPower(int amount)
    {
        attackPowerPerSecond += amount;
        Debug.Log($"⚔️ Dungeon 공격력 증가! 현재 공격력: {attackPowerPerSecond}");
    }


    private int currentHp;
    private bool isDead = false;

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
        if (isDead)
        {
            Debug.Log("🛑 데미지 무시됨: 던전 이미 죽음");
            return;
        }

        currentHp -= damage;
        Debug.Log($"💢 Dungeon 데미지 받음! 남은 HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            isDead = true;
            currentHp = 0;
            Debug.Log("☠️ Dungeon 사망! 초기화 예정");
            Invoke(nameof(ResetDungeonState), 1f);
        }
    }

    /// <summary>
    /// 던전 체력을 복원하고, 모든 용사와 전투 상황을 초기화합니다.
    /// </summary>
    private void ResetDungeonState()
    {
        currentHp = maxHp;
        isDead = false;
        Debug.Log($"✅ Dungeon 리셋됨! currentHp={currentHp}, isDead={isDead}");

        // 화면에 있는 모든 Hero 제거
        foreach (var hero in GameObject.FindGameObjectsWithTag("Hero"))
        {
            Destroy(hero);
        }

        // Hero 스폰 시스템 초기화
        HeroSpawner spawner = FindObjectOfType<HeroSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawner(); // TODO: 이 함수는 HeroSpawner에서 구현되어 있어야 함
        }

        // TODO: Unity 6 업그레이드 시, 리셋 시점에 화면 페이드 연출 추가 예정
    }
}