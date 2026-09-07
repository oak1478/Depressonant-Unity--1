using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlotHover : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public int slotIndex;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSlotHoverEnter(slotIndex);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (InventoryManager.Instance != null)
        {
            InventoryManager.Instance.OnSlotHoverExit(slotIndex);
        }
    }
}
