using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Script gérant les projectiles des ennemis
public class EnemyProjectile : MonoBehaviour
{
    private float damage = 10f;
    public GameManager gameManager;
    private bool hasCollided = false;

    private void OnCollisionEnter(Collision other) {
        //Si la balle touche le joueur, on inflige des dégâts au joueur et on détruit la balle
        if(!hasCollided && other.transform.tag == "MainCamera"){
            hasCollided = true;
            gameManager.UpdatePlayerHP(damage);
            Destroy(gameObject);
        }
    }
}
