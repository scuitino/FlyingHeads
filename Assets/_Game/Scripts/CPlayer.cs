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
        WALKING,
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
            _playerRB.drag = 0;
            _headSprite.SetActive(true);
        }
        else if (_state == PlayerState.WALKING)
        {
            _playerRB.drag = 6;
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
            CThrowController._instance.RemoveTarget(Launcher2D._instance.tp.hitInfo2D.collider.gameObject);
        }
        else if (_state == PlayerState.SPAWNING)
        {
            _spawnParticle.SetActive(true);
            SetState(PlayerState.IDLE);
        }        
    }

    void StatesUpdate()
    {
        if (_state == PlayerState.IDLE)
        {
            if (_playerRB.velocity.y != 0)
            {
                SetState(PlayerState.FALLING);
            } else if (_playerRB.velocity.x != 0)
            {
                SetState(PlayerState.WALKING);
            }            
        }
        else if (_state == PlayerState.WALKING)
        {
            if (_playerRB.velocity.y != 0)
            {
                SetState(PlayerState.FALLING);
            }
            else if (_playerRB.velocity.x == 0)
            {
                SetState(PlayerState.IDLE);
            }            
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

    // false = left, true = right
    public void MovePlayer(bool aSide)
    {
        if (aSide) // move right
        {
            _playerRB.AddForce(new Vector2(_moveSpeed, 0));
        }
        else // move left
        {
            _playerRB.AddForce(new Vector2(-_moveSpeed, 0));
        }
    }
}
