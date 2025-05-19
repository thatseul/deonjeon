using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class QuickSlot : MonoBehaviour, IDropHandler
{
    [Header("몬스터 소환 위치")]
    [Tooltip("실제로 몬스터가 소환될 월드 위치 오브젝트")]
    [SerializeField] private Transform summonPosition;

    [Header("던전 능력치 매니저")]
    [Tooltip("DungeonStatsManager 참조")]
    [SerializeField] private DungeonStatsManager statsManager;

    [Header("UI 이미지 표시용")]
    [SerializeField] private Image slotImage;

    public void OnDrop(PointerEventData eventData)
    {
        // 드래그된 오브젝트에서 MonsterSlot 컴포넌트 가져오기
        MonsterSlot draggedSlot = eventData.pointerDrag?.GetComponent<MonsterSlot>();
        if (draggedSlot == null) return;

        Debug.Log("🔍 소환 위치: " + summonPosition.position);

        // 해당 슬롯에 있는 몬스터 정보 가져오기
        MonsterItem item = draggedSlot.GetAssignedMonster();
        if (item == null || item.monsterPrefab == null)
        {
            Debug.LogWarning("❌ 소환 실패: 몬스터 정보 또는 프리팹이 비어 있음");
            return;
        }

        // 몬스터 프리팹 Instantiate (실제 생성)
        GameObject monsterObj = Instantiate(item.monsterPrefab, summonPosition.position, Quaternion.identity);

        // 능력치 가져오기
        float hp = statsManager.GetStatValue("HP");
        float atk = statsManager.GetStatValue("ATK");
        float aspd = statsManager.GetStatValue("ASPD");

        // 능력치 비율 적용
        Monster monster = monsterObj.GetComponent<Monster>();
        if (monster != null)
        {
            monster.Init(hp, atk, aspd, item.statScaleRatio);
            Debug.Log($"✅ {item.monsterName} 소환 완료! 능력치 비율: {item.statScaleRatio * 100}%");
        }
        else
        {
            Debug.LogWarning("⚠️ Monster.cs 컴포넌트가 프리팹에 없음!");
        }

        // UI 이미지 표시
        if (slotImage != null)
        {
            slotImage.sprite = item.monsterIcon; // MonsterItem 안에 Sprite 아이콘이 있어야 함
            slotImage.color = Color.white;       // 혹시 투명할 수도 있어서 색상 흰색
        }

    }
}
