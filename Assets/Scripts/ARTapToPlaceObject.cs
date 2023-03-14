using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.ARFoundation;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.UIElements;

[RequireComponent(typeof(ARRaycastManager))]

//Globalement le script que vous nous aviez donné en TP
//Ce script permet de placer une tourelle dans l'espace selon les points scannés par la caméra
public class ARTapToPlaceObject : MonoBehaviour
{
    public GameObject turretPrefab;
    private GameObject spawnedObject;
    private ARRaycastManager _arRaycastManager;
    private Vector2 touchPosition;
    static List<ARRaycastHit> hits = new List<ARRaycastHit>();
    public UIDocument PlaceTurretHud;
    public GameManager gameManager;
    private void Awake()
    {
        _arRaycastManager = GetComponent<ARRaycastManager>();
    }
    bool TryGetTouchPosition(out Vector2 touchPosition)
    {
        if (Input.touchCount > 0)
        {
            touchPosition = Input.GetTouch(index: 0).position;
            return true;
        }
        touchPosition = default;
        return false;
    }
    void Update()
    {
        if (!TryGetTouchPosition(out Vector2 touchPosition)) return;
        if (_arRaycastManager.Raycast(touchPosition, hits,
        trackableTypes: TrackableType.FeaturePoint))
        {
            var hitPose = hits[0].pose;
            if (spawnedObject == null)
            {
                //Si la tourelle n'a pas encore été placée, on l'instancie
                spawnedObject = Instantiate(turretPrefab, hitPose.position, hitPose.rotation);
                spawnedObject.SetActive(true);

                //On signale au script qui gère l'état du jeu que la tourelle est placée
                gameManager.isTurretPlaced = true;
                //Et on change la couleur du bouton pour signaler à l'utilisateur du changement
                PlaceTurretHud.rootVisualElement.Q<Button>("Start").style.backgroundColor = new Color(255/256f,127/256f,0/256f,1f);
            }
            else
            {
                //Si la tourelle avait déjà été placée, on la déplace au nouvel endroit
                spawnedObject.transform.position = hitPose.position;
                spawnedObject.transform.rotation = hitPose.rotation;
            }
        }
    }
}
