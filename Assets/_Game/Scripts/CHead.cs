using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CHead : MonoBehaviour {

    // when the player can spawn
    [SerializeField]
    LayerMask _safeFloorLayer;

    private void Update()
    {
        //RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, this.GetComponent<CircleCollider2D>().radius + 0.1f);
        //Debug.DrawRay(transform.position, Vector2.down * (this.GetComponent<CircleCollider2D>().radius + 0.1f), Color.red);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if ((_safeFloorLayer & 1 << collision.gameObject.layer) == 1 << collision.gameObject.layer)
        {
            // Cast a ray straight down.
            RaycastHit2D hit = Physics2D.Raycast(transform.position, -Vector2.up, (this.GetComponent<CircleCollider2D>().radius + 0.1f), _safeFloorLayer);
            Debug.DrawRay(transform.position, Vector2.down * (this.GetComponent<CircleCollider2D>().radius + 0.1f), Color.red);
            if (hit.collider != null)
            {
                Debug.Log("safe floor touched");
                CPlayer._instance.transform.position = this.transform.position;
                Destroy(this.gameObject);
            }                
        }        
    }
}
