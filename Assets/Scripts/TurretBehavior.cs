using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

//Ce script définit le comportement de la tourelle que le joueur pose au début de la partie
public class TurretBehavior : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject projectile;
    private GameManager gameManager;
    private float FireDelay = 1f; //Number of seconds to wait before firing
    private float lastFired;
    private float ammoForce = 100f;

    private EnemyBehavior currentEnemy;

    // Start is called before the first frame update
    void Start()
    {
        //On commence par demander à la tourelle de chercher un ennemi à intervalle régulier
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        StartCoroutine(FindEnemy());
    }

    // Update is called once per frame
    void Update()
    {
        TryShoot();
    }

    void TryShoot(){
        // Don't shoot if the player is dead
        if(gameManager.GetPlayerHP() <= 0) return;

        //Don't shoot if the game is paused 
        if(gameManager.isPaused) return;

        if(gameManager.enemiesCount == 0) return;

        //Don't shoot if no enemy to shoot
        if(currentEnemy == null) currentEnemy = FindObjectOfType<EnemyBehavior>();

        //Si le délai depuis le dernier tir est écoulé, on tire
        if(Time.time - lastFired > FireDelay){
            lastFired = Time.time;
            GameObject projectileInstance = Instantiate(projectile, shootingPoint.position, Quaternion.identity, transform);
            projectileInstance.transform.localRotation = Quaternion.LookRotation(currentEnemy.GetComponentInChildren<ShootingPoint>().transform.position - transform.position);
            projectileInstance.GetComponent<TurretProjectile>().gameManager = gameManager;
            projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * ammoForce, ForceMode.Impulse);
            Destroy(projectileInstance, 3);
            currentEnemy.TakeDamage(20);
        }
    }

    IEnumerator FindEnemy(){
        //Toutes les secondes, la tourelle cherche un nouvel ennemi
        while(currentEnemy == null){
            currentEnemy = FindObjectOfType<EnemyBehavior>();
            yield return new WaitForSeconds(1);
        }
    }
}
