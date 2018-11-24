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

    public enum PlayerState
    {
        IDLE,
        WALKING,
        TARGETING,
        WAITING
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
        }
        else if (_state == PlayerState.WALKING)
        {
            _playerRB.drag = 6;
        }
        else if (_state == PlayerState.TARGETING)
        {

        }
        else if (_state == PlayerState.WAITING)
        {

        }
    }

    void StatesUpdate()
    {
        if (_state == PlayerState.IDLE)
        {
            if (_playerRB.velocity.x != 0)
            {
                SetState(PlayerState.WALKING);
            }
        }
        else if (_state == PlayerState.WALKING)
        {
            Debug.Log(Mathf.Abs(_playerRB.velocity.x));
            if (_playerRB.velocity.x == 0)
            {
                SetState(PlayerState.IDLE);
            }
        }
        else if (_state == PlayerState.TARGETING)
        {

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
