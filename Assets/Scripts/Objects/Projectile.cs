using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private Rigidbody2D rb;
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        StartCoroutine(DeathTime());
    }
    public void Fire(float speed, Vector3 direction)
    {
        rb.velocity = direction * speed;
        transform.rotation = new Quaternion(direction.x, direction.y, 0f, 0f);
    }
    private void OnCollisionEnter2D(Collision2D col)
    {
        
        if(col.gameObject.GetComponent<PlayerMovement>() != null)
        {
            PlayerMovement movementScript = col.gameObject.GetComponent<PlayerMovement>();
            movementScript.Death();
        }
        Destroy(gameObject);
    }
    IEnumerator DeathTime()
    {
        yield return new WaitForSeconds(25);
        Destroy(gameObject);
    }
}
