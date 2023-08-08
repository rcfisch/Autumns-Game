using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnTouchKill : MonoBehaviour
{
    private void OnCollisionStay2D(Collision2D col)
    {
        if(col.gameObject.GetComponent<PlayerMovement>() != null)
        {
            col.gameObject.GetComponent<PlayerMovement>().Death();
        }
    }
}
