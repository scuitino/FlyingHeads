using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHead : MonoBehaviour {

    // when the player can spawn
    [SerializeField]
    LayerMask _safeFloorLayer;

    // press touch remember
    float _pressButtonRemember;

    // the time that the pressed button will be remembered
    [SerializeField]
    float _pressButtonRememberTime;

    // last touch floor remember
    float _lastFloorRemember;

    // the time that the last touched floor will be remembered
    [SerializeField]
    float _lastFloorRememberTime;

    // where spawn the body
    Vector2 _spawnPosition;

    private void Update()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, this.GetComponent<CircleCollider2D>().radius + 0.1f);
        //Debug.DrawRay(transform.position, Vector2.down * (this.GetComponent<CircleCollider2D>().radius + 0.1f), Color.red);

        // update second target position when head is active
        CThrowController._instance._secondCameraTarget.transform.position = this.transform.position;

        // decrese remember timers
        _pressButtonRemember -= Time.deltaTime;
        _lastFloorRemember -= Time.deltaTime;
    }

    // when the head touch something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_safeFloorLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer) // if the head touch a safe floor layer object
        {            
            if (CheckSafeFloor()) // check safe floor
            {
                if (_pressButtonRemember > 0) // if a button was touched before floor contact
                {
                    Respawn();
                }
                else // remember floor
                {
                    _lastFloorRemember = _lastFloorRememberTime;
                }                
            }                
        }        
    }

    // when player touch 
    public void PlayerTouch()
    {
        if (CheckSafeFloor() || _lastFloorRemember > 0) // if is a floor on memory
        {
            Respawn();
        }
        else // remember touch
        {
            _pressButtonRemember = _pressButtonRememberTime;
        }
    }

    // check if is a safe floor under the head
    public bool CheckSafeFloor()
    {
        // Cast a ray straight down.
        RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, (this.GetComponent<CircleCollider2D>().radius + 0.1f), _safeFloorLayer);
        Debug.DrawRay(transform.position, Vector2.down * (this.GetComponent<CircleCollider2D>().radius + 0.1f), Color.red);
        if (hit.collider != null)
        {
            _spawnPosition = this.transform.position;
            return true;            
        }
        else
        {
            return false;
        }        
    }

    // respawn player on head position
    public void Respawn()
    {
        Debug.Log("Respawn!");
        CPlayer._instance.transform.position = _spawnPosition;        
        CPlayer._instance.SetState(CPlayer.PlayerState.SPAWNING);
        Destroy(this.gameObject);
    }      
}
