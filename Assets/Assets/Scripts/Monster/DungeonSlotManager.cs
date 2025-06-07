using UnityEngine;

public class DungeonSlotManager : MonoBehaviour
{
    private QuickSlot selectedSlot;

    public void SelectSlot(QuickSlot slot)
    {
        if (selectedSlot != null && selectedSlot != slot)
            selectedSlot.Deselect();

        selectedSlot = slot;
    }

    public void DeleteSelectedSlot()
    {
        if (selectedSlot != null)
            selectedSlot.DeleteMonster();
    }
}
