using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PuzzleSlot : MonoBehaviour, IDropHandler
{
#pragma warning disable 0649 // is never assigned to, and will always have its default value null.
    public PuzzleSlotModel puzzleSlot;

    [SerializeField]
    private Image _image;

#pragma warning restore 0649
    public void OnDrop(PointerEventData eventData)
    {
        EventController.Invoke(EventMessage.ON_DROP, eventData, this);
    }

    public void SetSlot()
    {
        _image.color = Color.white;
    }
}
