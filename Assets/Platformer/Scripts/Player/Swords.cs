using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Swords : MonoBehaviour
{
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

    private Vector3 playerMovement;
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
    void Update()
    {
        playerMovement = pm.playerControls.ReadValue<Vector2>();
        Quaternion rotation = Quaternion.LookRotation(transform.position + playerMovement, Vector3.right);
    }
}
