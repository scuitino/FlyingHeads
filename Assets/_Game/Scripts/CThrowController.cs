using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DigitalRubyShared;
using Com.LuisPedroFonseca.ProCamera2D;

public class CThrowController : MonoBehaviour {

    #region SINGLETON PATTERN
    public static CThrowController _instance = null;
    #endregion

    // 2D camera
    public ProCamera2D _proCamera;

    // launcher instance
    [SerializeField]
    Launcher2D _launcher;

    // to see if is a head in use
    public GameObject _activeHead;

    // long press gesture instance
    private LongPressGestureRecognizer _longPressGesture;

    // when the touch needs to start when you want to throw
    [SerializeField]
    GameObject _throwZone;
    [SerializeField]
    LayerMask _throwZoneLayer;

    private void Awake()
    {
        //singleton check
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        // init gesture listener
        CreateLongPressGesture();

        // show touches, only do this for debugging as it can interfere with other canvases
        FingersScript.Instance.ShowTouches = true;
    }

    private void Update()
    {
        if (_activeHead == null)
        {
            _proCamera.CameraTargets[0].TargetTransform = this.transform;
        }
        else
        {
            _proCamera.CameraTargets[0].TargetTransform = _activeHead.transform;
        }
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
        if (GestureIntersectsSprite(gesture, _throwZone) && _activeHead == null) // start throw only if the start position is on the throw zone
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
