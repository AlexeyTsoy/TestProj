using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private GameObject _draggingObject;
    private ScrollRect _scrollRect;
    private Transform _parent;

    private bool _isScrolling;

    private int _siblingIndex;

    private void Start()
    {
        _scrollRect = GetComponentInParent<ScrollRect>();
        _parent = _scrollRect.content;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (_scrollRect != null && eventData.delta.y < 5f && eventData.delta.y > -5f)
        {
            _isScrolling = true;
            _draggingObject = null;
            _scrollRect.OnBeginDrag(eventData);
            return;
        }

        var canvas = FindInParents<Canvas>(gameObject);

        if (canvas == null)
            return;

        // We have clicked something that can be dragged.
        // What we want to do is create an icon for this.
        _draggingObject = gameObject;
        _siblingIndex = _draggingObject.transform.GetSiblingIndex();
        _draggingObject.transform.SetParent(canvas.transform, false);
        _draggingObject.transform.SetAsLastSibling();

        //var image = _draggingObject.AddComponent<Image>();
        //image.sprite = GetComponent<Image>().sprite;
        //image.SetNativeSize();
        //_draggingObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);

        _draggingObject.GetComponent<Image>().raycastTarget = false;

        if (_draggingObject != null)
            SetDraggedPosition(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (_isScrolling)
        {
            _draggingObject = null;
            _scrollRect.OnDrag(eventData);
            return;
        }

        if (_draggingObject != null)
            SetDraggedPosition(eventData);
    }

    private void SetDraggedPosition(PointerEventData eventData)
    {
        var rt = _draggingObject.GetComponent<RectTransform>();

        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rt, eventData.position,
            eventData.pressEventCamera, out var globalMousePos))
        {
            rt.position = globalMousePos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (_isScrolling)
        {
            _scrollRect.OnEndDrag(eventData);
            _isScrolling = false;
            return;
        }

        if (_draggingObject != null)
        {
            _draggingObject.transform.SetParent(_parent);
            _draggingObject.transform.localPosition = Vector2.zero;
            _draggingObject.GetComponent<Image>().raycastTarget = true;
            _draggingObject.transform.SetSiblingIndex(_siblingIndex);

        }
        //    Destroy(_draggingObject);
    }

    private static T FindInParents<T>(GameObject go) where T : Component
    {
        if (go == null) return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        var t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}