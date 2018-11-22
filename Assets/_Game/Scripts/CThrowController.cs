using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DigitalRubyShared;

public class CThrowController : MonoBehaviour {

    // throw values
    //Vector2 _pressStartPosition, _pressEndPosition;

    // long press gesture instance
    private LongPressGestureRecognizer _longPressGesture;

    private void Start()
    {
        CreateLongPressGesture();

        // show touches, only do this for debugging as it can interfere with other canvases
        FingersScript.Instance.ShowTouches = true;
    }

    // to know if the gesture start on the player
    private bool GestureIntersectsSprite(DigitalRubyShared.GestureRecognizer g, GameObject obj)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(g.StartFocusX, g.StartFocusY, -Camera.main.transform.position.z));
        Collider2D col = Physics2D.OverlapPoint(worldPos);
        return (col != null && col.gameObject != null && col.gameObject == obj);
    }

    // manage long press gesture
    private void LongPressGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Began)
        {
            Debug.Log("empezo");
            Debug.Log(gesture.FocusX + " x / y " + gesture.FocusY);
            // set gesture start position
            //_pressStartPosition = new Vector2(gesture.DeltaX, gesture.);
            //DebugText("Long press began: {0}, {1}", gesture.FocusX, gesture.FocusY);
            //BeginDrag(gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Executing)
        {
            Debug.Log(gesture.DistanceX + " x / y " + gesture.DistanceY);
            //Debug.Log("continua");
            //DebugText("Long press moved: {0}, {1}", gesture.FocusX, gesture.FocusY);
            //DragTo(gesture.FocusX, gesture.FocusY);
        }
        else if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.Log("termino");
            Debug.Log(gesture.FocusX + " x / y " + gesture.FocusY);
            // set gesture end position
            //_pressEndPosition = new Vector2(gesture.FocusX, gesture.FocusY);
            //DebugText("Long press end: {0}, {1}, delta: {2}, {3}", gesture.FocusX, gesture.FocusY, gesture.DeltaX, gesture.DeltaY);
            //EndDrag(longPressGesture.VelocityX, longPressGesture.VelocityY);
        }
    }

    // init the long press gesture
    private void CreateLongPressGesture()
    {
        _longPressGesture = new LongPressGestureRecognizer();
        _longPressGesture.MaximumNumberOfTouchesToTrack = 1;
        _longPressGesture.StateUpdated += LongPressGestureCallback;
        _longPressGesture.MinimumDurationSeconds = 0;
        FingersScript.Instance.AddGesture(_longPressGesture);
    }
}
