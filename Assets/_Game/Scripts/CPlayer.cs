using Com.LuisPedroFonseca.ProCamera2D;
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

    [SerializeField, Header("Particles")]
    GameObject _spawnParticle;

    [SerializeField]
    GameObject _shotParticle;

    [SerializeField]
    GameObject _noHeadParticle;

    [SerializeField]
    GameObject _deathParticlePrefab;

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
            // enable pan / disable camera targeting mode 
            //CThrowController._instance.ChangeCameraMode(false);
            CThrowController._instance.CameraSetState(CThrowController.CameraState.PANNING);

            // enable throw mode
           // CThrowController._instance._longPressGesture.MinimumDurationSeconds = 0f;
            _playerRB.drag = 0;
            _headSprite.SetActive(true);           
        }
        else if (_state == PlayerState.FALLING)
        {
            _playerRB.drag = 0;
        }
        else if (_state == PlayerState.TARGETING)
        {
            
        }
        else if (_state == PlayerState.WAITING)
        {
            _headSprite.SetActive(false);
            _shotParticle.SetActive(true);
            _noHeadParticle.SetActive(true);
        }
        else if (_state == PlayerState.SPAWNING)
        {
            // death effects            
            _noHeadParticle.SetActive(false);
            _spawnParticle.SetActive(true);
            this.GetComponent<AudioSource>().Play();

            // configure the camera when spawn
            CThrowController._instance._secondCameraTarget.transform.position = this.transform.position;
            CThrowController._instance._proCamera.CameraTargets[0].TargetTransform = this.transform;
            CThrowController._instance._proCamera.CameraTargets[0].TargetOffset = new Vector2(0, 3.52f);
            
            SetState(PlayerState.IDLE);            
        }        
    }

    // effects when player death
    public void PlayPlayerDeathParticles()
    {
        Instantiate(_deathParticlePrefab, transform.position, Quaternion.identity);
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
