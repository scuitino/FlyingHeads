using Com.LuisPedroFonseca.ProCamera2D;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CPlayer : MonoBehaviour {

    #region SINGLETON PATTERN
    public static CPlayer _instance = null;
    #endregion

    [SerializeField, Header("Gameplay")]
    float _moveSpeed;

    [SerializeField]
    Rigidbody2D _playerRB;

    [SerializeField]
    Vector3 _lastRespawnPosition;

    // layer that destroy the head
    [SerializeField]
    LayerMask _enemyLayer;

    // time before respawn after death
    [SerializeField]
    float _deathRespawnDelay;

    // true when the body is safe
    public bool _bodyIsSafe;

    [SerializeField, Header("ART")]
    GameObject _headSprite;

    [SerializeField]
    GameObject _artObjects;

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
        SPAWNING, //  when successfull spawn
        CHECKPOINT_RESPAWNING // when player and head both dead
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

    private void Start()
    {
        _lastRespawnPosition = this.transform.position;
    }

    private void Update()
    {
        StatesUpdate();
    }

    // when the player touch something
    private void OnCollisionEnter2D(Collision2D collision)
    {        
        if ((_enemyLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer) // if the player touch an enemy
        {
            Die(); // die motherfucker!!
        }
    }

    // change player state
    public void SetState(PlayerState aState)
    {
        _state = aState;
        if (_state == PlayerState.IDLE)
        {
            _bodyIsSafe = true;

            // enable pan / disable camera targeting mode 
            if (CThrowController._instance.GetCameraState() != CThrowController.CameraState.PANNING)
            {
                CThrowController._instance.SetCameraState(CThrowController.CameraState.PANNING);
            }

            // return camera to player position
            CThrowController._instance.ReturnCamera();

            // enable throw mode
            // CThrowController._instance._longPressGesture.MinimumDurationSeconds = 0f;
            _playerRB.drag = 0;
            _playerRB.simulated = true;
            _headSprite.SetActive(true);
            _artObjects.SetActive(true);
            
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
            _artObjects.SetActive(true);
            this.GetComponent<AudioSource>().Play();

            // configure the camera when spawn
            CThrowController._instance._secondCameraTarget.transform.position = this.transform.position;
            CThrowController._instance._proCamera.CameraTargets[0].TargetTransform = this.transform;
            CThrowController._instance._proCamera.CameraTargets[0].TargetOffset = new Vector2(0, 3.52f);

            SetState(PlayerState.IDLE);
        }
        else if (_state == PlayerState.CHECKPOINT_RESPAWNING)
        {
            // respawn on checkpoint
            transform.position = _lastRespawnPosition;

            // death effects                       
            _noHeadParticle.SetActive(false);
            _spawnParticle.SetActive(true);
            _artObjects.SetActive(true);
            _shotParticle.SetActive(false);
            this.GetComponent<AudioSource>().Play();

            // configure the camera when spawn
            //CThrowController._instance._proCamera.CameraTargets[0].TargetTransform = this.transform;
            CThrowController._instance._secondCameraTarget.transform.position = this.transform.position;

            SetState(PlayerState.IDLE);
        }
    }

    // effects when player death
    public void PlayPlayerDeathParticles()
    {
        Instantiate(_deathParticlePrefab, transform.position, Quaternion.identity);
    }

    // when the player body die!
    public void Die()
    {
        _bodyIsSafe = false;
        _playerRB.simulated = false;
        PlayPlayerDeathParticles();
        _artObjects.SetActive(false);

        // there is no active head
        if (CThrowController._instance._activeHead == null)
        {
            StartCoroutine(CheckpointNoHeadRespawn());
        }
    }

    // when player die but is not active head
    IEnumerator CheckpointNoHeadRespawn()
    {
        //CThrowController._instance.SetCameraState(CThrowController.CameraState.PANNING);
        yield return new WaitForSeconds(_deathRespawnDelay);
        CPlayer._instance.SetState(CPlayer.PlayerState.CHECKPOINT_RESPAWNING);        
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
