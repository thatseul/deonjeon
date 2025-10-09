using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// 플레이어가 조작하는 던전 본체. 체력/공격력은 DungeonStatsManager에서 가져와 사용.
/// </summary>
public class Dungeon : MonoBehaviour
{
    [Header("스탯 관리자")]
    [Tooltip("Dungeon의 능력치를 관리하는 스크립트")]
    [SerializeField] private DungeonStatsManager statsManager;
    [SerializeField] private Slider hpBar;

    // 외부에서 읽는 전투 파라미터 (널/이상치 방지 포함)
    public float MaxHp      => statsManager ? Mathf.Max(1f, statsManager.GetStatValue("HP"))   : 100f;
    public float AttackPower => statsManager ? Mathf.Max(0f, statsManager.GetStatValue("ATK")) : 1f;
    public float AttackSpeed => statsManager ? Mathf.Max(0.01f, statsManager.GetStatValue("ASPD")) : 1f;

    private float currentHp;
    private bool isDead;

    // 씬 재로딩/오브젝트 재활성화 시도 항상 안전하게 시작
    private void OnEnable()
    {
        InitState();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        // Start에서도 한 번 더 보정(직접 활성화 케이스)
        if (currentHp <= 0f) InitState();
    }

    private void OnSceneLoaded(Scene sc, LoadSceneMode mode)
    {
        // 메인 씬에 돌아왔을 때 상태 보장
        if (sc.name == "deonjeon2020" && gameObject.activeInHierarchy)
        {
            InitState();
        }
    }

    private void InitState()
    {
        isDead = false;
        currentHp = MaxHp;
        if (hpBar)
        {
            hpBar.maxValue = MaxHp;
            hpBar.value = currentHp;
        }
    }

    /// <summary> 용사에게 피해를 받았을 때 호출됩니다. </summary>
    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHp -= damage;
        if (currentHp < 0) currentHp = 0;

        if (hpBar) hpBar.value = currentHp;

        if (currentHp <= 0)
        {
            isDead = true;
            // 페널티/연출 등 필요 시 여기서 GameState 연동 가능
            // ex) GameState.I?.OnDungeonDeath();
            Invoke(nameof(ResetDungeonState), 1f);
        }
    }

    /// <summary>
    /// 던전 체력 복원 & 히어로 정리 & 스폰 재시작
    /// </summary>
    private void ResetDungeonState()
    {
        InitState();

        // 모든 히어로 제거
        var heroes = GameObject.FindGameObjectsWithTag("Hero");
        for (int i = 0; i < heroes.Length; i++)
        {
            if (heroes[i]) Destroy(heroes[i]);
        }

        // 히어로 스폰 재시작
        var spawner = FindFirstObjectByType<HeroSpawner>();
        if (spawner) spawner.ResetSpawner();
    }
}
