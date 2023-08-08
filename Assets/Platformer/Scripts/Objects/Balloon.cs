using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Balloon : MonoBehaviour

{
    private Animator anim;
    private BoxCollider2D bc;
    [SerializeField]private float poppedWaitTime;



    void Awake()
    {
        anim=GetComponent<Animator>();
    }

    public void OnHit()
    {
        bc.enabled=false;
        anim.SetTrigger("Pop");
        StartCoroutine(Popped());
    }

    IEnumerator Popped()
    {
        yield return new WaitForSeconds(poppedWaitTime);
        anim.SetTrigger("IsReturning");
    }
}
