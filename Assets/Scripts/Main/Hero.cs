using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int attackPowerPerSecond = 1;
    [SerializeField] private int goldReward = 10;

    [Header("주변 탐지 반경")]
    [SerializeField] private float detectRadius = 0.25f;

    private int maxHp;
    private float currentHp;

    private bool isFighting = false;
    private bool isDead = false;

    private Rigidbody2D rb;
    private Dungeon dungeonTarget;
    private Coroutine combatCoroutine;
    private HeroSpawner spawner;

    public void Initialize(int hp, HeroSpawner heroSpawner)
    {
        maxHp = hp;
        currentHp = hp;
        spawner = heroSpawner;
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (isDead) return;

        if (!isFighting)
        {
            // 평소엔 왼쪽으로 이동
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);

            // 이동 중 던전과 겹쳐 있으면 전투 시작
            TryStartFightWithDungeonAround();
        }
        else
        {
            // 던전 때리는 중이면, 주변에 몬스터 생기면 우선 몬스터로 타깃 전환
            if (dungeonTarget != null)
            {
                TrySwitchToMonsterIfExists();
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (isDead || currentHp <= 0) return;
        if (other.CompareTag("Hero")) return;

        // 몬스터 먼저 체크
        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null && monster.IsAlive())
            {
                StartFightWithMonster(monster);
            }
            return;
        }

        // 던전
        if (other.TryGetComponent<Dungeon>(out Dungeon dungeon))
        {
            StartFightWithDungeon(dungeon);
        }
    }

    // ========================
    //   몬스터 전투
    // ========================
    private void StartFightWithMonster(Monster monster)
    {
        StopCombat();
        isFighting = true;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        combatCoroutine = StartCoroutine(FightWithMonster(monster));
    }

    private IEnumerator FightWithMonster(Monster monster)
    {
        float heroAttackInterval = 1f;

        while (currentHp > 0)
        {
            if (monster == null || !monster.IsAlive())
            {
                // 몬스터가 죽었으면 전투 종료 후, 던전이 근처에 있으면 다시 던전과 싸움
                StopCombat();
                TryStartFightWithDungeonAround();
                yield break;
            }

            yield return new WaitForSeconds(heroAttackInterval);

            if (monster == null) yield break;

            monster.TakeDamage(attackPowerPerSecond);
            currentHp -= monster.Atk;

            if (currentHp <= 0)
            {
                Die();
                yield break;
            }
        }
    }

    // ========================
    //   던전 전투
    // ========================
    private void StartFightWithDungeon(Dungeon dungeon)
    {
        StopCombat();
        isFighting = true;
        dungeonTarget = dungeon;

        if (rb != null) rb.linearVelocity = Vector2.zero;

        combatCoroutine = StartCoroutine(FightDungeonLoop());
    }

    private IEnumerator FightDungeonLoop()
    {
        while (currentHp > 0)
        {
            if (dungeonTarget == null)
            {
                StopCombat();
                yield break;
            }

            float aspd = dungeonTarget.AttackSpeed;          // 던전의 초당 공격 횟수
            float attackInterval = 1f / Mathf.Max(0.01f, aspd);

            yield return new WaitForSeconds(attackInterval);

            dungeonTarget.TakeDamage(attackPowerPerSecond);
            currentHp -= dungeonTarget.AttackPower;

            if (currentHp <= 0)
            {
                Die();
                yield break;
            }
        }
    }

    // ========================
    //   공통 전투 종료 로직
    // ========================
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hero")) return;

        // 몬스터/던전을 벗어나면 일단 전투 종료
        StopCombat();
    }

    private void StopCombat()
    {
        isFighting = false;
        dungeonTarget = null;

        if (combatCoroutine != null)
        {
            StopCoroutine(combatCoroutine);
            combatCoroutine = null;
        }
    }

    // ========================
    //   사망
    // ========================
    private void Die()
    {
        if (isDead) return;
        isDead = true;

        StopCombat();
        if (rb != null) rb.linearVelocity = Vector2.zero;

        if (GameState.I != null)
            GameState.I.AddGold(goldReward);

        spawner?.OnHeroDeath(gameObject);
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        StopCombat();
    }

    // ========================
    //   주변 탐지 유틸
    // ========================
    private void TryStartFightWithDungeonAround()
    {
        // 주변에서 Dungeon 콜라이더 찾기
        var hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
        foreach (var hit in hits)
        {
            if (hit.TryGetComponent<Dungeon>(out Dungeon dungeon))
            {
                StartFightWithDungeon(dungeon);
                return;
            }
        }
    }

    private void TrySwitchToMonsterIfExists()
    {
        var hits = Physics2D.OverlapCircleAll(transform.position, detectRadius);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Monster"))
            {
                var m = hit.GetComponent<Monster>();
                if (m != null && m.IsAlive())
                {
                    StartFightWithMonster(m);
                    return;
                }
            }
        }
    }
}
