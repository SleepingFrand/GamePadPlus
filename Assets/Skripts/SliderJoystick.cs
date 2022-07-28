using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;

public class SliderJoystick : MonoBehaviour, IDragHandler, IEndDragHandler
{
    [System.Serializable]
    public class Event : UnityEvent<float> { }
    [SerializeField] private RectTransform Handle;
    [SerializeField] float MoveRadius;
    [SerializeField] private bool HPositionStick;

    [SerializeField] private bool InvertV = false;
    [SerializeField] private bool InvertH = false;

    public Event joystickOutputEvent;

    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(this.GetComponent<RectTransform>(), Input.mousePosition, Camera.main, out Vector2 inputPosition);

        Vector3 HandlePos =Vector3.ClampMagnitude(inputPosition, MoveRadius);

        if (HPositionStick) {
            Handle.localPosition = new Vector3(HandlePos.x, Handle.gameObject.transform.position.y, Handle.gameObject.transform.position.z);
        }
        else
        {
            Handle.localPosition = new Vector3(Handle.localPosition.x, HandlePos.y, Handle.localPosition.z);
        }
        OutputPointerEventValue();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Handle.transform.localPosition = Vector3.zero;
        OutputPointerEventValue();
    }
    private void OutputPointerEventValue()
    {
        Vector2 position = Handle.transform.position - gameObject.transform.position;
        position = Vector2.ClampMagnitude(position, 1.0f);
        ApplyInversionFilter(position);
        joystickOutputEvent.Invoke(HPositionStick? position.x: position.y);
        Debug.Log("Позиция стика: " + (HPositionStick? position.x: position.y));
    }

    Vector2 ApplyInversionFilter(Vector2 position)
    {
        if (InvertH)
        {
            position.x = InvertValue(position.x);
        }

        if (InvertV)
        {
            position.y = InvertValue(position.y);
        }

        return position;
    }

    float InvertValue(float value)
    {
        return -value;
    }
}
