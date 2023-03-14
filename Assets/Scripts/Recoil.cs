using UnityEngine;
using System.Collections;


//C'est un script trouvé sur internet je suis pas sûr de savoir l'expliquer, ça applique un effet de recul
public class Recoil : MonoBehaviour
{
    private float recoil = 0.0f;
    private float maxRecoil_x = -20f;
    private float maxRecoil_y = 20f;
    private float recoilSpeed = 2f;

    private Vector3 initialPosition, maxPosRecoil;

    private void Start() {
        initialPosition = transform.position;
        maxPosRecoil = transform.localPosition + new Vector3(0,0,-0.2f);
    }
 
    public void StartRecoil (float recoilParam, float maxRecoil_xParam, float recoilSpeedParam)
    {
        // in seconds
        recoil = recoilParam;
        maxRecoil_x = maxRecoil_xParam;
        recoilSpeed = recoilSpeedParam;
        maxRecoil_y = Random.Range(-maxRecoil_xParam, maxRecoil_xParam);
    }
 
    void recoiling ()
    {
        if (recoil > 0f) {
            Quaternion maxRecoil = Quaternion.Euler (maxRecoil_x, maxRecoil_y, 0f);
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp (transform.localRotation, maxRecoil, Time.deltaTime * recoilSpeed);
            transform.localPosition = Vector3.Slerp(transform.localPosition, maxPosRecoil, Time.deltaTime * recoilSpeed);
            recoil -= Time.deltaTime;
        } else {
            recoil = 0f;
            // Dampen towards the target rotation
            transform.localRotation = Quaternion.Slerp (transform.localRotation, Quaternion.identity, Time.deltaTime * recoilSpeed / 2);
            transform.localPosition = Vector3.Slerp(transform.localPosition, initialPosition, Time.deltaTime * recoilSpeed / 2);
        }
    }

    // Update is called once per frame
    void Update ()
    {
        recoiling();
    }
}