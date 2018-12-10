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
        
    // delay before return to player position when is no touching
    [SerializeField, Header("Camera Config")]
    float _delayBeforeCameraReturn;

    [SerializeField]
    float _cameraReturnTimer;

    // 2D camera
    public ProCamera2D _proCamera;

    [SerializeField]
    float _camMaxXDif;

    public enum CameraState
    {
        PANNING,
        TARGETING
    }

    [SerializeField]
    CameraState _cameraState;

    public CameraState GetCameraState()
    {
        return _cameraState;
    }

    // launcher instance
    [SerializeField, Header("Gameplay")]
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
    //private TapGestureRecognizer tapGesture;
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
        //CreateTapGesture();
        CreateLongPressGesture();

        // put camera on position
        ReturnCamera();

        // show touches, only do this for debugging as it can interfere with other canvases
        //FingersScript.Instance.ShowTouches = true;
    }

    private void Update()
    {
        CameraStatesUpdate();
    }

    // camera states Updates
    public void CameraStatesUpdate()
    {
        if (_cameraState == CameraState.PANNING)
        {
            if (_proCamera.GetComponent<ProCamera2DPanAndZoom>().IsPanning) // if is panning reset timer
            {
                _cameraReturnTimer = 0;
                return;
            }

            _cameraReturnTimer += Time.deltaTime;

            if (_cameraReturnTimer >= _delayBeforeCameraReturn) // return camera
            {
                ReturnCamera();
            }
        }
        else if (_cameraState == CameraState.TARGETING)
        {

        }
    }

    // change Camera state
    public void SetCameraState(CameraState aState)
    {
        _cameraState = aState;
        if (_cameraState == CameraState.PANNING)
        {            
            _proCamera.GetComponent<ProCamera2DPanAndZoom>().enabled = true;
            _proCamera.RemoveCameraTarget(CPlayer._instance.transform);
            _proCamera.RemoveCameraTarget(CThrowController._instance._secondCameraTarget.transform);
        }
        else if (_cameraState == CameraState.TARGETING)
        {
            _proCamera.AddCameraTarget(CPlayer._instance.transform);
            _proCamera.GetCameraTarget(CPlayer._instance.transform).TargetOffset = new Vector2(0, 3.52f);
            _proCamera.AddCameraTarget(_secondCameraTarget.transform);
            _proCamera.GetCameraTarget(_secondCameraTarget.transform).TargetOffset = new Vector2(0, 3.52f);
            _proCamera.GetComponent<ProCamera2DPanAndZoom>().enabled = false;
        }
    }

    // to know if the gesture start on the player
    private bool GestureIntersectsCancelZone(DigitalRubyShared.GestureRecognizer g, GameObject obj)
    {
        Vector3 worldPos = Camera.main.ScreenToWorldPoint(new Vector3(g.FocusX, g.FocusY, -Camera.main.transform.position.z));
        Collider2D col = Physics2D.OverlapPoint(worldPos, _cancelZoneLayer);
        return (col != null && col.gameObject != null && col.gameObject == obj);
    }

    // to return camera to player position
    [ContextMenu("Return to player")]
    public void ReturnCamera()
    {
        _cameraReturnTimer = 0;
        if (!_proCamera.CameraTargets[0].TargetTransform != CPlayer._instance.transform) // to fix the bug when camera target move the player
        {
            SetCameraState(CameraState.PANNING);
        }
        _proCamera.CameraTargets[0].TargetTransform.position = CPlayer._instance.transform.position + Vector3.up * 3.5f;
    }

    // manage long press gesture
    private void LongPressGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    {
        // start throw only if the start position is on the throw zone / is not a head flying / the player is on idle
        if (_activeHead == null)
        {            
            if (CPlayer._instance.GetState() == CPlayer.PlayerState.IDLE) // if is idle
            {
                _cameraReturnTimer = 0; // reset return timer if is touching the screen

                if (GestureIntersectsCancelZone(gesture, _cancelZone))
                {                 
                    if (gesture.State == GestureRecognizerState.Began)
                    {
                        // enable camera targeting mode / disable pan
                        //ChangeCameraMode(true);
                        SetCameraState(CameraState.TARGETING);

                        CPlayer._instance.SetState(CPlayer.PlayerState.TARGETING);
                    }
                }               
            }
            else if (CPlayer._instance.GetState() == CPlayer.PlayerState.TARGETING) // if is targeting
            { 
                if (gesture.State == GestureRecognizerState.Executing)
                {                  
                    // get touch position                    
                    Vector3 tWorldPos = Camera.main.ScreenToWorldPoint(new Vector3(gesture.FocusX, gesture.FocusY, -Camera.main.transform.position.z));

                    // change player sprites orientation
                    if (tWorldPos.x > CPlayer._instance.transform.position.x) // looking right
                    {
                        CPlayer._instance.UpdatePlayerOrientation(false);
                    }
                    else // looking left
                    {
                        CPlayer._instance.UpdatePlayerOrientation(true);
                    }

                    // update camera position
                    Vector3 tSecondTargetPosition = this.transform.position - (-(this.transform.position - tWorldPos) * 2);
                    _secondCameraTarget.transform.position = new Vector3(tSecondTargetPosition.x, this.transform.position.y, 0);

                    //_secondCameraTarget.transform.position = this.transform.position - (-(this.transform.position - tWorldPos) * 2);

                    // update throw zone limit object
                    _throwZoneLimit.position = this.transform.position + (tWorldPos - this.transform.position).normalized * _throwZoneStartDistance;
                    
                    // limit second target distance
                    if (_secondCameraTarget.transform.localPosition.x > _camMaxXDif) // targeting right
                    {
                        _secondCameraTarget.transform.localPosition = new Vector3(_camMaxXDif, 0, 0);                        
                    }
                    else if (_secondCameraTarget.transform.localPosition.x < -_camMaxXDif) // targeting left
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
                    if (GestureIntersectsCancelZone(gesture, _cancelZone)) // cancel shoot
                    {
                        CPlayer._instance.SetState(CPlayer.PlayerState.IDLE);                       
                    }
                    else
                    {
                        //_longPressGesture.MinimumDurationSeconds = 1f;
                        _launcher.launch = true;
                        CPlayer._instance.SetState(CPlayer.PlayerState.WAITING);
                    }                    
                }
            }
        }
        else // if is an active head
        {
            if (gesture.State == GestureRecognizerState.Began)
            {
                Debug.Log("Touch");
                _activeHead.GetComponent<CHead>().PlayerTouch(); // respawn
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

    //// tap callback
    //private void TapGestureCallback(DigitalRubyShared.GestureRecognizer gesture)
    //{
    //    if (gesture.State == GestureRecognizerState.Ended)
    //    {
    //        Debug.Log("si");
    //        _activeHead.GetComponent<CHead>().PlayerTouch();            
    //    }
    //}

    //// init tap gesture
    //private void CreateTapGesture()
    //{
    //    tapGesture = new TapGestureRecognizer();
    //    tapGesture.StateUpdated += TapGestureCallback;
    //    //tapGesture.RequireGestureRecognizerToFail = doubleTapGesture;
    //    FingersScript.Instance.AddGesture(tapGesture);
    //}
}
