using UnityEngine;

public class Dungeon : MonoBehaviour
{
    public void UpgradeHp(int amount)
    {
    maxHp += amount;
    Debug.Log($"던전 HP 강화됨! maxHp: {maxHp}");
    }
    [Header("던전 체력 설정")]
    [Tooltip("던전의 최대 체력입니다. 반복 루프 시 항상 이 수치로 복원됩니다.")]
    [SerializeField] private int maxHp = 10;

    public int CurrentHp => currentHp;

    private int currentHp;

    // ✅ 추가: 죽었는지 확인하는 플래그
    private bool isDead = false;

    private void Start()
    {
        currentHp = maxHp;
        isDead = false;
    }

    /// <summary>
    /// 던전이 용사에게 공격받을 때 호출됩니다.
    /// </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) // 죽은 상태면 무시
        {
        Debug.Log("데미지 무시됨: 던전 이미 죽은 상태");
        return;
        } 

        currentHp -= damage;
        Debug.Log($"Dungeon HP: {currentHp}/{maxHp}");

        if (currentHp <= 0)
        {
            isDead = true;
            currentHp = 0;
            Debug.Log("☠️ Dungeon 사망! 초기화 준비...");
            Invoke(nameof(ResetDungeonState), 1f);
        }
    }

    private void DieAndRespawn()
    {
        Debug.Log("💥 Dungeon HP 0! Resetting dungeon state...");
        Invoke(nameof(ResetDungeonState), 1f);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"[Dungeon 입장 로그] 충돌자: {other.name}");
    }
    private void ResetDungeonState()
    {
        currentHp = maxHp; // 강화된 maxHp 기준
        isDead = false;
        Debug.Log($"Dungeon HP restored to {currentHp}/{maxHp}");

        foreach (var hero in GameObject.FindGameObjectsWithTag("Hero"))
        {
            Destroy(hero);
        }

        HeroSpawner spawner = FindObjectOfType<HeroSpawner>();
        if (spawner != null)
        {
            spawner.ResetSpawner(); // 구현되어 있다면 OK
        }
        Debug.Log($"현재 씬 내 Dungeon 수: {GameObject.FindGameObjectsWithTag("Dungeon").Length}");
        Debug.Log($"던전 상태: active={gameObject.activeInHierarchy}, collider enabled={GetComponent<Collider2D>().enabled}");

    }
}
