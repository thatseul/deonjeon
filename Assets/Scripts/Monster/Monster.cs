using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("몬스터 능력치 (읽기용)")]
    [SerializeField] private float hp;
    [SerializeField] private float atk;
    [SerializeField] private float aspd;

    public float Atk => atk;
    public float Aspd => Mathf.Max(0.01f, aspd);
    public bool IsAlive() => !isDead;

    private bool isDead = false;

    // QuickSlot에서 할당하는 슬롯 주인
    private QuickSlot quickSlotOwner;

    private void OnEnable()
    {
        isDead = false;
    }

    public void Init(float dungeonHp, float dungeonAtk, float dungeonASPD, float ratio)
    {
        hp = Mathf.Max(1f, dungeonHp) * Mathf.Max(0f, ratio);
        atk = Mathf.Max(0f, dungeonAtk) * Mathf.Max(0f, ratio);
        aspd = Mathf.Max(0.01f, dungeonASPD) * Mathf.Max(0f, ratio);
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

        // ⬇️ 여기서 QuickSlot으로 "죽음"을 통보한다
        if (quickSlotOwner != null)
        {
            quickSlotOwner.OnMonsterDied(this);
        }

        Destroy(gameObject);
    }

    public void SetQuickSlotOwner(QuickSlot slot) => quickSlotOwner = slot;
    public QuickSlot GetQuickSlotOwner() => quickSlotOwner;
}
