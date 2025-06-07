using UnityEngine;

/// <summary>
/// 게임 화면에 배치되는 실제 몬스터 오브젝트
/// 능력치는 소환 시 Init()으로 설정됨
/// </summary>
public class Monster : MonoBehaviour
{
    [Header("몬스터 능력치 (읽기용)")]
    [SerializeField] private float hp;
    [SerializeField] private float atk;
    [SerializeField] private float aspd;

    public float Atk => atk;
    public float Aspd => aspd;
    public bool IsAlive() => !isDead;

    private bool isDead = false;
    private QuickSlot quickSlotOwner; // ✅ 연결된 슬롯 기억

    /// <summary>
    /// 몬스터 능력치 초기화 (던전 능력치 * 비율)
    /// </summary>
    public void Init(float dungeonHp, float dungeonAtk, float dungeonASPD, float ratio)
    {
        hp = dungeonHp * ratio;
        atk = dungeonAtk * ratio;
        aspd = dungeonASPD * ratio;

        Debug.Log($"🐲 몬스터 능력치 초기화 완료: HP={hp}, ATK={atk}, SPD={aspd}");
    }

    public void TakeDamage(float damage)
    {
        hp -= damage;
        Debug.Log($"🩸 [{gameObject.name}] 데미지 {damage} ▶ 현재 HP: {hp}");

        if (hp <= 0)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;

        // ✅ 퀵슬롯에 알림
        quickSlotOwner?.ClearSlot();

        Destroy(gameObject);
    }

    public void SetQuickSlotOwner(QuickSlot slot)
    {
        quickSlotOwner = slot;
    }
}
