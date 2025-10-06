using System.Collections;
using UnityEngine;

public class Hero : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private int attackPowerPerSecond = 1;

    private int maxHp;
    private float currentHp;

    private bool isFighting = false;
    private bool isDead = false;
    private Rigidbody2D rb;
    private Dungeon dungeonTarget;
    private Coroutine damageCoroutine;
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
        if (!isFighting && !isDead)
        {
            transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
        }
    }


    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Hero") || currentHp <= 0) return;

        isFighting = true;
        rb.linearVelocity = Vector2.zero;

        if (other.CompareTag("Monster"))
        {
            Monster monster = other.GetComponent<Monster>();
            if (monster != null)
                StartCoroutine(FightWithMonster(monster));
            return;
        }

        if (other.TryGetComponent<Dungeon>(out Dungeon dungeon))
        {
            dungeonTarget = dungeon;
            damageCoroutine = StartCoroutine(FightLoop());
        }
    }

    private IEnumerator FightWithMonster(Monster monster)
    {
        float heroAttackInterval = 1f;

        while (currentHp > 0 && monster != null && monster.IsAlive())
        {
            yield return new WaitForSeconds(heroAttackInterval);

            // 🔐 안전한 접근: Destroy 되었는지 확인
            if (monster == null || !monster.gameObject) yield break;

            monster.TakeDamage(attackPowerPerSecond);
            currentHp -= monster.Atk;

            if (!monster.IsAlive())
            {
                yield break;
            }

            if (currentHp <= 0)
            {
                Die();
                yield break;
            }
        }
    }
    private IEnumerator FightLoop()
    {
        while (currentHp > 0)
        {
            //공격 속도에 따라 대기 시간 계산
            float attackInterval = 1f;

            if (dungeonTarget != null)
            {
                float aspd = dungeonTarget.AttackSpeed; // 초당 공격 횟수
                attackInterval = 1f / aspd;
            }

            yield return new WaitForSeconds(attackInterval);


            if (dungeonTarget == null)
                yield break;

            dungeonTarget.TakeDamage(attackPowerPerSecond);
            currentHp -= dungeonTarget.AttackPower;

            if (currentHp <= 0)
            {
                Die();
                yield break;
            }
        }
    }
    private void Die()
    {
        isDead = true;
        rb.linearVelocity = Vector2.zero;
        StopCombat();
        GoldManager.Instance?.AddGold();
        spawner?.OnHeroDeath(gameObject);
        Destroy(gameObject);
    }
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Hero")) return;
        StopCombat();
    }

    private void StopCombat()
    {
        isFighting = false;

        if (damageCoroutine != null)
        {
            StopCoroutine(damageCoroutine);
            damageCoroutine = null;
        }

        dungeonTarget = null;
    }

    private void OnDestroy()
    {
        StopCombat();
    }
}