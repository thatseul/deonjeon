using UnityEngine;

/// <summary>
/// 일정 속도로 왼쪽으로 이동하며,
/// 던전과 충돌 시 데미지를 주고 사라지는 용사.
/// </summary>
public class Hero : MonoBehaviour
{
    [Header("이동 설정")]
    [Tooltip("초당 이동 속도")]
    [SerializeField] private float moveSpeed = 2f;

    private HeroSpawner spawner;

    [Header("체력 설정")]
    [Tooltip("용사의 최대 체력")]
    private int maxHp;

    [Tooltip("용사의 현재 체력")]
    private int currentHp;

    /// <summary>
    /// HeroSpawner에서 생성 시 초기화용.
    /// </summary>
    /// <param name="hp">최대 체력</param>
    /// <param name="heroSpawner">스포너 참조</param>
    public void Initialize(int hp, HeroSpawner heroSpawner)
    {
        maxHp = hp;
        currentHp = hp;
        spawner = heroSpawner;
    }

    private void Update()
    {
        // 왼쪽으로 이동
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    /// <summary>
    /// 던전과 충돌 시 데미지를 주고 제거됨.
    /// </summary>
    /// <param name="collision">충돌 대상</param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent<Dungeon>(out Dungeon dungeon))
        {
            dungeon.TakeDamage(1);

            // 스포너에게 사망 알림
            spawner?.OnHeroDeath(gameObject);

            // TODO: 추후 이펙트나 사운드 추가 가능
            Destroy(gameObject);
        }
    }
}
