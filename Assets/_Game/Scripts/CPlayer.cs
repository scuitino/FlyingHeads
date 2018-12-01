using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayer : MonoBehaviour {

    #region SINGLETON PATTERN
    public static CPlayer _instance = null;
    #endregion

    [SerializeField]
    float _moveSpeed;

    [SerializeField]
    Rigidbody2D _playerRB;

    [SerializeField]
    GameObject _headSprite;

    [SerializeField]
    GameObject _spawnParticle;

    public enum PlayerState
    {
        IDLE,
        FALLING,
        TARGETING,
        WAITING,
        SPAWNING
    }

    [SerializeField]
    PlayerState _state;

    public PlayerState GetState()
    {
        return _state;
    }

    private void Awake()
    {
        //singleton check
        if (_instance == null)
            _instance = this;
        else if (_instance != this)
            Destroy(gameObject);
    }

    private void Update()
    {
        StatesUpdate();
    }

    // change player state
    public void SetState(PlayerState aState)
    {
        _state = aState;
        if (_state == PlayerState.IDLE)
        {
            CThrowController._instance._longPressGesture.MinimumDurationSeconds = 0f;
            _playerRB.drag = 0;
            _headSprite.SetActive(true);
        }
        else if (_state == PlayerState.FALLING)
        {
            _playerRB.drag = 0;
        }
        else if (_state == PlayerState.TARGETING)
        {
            _headSprite.SetActive(false);
        }
        else if (_state == PlayerState.WAITING)
        {
            CThrowController._instance._proCamera.RemoveCameraTarget(CThrowController._instance._secondCameraTarget.transform);            
        }
        else if (_state == PlayerState.SPAWNING)
        {
            CThrowController._instance._secondCameraTarget.transform.position = this.transform.position;
            CThrowController._instance._proCamera.AddCameraTarget(CThrowController._instance._secondCameraTarget.transform);
            _spawnParticle.SetActive(true);
            SetState(PlayerState.IDLE);            
        }        
    }

    void StatesUpdate()
    {
        if (_state == PlayerState.IDLE)
        {
        
        }
        else if (_state == PlayerState.FALLING)
        {
            if (_playerRB.velocity.y == 0)
            {
                SetState(PlayerState.IDLE);
            }
        }
        else if (_state == PlayerState.TARGETING)
        {
            //if (Launcher2D._instance.tp.hitInfo2D && Launcher2D._instance.tp.hitInfo2D.collider.gameObject != null) //  if is a second target
            //{
            //    CThrowController._instance.AddTarget(Launcher2D._instance.tp.hitInfo2D.collider.gameObject);
            //}
        }
        else if (_state == PlayerState.WAITING)
        {

        }
    }
}
