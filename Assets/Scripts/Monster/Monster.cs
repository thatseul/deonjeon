using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 능력치 (읽기용)")]
    [SerializeField] private float hp;
    [SerializeField] private float atk;
    [SerializeField] private float aspd;

    public float Atk  => atk;
    public float Aspd => Mathf.Max(0.01f, aspd);
    public bool IsAlive() => !isDead;

    private bool isDead = false;

    // 연결된 슬롯 기억 (QuickSlot.SetQuickSlotOwner로 지정)
    private QuickSlot quickSlotOwner;

    private void OnEnable()
    {
        // 씬 재진입 시 유령 상태 방지(필요하면 여기서 UI/애니 재개)
        isDead = false;
    }

    private void OnDisable()
    {
        // 전투/코루틴이 있었다면 여기서 정리
        // (현재 전투 루프는 Hero 쪽에서 관리하므로 비움)
    }

    /// <summary> 던전 능력치 * 비율로 초기화 </summary>
    public void Init(float dungeonHp, float dungeonAtk, float dungeonASPD, float ratio)
    {
        hp   = Mathf.Max(1f, dungeonHp)  * Mathf.Max(0f, ratio);
        atk  = Mathf.Max(0f, dungeonAtk) * Mathf.Max(0f, ratio);
        aspd = Mathf.Max(0.01f, dungeonASPD) * Mathf.Max(0f, ratio);

        Debug.Log($"🐲 몬스터 능력치 초기화 완료: HP={hp}, ATK={atk}, SPD={aspd}");
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;

        hp -= Mathf.Max(0f, damage);
        if (hp <= 0f) Die();
    }

    private void Die()
    {
        if (isDead) return;
        isDead = true;

        // 슬롯 UI 정리
        quickSlotOwner?.ClearSlot();

        Destroy(gameObject);
    }

    public void SetQuickSlotOwner(QuickSlot slot) => quickSlotOwner = slot;
    public QuickSlot GetQuickSlotOwner() => quickSlotOwner;
}
