using UnityEngine;

public class MonsterDeleteButton : MonoBehaviour
{
    [SerializeField] private DungeonSlotManager dungeonSlotManager;

    public void OnClickDelete()
    {
        dungeonSlotManager.DeleteSelectedSlot();
    }
}
