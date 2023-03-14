using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.ARFoundation;

public class ImageRecognition : MonoBehaviour
{
   public List<EffectCube> Cubes;

   private ARTrackedImageManager _aRTrackedImageManager; // Reference au s c r i p t de gestion
   private void Awake()
   {
      _aRTrackedImageManager = GetComponent<ARTrackedImageManager>();
   }
   private void OnEnable()
   {
      _aRTrackedImageManager.trackedImagesChanged += OnImageChanged;
   }
   public void OnDisable()
   {
      _aRTrackedImageManager.trackedImagesChanged -= OnImageChanged;
   }

   public void OnImageChanged(ARTrackedImagesChangedEventArgs args)
   {
      foreach (ARTrackedImage img in args.added)
      {
         //Pour chaque image détectée, on regarde si elle correspond à un des effets activable
         foreach (EffectCube cube in Cubes){
            //Pour savoir si elle correspond, on compare le nom de l'image avec le nom de la texture du cube qui représente l'effet
            if(img.referenceImage.name == cube.gameManager.selectedCards[cube.index].texture.name){
               //Si elle correspond, on active l'effet
               if(!cube.isUsed) cube.Use();
            }
         }
      }
   }

   
}
