using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class LongPressButton : UIBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler {
    [Tooltip("How long must pointer be down on this object to trigger a long press")]
    public float durationThreshold = 1.0f;

    public UnityEvent onShortPress = new UnityEvent();
    public UnityEvent onLongPress = new UnityEvent();

    private bool isPointerDown = false;
    private bool longPressTriggered = false;
    private float timePressStarted;


    private void Update() {
        if (isPointerDown && !longPressTriggered) {
            if (Time.time - timePressStarted > durationThreshold) {
                longPressTriggered = true;
                onLongPress.Invoke();
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData) {
        timePressStarted = Time.time;
        isPointerDown = true;
        longPressTriggered = false;
    }

    public void OnPointerUp(PointerEventData eventData) {
        isPointerDown = false;
        if (!longPressTriggered) {
            onShortPress.Invoke();
        }
    }


    public void OnPointerExit(PointerEventData eventData) {
        isPointerDown = false;
    }
}