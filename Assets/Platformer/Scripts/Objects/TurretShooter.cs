using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretShooter : MonoBehaviour
{
    public GameObject arrowPrefab;

    [SerializeField]private float projectileSpeed;
    [SerializeField]private Vector3 dir;
    [SerializeField]private float waitTime;
    private void Awake()
    {
        StartCoroutine(ShootDelay());
    }

    IEnumerator ShootDelay()
    {
        yield return new WaitForSeconds(waitTime);
        Shoot();
    }   
    private void Shoot()
    {
        Vector3 pos = transform.position;
        Quaternion rotation = transform.rotation; //* new Quaternion(dir.x, dir.y, dir.z, 1f);
        GameObject projectile = Instantiate(arrowPrefab, pos, rotation);
        projectile.GetComponent<Projectile>().Fire(projectileSpeed, dir);
        projectile.transform.SetParent(transform);
        StartCoroutine(ShootDelay());
    }

}
