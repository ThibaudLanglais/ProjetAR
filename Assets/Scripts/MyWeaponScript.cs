using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MyWeaponScript : MonoBehaviour
{
   private Camera cam;
   public GameObject projectile;
   public Transform projectilePosition;
   private int ammoForce = 10;
   public float FireRate = 10f;  // The number of bullets fired per second
   private float lastfired;      // The value of Time.time at the last firing moment
   public int maxAmmo = 30;
   public int currentAmmo;
   private Recoil recoilScript;
   private bool isReloading = false;
   private GameManager gameManager;
   public Animator animator;

   private AudioSource audioSource;
   public AudioClip shootClip;
   public AudioClip reloadClip;

   void Start()
   {
      if (!projectile) return;
      gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
      if (!gameManager) return;

      currentAmmo = maxAmmo;
      gameManager.UpdateMaxAmmo(maxAmmo);
      gameManager.UpdateCurrentAmmo(maxAmmo);

      //Cette portion de code permet d'orienter l'arme vers le centre de l'écran
      cam = Camera.main;
      Vector3 screenPos = new Vector3(Screen.width / 2.0f, Screen.height / 2.0f, 100);
      Vector3 worldPos = cam.ScreenToWorldPoint(screenPos);
      transform.LookAt(worldPos);
      recoilScript = gameObject.GetComponent<Recoil>();

      audioSource = GetComponent<AudioSource>();
   }

   private void OnEnable() {
      isReloading = false;
   }

   // Update is called once per frame
   void Update()
   {
      TryShoot();
   }

   void TryShoot()
   {
      if(isReloading) return;

      if (currentAmmo == 0){
         StartCoroutine(Reload());
         return;
      }

      //On ne peut pas tirer en pause, il faut appuyer sur l'écran, et que le délai de tir soit écoulé
      if (!gameManager.isPaused && Input.GetButton("Fire1") && (Time.time - lastfired > 1f / FireRate))
      {
         gameManager.UpdateHealthBar();
         currentAmmo--;
         //Updating HUD
         gameManager.UpdateCurrentAmmo(currentAmmo);
         //Firing bullet
         lastfired = Time.time;
         //On instancie une balle, elle gère ses détections elle-même
         GameObject projectileInstance = Instantiate(projectile, projectilePosition.position, Quaternion.identity, transform);
         projectileInstance.transform.localRotation = Quaternion.identity; //Add this line to reset rotation to 0, otherwise it somehow doesn't work
         projectileInstance.GetComponent<Rigidbody>().AddForce(projectileInstance.transform.forward * ammoForce, ForceMode.Impulse);
         Destroy(projectileInstance, 3);
         //On applique un effet de recul sur l'arme
         recoilScript.StartRecoil(0.05f, -3f, 10f);
         //On joue un son de tir
         PlayShoot();
      }
   }

   void PlayShoot()
   {
      if(audioSource && shootClip) audioSource.PlayOneShot(shootClip);
   }

   void PlayReload()
   {
      if(audioSource && reloadClip) audioSource.PlayOneShot(reloadClip);
   }

   IEnumerator Reload()
   {
      //On joue l'animation de reloading, on attend qu'elle finisse et on termine la coroutine
      isReloading = true;
      animator.SetBool("Reloading", true);
      currentAmmo = maxAmmo;
      PlayReload();
      yield return new WaitForSeconds(2f - 0.25f);
      animator.SetBool("Reloading", false);
      yield return new WaitForSeconds(0.25f);
      isReloading = false;
      gameManager.UpdateCurrentAmmo(currentAmmo);
   }
}
