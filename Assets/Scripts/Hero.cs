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
    public void Initialize(int hp, HeroSpawner heroSpawner)
    {
        maxHp = hp;
        currentHp = hp;
        spawner = heroSpawner;

        Debug.Log($"🧠 Hero 초기화됨! currentHp={currentHp}, RigidbodyType={GetComponent<Rigidbody2D>().bodyType}");
    }

    private void Update()
    {
        // 왼쪽으로 이동
        transform.Translate(Vector2.left * moveSpeed * Time.deltaTime);
    }

    private void OnDestroy()
    {
        Debug.Log("💀 Hero 파괴됨");
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"⚔️ Hero 충돌 감지! 대상: {other.name}, 레이어: {LayerMask.LayerToName(other.gameObject.layer)}");

        // 던전과 충돌했는지 확인
        if (other.TryGetComponent<Dungeon>(out Dungeon dungeon))
        {
            Debug.Log($"🗡 Hero가 Dungeon에게 데미지를 줌! Dungeon 현재 HP: {dungeon.CurrentHp}");

            dungeon.TakeDamage(1);
            spawner?.OnHeroDeath(gameObject);

            // 바로 Destroy 시 로그 유실 가능 → 한 프레임 뒤에 제거
            StartCoroutine(DestroyNextFrame());
        }
        else
        {
            Debug.LogWarning("❌ Dungeon 컴포넌트 없음. 충돌한 오브젝트의 컴포넌트 목록:");
            foreach (var comp in other.GetComponents<Component>())
            {
                Debug.Log($"🔍 {comp.GetType().Name}");
            }
        }
    }

    /// <summary>
    /// Destroy 지연 처리: 로그 출력 보장 및 충돌 안정화
    /// </summary>
    private System.Collections.IEnumerator DestroyNextFrame()
    {
        yield return null;
        Destroy(gameObject);
    }
}
