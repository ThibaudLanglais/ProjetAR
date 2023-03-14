using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using PathCreation.Examples;


//Ce script spawn des ennemis sur la surface d'une sphere invisible qui suit le joueur
public class SpawnEnemies : MonoBehaviour
{
    public SphereCollider spawnSphere;
    public GameObject enemy;
    public GameManager gameManager;
    public Transform XROrigin;
    //How many we want to spawn per second
    private float spawningRate = 1f;
    private float lastSpawned = 0f;
    private int countSpawned = 0;
    public float EnemyMovingSpeed = 1f;
    private ConstraintSource targetLookAt;

    // Start is called before the first frame update
    void Start()
    {
        if(!spawnSphere) throw new System.Exception("No sphere collided provided");
        else {
            //On définit une contrainte pour toujours regarder le joueur qui sera donnée aux ennemis qui spawnent
            targetLookAt = new ConstraintSource();
            targetLookAt.sourceTransform = spawnSphere.transform;
            targetLookAt.weight = 1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(!spawnSphere || !enemy || gameManager.GetPlayerHP() <= 0 || gameManager.enemiesCount >= 10) return;

        //Spawn according to spawningRate
        if(Time.time - lastSpawned > 1f / spawningRate){
            lastSpawned = Time.time;
            countSpawned++;
            gameManager.enemiesCount++;

            
            //On spawn un ennemi sur un point aléatoire sur la surface de la sphère qui entoure le joueur
            GameObject instance = Instantiate(enemy, (Random.onUnitSphere * spawnSphere.radius + spawnSphere.transform.position), Quaternion.identity, XROrigin);
            instance.GetComponentInChildren<LookAtConstraint>().AddSource(targetLookAt);
            instance.GetComponentInChildren<PathFollower>().speed = EnemyMovingSpeed;
            
            //On joue un son pour signaler de l'apparition de l'ennemi
            EnemyBehavior script = instance.GetComponentInChildren<EnemyBehavior>();
            AudioSource audioSource = script.GetComponent<AudioSource>();
            List<AudioClip> clips = script.GetSpawnClips();
            audioSource.volume = 0.5f;
            audioSource.PlayOneShot(clips[Mathf.FloorToInt(Random.Range(0, clips.Count))]);
        }
    }
}
