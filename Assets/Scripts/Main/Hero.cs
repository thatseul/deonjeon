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
            return;
        }

        if (other.TryGetComponent<Dungeon>(out Dungeon dungeon))
        {
            dungeonTarget = dungeon;
            damageCoroutine = StartCoroutine(FightLoop());
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
                isDead = true;
                rb.linearVelocity = Vector2.zero;
                StopCombat();
                GoldManager.Instance?.AddGold();
                spawner?.OnHeroDeath(gameObject);
                Destroy(gameObject);
                yield break;
            }
        }
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