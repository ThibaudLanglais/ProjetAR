using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerProjectile : MonoBehaviour
{
    private float damage = 5f;
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void OnCollisionEnter(Collision other) {
        //Si la balle touche un ennemi, on lui inflige des dégâts
        if(other.transform.tag == "Enemy"){
            EnemyBehavior script = other.transform.GetComponent<EnemyBehavior>();
            List<AudioClip> HitClips = script.GetHitClips();
            if(HitClips.Count > 0){
                //On jour un son de collision aléatoire parmi la liste
                AudioSource audioSource = other.transform.GetComponent<AudioSource>();
                AudioClip hitClip = HitClips[Mathf.FloorToInt(Random.Range(0, HitClips.Count))];
                audioSource.Stop();
                audioSource.volume = 0.2f;
                audioSource.PlayOneShot(hitClip);
            }
            //On inflige des dégâts, l'objet sera détruit dans tous les cas au bout de 3s
            script.TakeDamage(damage);
        }
    }
}
