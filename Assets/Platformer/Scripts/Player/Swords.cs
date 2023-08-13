using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swords : MonoBehaviour
{
    public InputAction swordControls;
    [Header("Levels")]
    [Range(1, 4)]
    public int swordLevel;
    [SerializeField]private float level1Hits;
    [SerializeField]private float level2Hits;
    [SerializeField]private float level3Hits;
    private float amountOfHits;

    [Header("GFX")]
    [SerializeField]private Sprite level1Sprite;
    [SerializeField]private Sprite level2Sprite;
    [SerializeField]private Sprite level3Sprite;
    [SerializeField]private Sprite level4Sprite;

    [Header("Components")]
    private SpriteRenderer sr;
    private PlayerMovement pm;

    private float vectorX;
    private float vectorY;
    private float angleX;
    private float radiansX;
    private float angleY;
    private float radiansY;
    private float bouncePower;
    void Awake()
    {
        pm = FindObjectOfType<PlayerMovement>();
        sr = GetComponentInChildren<SpriteRenderer>();
        if(swordLevel == 1)
        {
            sr.sprite = level1Sprite;
            amountOfHits = level1Hits;
        }else if(swordLevel == 2){
            sr.sprite = level2Sprite;
            amountOfHits = level2Hits;
        }else if(swordLevel == 3){
            sr.sprite = level3Sprite;
            amountOfHits = level3Hits;
        }else if(swordLevel == 4){
            sr.sprite = level4Sprite;
            amountOfHits = Mathf.Infinity;
        }
    }
    void LateUpdate()
    {
        vectorX = Mathf.Round(swordControls.ReadValue<Vector2>().x);
        vectorY = Mathf.Round(swordControls.ReadValue<Vector2>().y);
       
        
        //radiansX = Mathf.Acos(vectorX);
        //angleX = radiansX * Mathf.Rad2Deg;
        //radiansY = Mathf.Asin(vectorY);
        //angleY = radiansX * Mathf.Rad2Deg;
        //if(vectorY < 0)
        //{
        //    angleY = angleY * -1;
        //}
        //if(vectorX < 0)
        //{
        //    angleY = angleY * -1;
        //}

        //transform.rotation = Quaternion.Euler(0, angleX, angleY );
        //Debug.Log(vectorY);
        if(vectorX > 0)
        {
            if(vectorY > 0)
            {
                transform.rotation = new Quaternion(0f, 135f, 0f, 0f);
            }else if(vectorY < 0){
                transform.rotation = new Quaternion(0f, 45f, 0f, 0f);
            }else{
                transform.rotation = new Quaternion(0f, 90f, 0f, 0f);
            }
        }else if(vectorX < 0)
        {
            if(vectorY > 0)
            {
                transform.rotation = new Quaternion(0f, -135f, 0f, 0f);
            }else if(vectorY < 0){
                transform.rotation = new Quaternion(0f, -45f, 0f, 0f);
            }else{
                transform.rotation = new Quaternion(0f, -90f, 0f, 0f);
            }
        }else{
             if(vectorY > 0)
            {
                transform.rotation = new Quaternion(0f, 180f, 0f, 0f);
            }else if(vectorY < 0){
                transform.rotation = new Quaternion(0f, -180f, 0f, 0f);
            }else{
                transform.rotation = new Quaternion(0f, -180f, 0f, 0f);
            }
            Debug.Log(vectorX);
        }
    }
}
