using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ce script gère plus ou moins tout ce qui touche aux ennemis
//Leurs sons, leurs projectiles, les tirs
public class EnemyBehavior : MonoBehaviour
{
    public Transform shootingPoint;
    public GameObject projectile;
    private GameManager gameManager;
    private float FireDelay = 1f; //Number of seconds to wait before firing
    private float lastFired;
    private float ammoForce = 10f;
    private float MaxEnemyHP = 40f;
    private float EnemyHP;
    private bool isDying = false;
    private AudioSource audioSource;

    public List<AudioClip> SpawnClips;
    public List<AudioClip> DeathClips;
    public List<AudioClip> HitClips;
    public List<AudioClip> TauntClips;


    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        EnemyHP = MaxEnemyHP;
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        //Si l'ennemi meurt, on fait rien de spécial
        //Sinon, on vérifie ses points de vie. S'il n'en as plus, on lui demande gentiment de décéder, 
        //s'il en a, il peut continuer ses activités de robot tueur.
        if(isDying) return;

        if(EnemyHP <= 0){
            isDying = true;
            StartCoroutine(Die());
        }else{
            TryShoot();
        }
    }

    public void TakeDamage(float value){
        EnemyHP = EnemyHP - value < 0 ? 0 : EnemyHP - value;
    }

    void TryShoot(){
        //Don't shoot if the player is dead
        if(gameManager.GetPlayerHP() <= 0) return;

        //Don't shoot if the game is paused 
        if(gameManager.isPaused) return;

        //Si le délai depuis le dernier tir est écoulé, on tire à nouveau, pas avec un raycast mais avec un gameobject et collider directement
        if(Time.time - lastFired > FireDelay){
            lastFired = Time.time;
            FireDelay = Random.Range(1f, 3f);
            GameObject projectileInstance = Instantiate(projectile, shootingPoint.position, Quaternion.identity, transform);
            projectileInstance.transform.localRotation = Quaternion.identity; //Add this line to reset rotation to 0, otherwise it somehow doesn't work
            projectileInstance.GetComponent<EnemyProjectile>().gameManager = gameManager;
            projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * ammoForce, ForceMode.Impulse);
            Destroy(projectileInstance, 3);
        }
    }
    public List<AudioClip> GetSpawnClips(){
        return SpawnClips;
    }
    
    public List<AudioClip> GetHitClips(){
        return HitClips;
    }

    IEnumerator Die(){
        //Si on a un son de mort, on le joue
        if(DeathClips.Count > 0){
            AudioClip deathClip = DeathClips[Mathf.FloorToInt(Random.Range(0, DeathClips.Count))];
            audioSource.Stop();
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(deathClip);
            yield return new WaitForSeconds(deathClip.length);
        }
        //Sinon on remercie simplement l'ennemi pour ses services et le laissons partir (on le détruit)
        gameManager.enemiesCount--;
        Destroy(gameObject);
    }
}
