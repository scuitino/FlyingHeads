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

    [SerializeField]
    float _camMaxXDif;

    // launcher instance
    [SerializeField]
    Launcher2D _launcher;

    // to see if is a head in use
    public GameObject _activeHead;

    // to adjust camera when targeting
    public GameObject _secondCameraTarget;

    // throw zone start limit
    public Transform _throwZoneLimit;
    [SerializeField]
    float _throwZoneStartDistance;

    // add tap gesture
    private TapGestureRecognizer tapGesture;
    // long press gesture instance
    public LongPressGestureRecognizer _longPressGesture;

    // throw cancel zone
    [SerializeField]
    GameObject _cancelZone;

    // cancel zone Layer
    [SerializeField]
    LayerMask _cancelZoneLayer;

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
        // init gesture listeners
        CreateTapGesture();
        CreateLongPressGesture();

        // show touches, only do this for debugging as it can interfere with other canvases
        //FingersScript.Instance.ShowTouches = true;
    }

    // to know if the gesture start on the player
    private bool GestureIntersectsCancelZone(DigitalRubyShared.GestureRecognizer g, GameObject obj)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(g.FocusX, g.FocusY, -Camera.main.transform.position.z));
        Collider2D col = Physics2D.OverlapPoint(worldPos, _cancelZoneLayer);
        return (col != null && col.gameObject != null && col.gameObject == obj);
    }

    // manage long press gesture
    private void LongPressGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        // start throw only if the start position is on the throw zone / is not a head flying / the player is on idle
        if (_activeHead == null)
        {            
            if (CPlayer._instance.GetState() == CPlayer.PlayerState.IDLE) // if is idle
            {
                if (gesture.State == GestureRecognizerState.Began)
                {
                    Debug.Log("empezo");
                    CPlayer._instance.SetState(CPlayer.PlayerState.TARGETING);
                }
            }
            else if (CPlayer._instance.GetState() == CPlayer.PlayerState.TARGETING) // if is targeting
            { 
                if (gesture.State == GestureRecognizerState.Executing)
                {                  
                    // get touch position                    
                    Vector3 tWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, -Camera.main.transform.position.z));

                    // update camera position
                    Vector3 tSecondTargetPosition = this.transform.position - (-(this.transform.position - tWorldPos) * 2);
                    _secondCameraTarget.transform.position = new Vector3(tSecondTargetPosition.x, this.transform.position.y, 0);

                    //_secondCameraTarget.transform.position = this.transform.position - (-(this.transform.position - tWorldPos) * 2);

                    // update throw zone limit object
                    _throwZoneLimit.position = this.transform.position + (tWorldPos - this.transform.position).normalized * _throwZoneStartDistance;

                    if (_secondCameraTarget.transform.localPosition.x > _camMaxXDif)
                    {                        
                        _secondCameraTarget.transform.localPosition = new Vector3(_camMaxXDif, 0, 0);
                    }
                    else if (_secondCameraTarget.transform.localPosition.x < -_camMaxXDif)
                    {
                        _secondCameraTarget.transform.localPosition = new Vector3(-_camMaxXDif, 0, 0);
                    }

                    // update throw data with touch position
                    if (!GestureIntersectsCancelZone(gesture, _cancelZone)) // only target if is outside the cancel zone
                    {
                        _launcher.UpdateThrowData(tWorldPos);
                    }                    
                }
                else if (gesture.State == GestureRecognizerState.Ended)
                {
                    Debug.Log("termino");
                    if (GestureIntersectsCancelZone(gesture, _cancelZone)) // cancel shoot
                    {
                        Debug.Log("solto en cancel zone");
                        CPlayer._instance.SetState(CPlayer.PlayerState.IDLE);
                    }
                    else
                    {
                        _longPressGesture.MinimumDurationSeconds = 1f;
                        _launcher.launch = true;
                        CPlayer._instance.SetState(CPlayer.PlayerState.WAITING);
                    }                    
                }
            }
        }        
    }

    // init the long press gesture
    private void CreateLongPressGesture()
    {
        _longPressGesture = new LongPressGestureRecognizer();
        _longPressGesture.MaximumNumberOfTouchesToTrack = 1;
        _longPressGesture.StateUpdated += LongPressGestureCallback;
        _longPressGesture.MinimumDurationSeconds = 0f;
        FingersScript.Instance.AddGesture(_longPressGesture);
    }

    // tap callback
    private void TapGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        if (gesture.State == GestureRecognizerState.Ended)
        {
            Debug.Log("si");
            _activeHead.GetComponent<CHead>().PlayerTouch();            
        }
    }

    // init tap gesture
    private void CreateTapGesture()
    {
        tapGesture = new TapGestureRecognizer();
        tapGesture.StateUpdated += TapGestureCallback;
        //tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
        FingersScript.Instance.AddGesture(tapGesture);
    }
}
