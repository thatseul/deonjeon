// GoldManager.cs
using UnityEngine;
using TMPro;

/// <summary>
/// 현재 골드 보유량을 관리하고, 증가 및 UI 갱신 등을 담당하는 클래스
/// </summary>
public class GoldManager : MonoBehaviour
{
    public static GoldManager Instance { get; private set; }

    [Tooltip("현재 플레이어가 보유 중인 골드")] 
    [SerializeField] private int currentGold = 0;

    [Header("UI 연결")]
    [SerializeField] private TextMeshProUGUI goldText;

    private void Awake()
    {
        // 싱글톤 패턴 (씬 내 유일한 골드 매니저)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

        }
    }

    private void Start()
        {
            UpdateGoldUI();
        }
    /// <summary>
    /// 골드 추가 (기본값: 1)
    /// </summary>
    public void AddGold(int amount = 10)
    {
        currentGold += amount;
        Debug.Log($"💰 골드 획득! 현재 골드: {currentGold}");
        UpdateGoldUI();

        // TODO: UI 갱신 코드 추가 예정
    }

    /// <summary>
    /// 현재 골드 수치 반환
    /// </summary>
    public int GetCurrentGold() => currentGold;

    private void UpdateGoldUI()
    {
        if (goldText != null)
        {
            goldText.text = $"Gold: {currentGold}";
        }
    }

    public bool SpendGold(int amount)
    {
        if (currentGold >= amount)
        {
            currentGold -= amount;
            Debug.Log($"💸 골드 사용! 남은 골드: {currentGold}");
            // UI 갱신
            UpdateGoldUI();
            return true;
        }
        else
        {
            Debug.LogWarning("❌ 골드 부족!");
            return false;
        }
    }

}
