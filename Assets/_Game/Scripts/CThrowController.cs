﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DigitalRubyShared;

public class CThrowController : MonoBehaviour {

    // launcher instance
    [SerializeField]
    Launcher2D _launcher;

    // long press gesture instance
    private LongPressGestureRecognizer _longPressGesture;

    // when the touch needs to start when you want to throw
    [SerializeField]
    GameObject _throwZone;

    [SerializeField]
    LayerMask _throwZoneLayer;

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
        Collider2D col = Physics2D.OverlapPoint(worldPos, _throwZoneLayer);
        return (col != null && col.gameObject != null && col.gameObject == obj);
    }

    // manage long press gesture
    private void LongPressGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (GestureIntersectsSprite(gesture, _throwZone)) // start throw only if the start position is on the throw zone
        {
            if (gesture.State == GestureRecognizerState.Began)
            {
                Debug.Log("empezo");
            }
            else if (gesture.State == GestureRecognizerState.Executing)
            {
                // update throw data with touch position
                Vector2 tWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, -Camera.main.transform.position.z));
                _launcher.UpdateThrowData(tWorldPos);
            }
            else if (gesture.State == GestureRecognizerState.Ended)
            {
                Debug.Log("termino");
                // fire
                _launcher.launch = true;
            }
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
