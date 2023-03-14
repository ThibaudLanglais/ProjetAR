using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurretProjectile : MonoBehaviour
{
    private float damage = 20f;
    public GameManager gameManager;
    private bool hasCollided = false;

    // private void OnCollisionEnter(Collision other) {
    //     if(!hasCollided && other.transform.tag == "Enemy"){
    //         hasCollided = true;
    //         other.collider.gameObject.GetComponentInParent<EnemyBehavior>().TakeDamage(damage);
    //         Destroy(gameObject);
    //     }
    // }
}
