using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MonsterBuyButton : MonoBehaviour
{
    [Header("몬스터 데이터")]
    [Tooltip("소환 가능한 몬스터 리스트 (ScriptableObject들)")]
    [SerializeField] private List<MonsterItem> availableMonsters;

    [Header("UI 프리팹 및 배치 대상")]
    [Tooltip("몬스터 아이콘 UI 프리팹 (1개만 필요)")]
    [SerializeField] private GameObject monsterIconPrefab;

    [Tooltip("MonsterIconPrefab이 들어갈 패널")]
    [SerializeField] private RectTransform monsterInvenPanel;

    [Tooltip("몬스터 구매 버튼")]
    [SerializeField] private Button buyButton;

    [Tooltip("배치 시 테두리 여백")]
    [SerializeField] private float padding = 20f;

    private void Start()
    {
        if (buyButton != null)
            buyButton.onClick.AddListener(BuyRandomMonster);
    }

    private void BuyRandomMonster()
    {
        if (availableMonsters == null || availableMonsters.Count == 0)
        {
            Debug.LogWarning("❌ 몬스터 리스트가 비어 있습니다.");
            return;
        }

        // 몬스터 하나 랜덤 선택
        MonsterItem selected = availableMonsters[Random.Range(0, availableMonsters.Count)];

        // 골드 확인
        if (GoldManager.Instance.GetCurrentGold() < selected.cost)
        {
            Debug.LogWarning("💸 골드 부족!");
            return;
        }

        // 골드 차감
        GoldManager.Instance.SpendGold(selected.cost);

        // 몬스터 UI 프리팹 생성
        GameObject icon = Instantiate(monsterIconPrefab, monsterInvenPanel);

        // MonsterItem 정보 적용 (이름, 아이콘 등)
        icon.GetComponent<MonsterIcon>()?.SetMonsterItem(selected);

        // 랜덤 위치 지정
        Vector2 size = monsterInvenPanel.rect.size;
        float x = Random.Range(padding, size.x - padding);
        float y = Random.Range(padding, size.y - padding);
        icon.GetComponent<RectTransform>().anchoredPosition = new Vector2(x, y);

        Debug.Log($"🎲 랜덤 소환: {selected.monsterName} → 위치 ({x:F0}, {y:F0})");
    }
}
