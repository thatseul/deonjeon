// Hero.cs
using System.Collections;
using UnityEngine;

/// <summary>
/// 일정 속도로 왼쪽으로 이동하며,
/// 던전이나 장애물과 충돌 시 멈춰서 서로 공격하는 용사.
/// </summary>
public class Hero : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("초당 이동 속도")]
    [SerializeField] private float moveSpeed = 2f;

    private bool isFighting = false;
    private Rigidbody2D rb;

    private HeroSpawner spawner;
    private Dungeon dungeonTarget;
    private Coroutine damageCoroutine;

    [Header("체력 설정")]
    [Tooltip("용사의 최대 체력")]
    private int maxHp;

    [Tooltip("용사의 현재 체력")]
    private int currentHp;

    [Header("공격 설정")]
    [Tooltip("초당 공격력")]
    [SerializeField] private int attackPowerPerSecond = 1;

    /// <summary>
    /// HeroSpawner에서 생성 시 초기화용.
    /// </summary>
    public void Initialize(int hp, HeroSpawner heroSpawner)
    {
        maxHp = hp;
        currentHp = hp;
        spawner = heroSpawner;

        rb = GetComponent<Rigidbody2D>();

        Debug.Log($"🧠 Hero 초기화됨! currentHp={currentHp}, RigidbodyType={rb.bodyType}");
    }

    private void Update()
    {
        if (!isFighting)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Hero끼리의 충돌은 무시
        if (other.CompareTag("Hero")) return;

        // 이미 죽은 히어로는 충돌 반응하지 않음
        if (currentHp <= 0) return;

        // 전투 시작: 이동 멈추고 공격 코루틴 시작
        isFighting = true;
        rb.velocity = Vector2.zero;

        Debug.Log($"⚔️ Hero 전투 시작! 충돌 대상: {other.name}");

        // 던전이면 참조 보관하여 데미지 줌
        if (other.TryGetComponent<Dungeon>(out Dungeon dungeon))
        {
            dungeonTarget = dungeon;
        }

        // 전투 시작
        damageCoroutine = StartCoroutine(FightLoop());
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // Hero끼리 나간 건 무시
        if (other.CompareTag("Hero")) return;

        Debug.Log("🏃 Hero 충돌 해제 - 전투 종료");
        StopCombat();
    }

    /// <summary>
    /// 전투 루프 - 초당 서로 공격
    /// </summary>
    private IEnumerator FightLoop()
    {
        while (currentHp > 0)
        {
            yield return new WaitForSeconds(1f);

            // Hero가 Dungeon에 데미지 줌
            if (dungeonTarget != null)
            {
                dungeonTarget.TakeDamage(attackPowerPerSecond);
            }

            // Dungeon이 Hero에게 데미지 줌
            if (dungeonTarget != null)
            {
                currentHp -= dungeonTarget.AttackPower;
                Debug.Log($"💢 Hero 피해 중! 남은 HP: {currentHp}/{maxHp}");

                if (currentHp <= 0)
                {
                    Debug.Log("💀 Hero 사망 처리");
                    spawner?.OnHeroDeath(gameObject);
                    Destroy(gameObject);
                }
            }
        }
    }

    private void StopCombat()
    {
        isFighting = false;
        dungeonTarget = null;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        Debug.Log("💀 Hero 파괴됨");
    }
}