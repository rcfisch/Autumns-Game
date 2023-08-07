using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldManagement : MonoBehaviour
{
    void Update()
    {
        GameObject[] gameObjects = FindObjectsOfType<GameObject>();

        foreach(GameObject gameObject in gameObjects)
        {
            if(gameObject.tag == "Player")
            {
                if(gameObject.transform.position.y <= -10)
                {
                    gameObject.GetComponent<PlayerMovement>().Death();
                }
            }
        }
    }
}
