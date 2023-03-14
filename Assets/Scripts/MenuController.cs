using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;
using UnityEngine.XR.ARFoundation;

public class MenuController : MonoBehaviour
{
    public UIDocument GameHud, PauseHud, PlaceTurretHud;
    public Volume volume;
    public GameManager gameManager;
    public ARPointCloudManager pointCloudManager;

    // Start is called before the first frame update
    void Start()
    {
        //La fonction commence par désactiver les UI du menu pause et du jeu pour n'afficher que celui du placement de la tourelle
        if(!GameHud || !PauseHud || !PlaceTurretHud) return;
        GameHud.rootVisualElement.style.display = DisplayStyle.None;
        PauseHud.rootVisualElement.style.display = DisplayStyle.None;

        //On écoute les clics sur le bouton démarrer de l'écran de placement de la tourelle
        PlaceTurretHud.rootVisualElement.Q<Button>("Start").RegisterCallback<ClickEvent>(ev => StartGame());
    }
    void Destroy(){
        //Quand le script est déchargé, on supprime tous les écouteurs d'événements pour éviter les memory leaks

        if(!GameHud || !PauseHud || !PlaceTurretHud) return;
        
        GameHud.rootVisualElement.Q<Button>("PauseButton").UnregisterCallback<ClickEvent>(ev => ChangePauseState(true));
        PauseHud.rootVisualElement.Q<Button>("PlayButton").UnregisterCallback<ClickEvent>(ev => ChangePauseState(false));
        GameHud.rootVisualElement.Q<Button>("VolumeON").UnregisterCallback<ClickEvent>(ev => SetVolumeOff());
        GameHud.rootVisualElement.Q<Button>("VolumeOFF").UnregisterCallback<ClickEvent>(ev => SetVolumeOn());
        PlaceTurretHud.rootVisualElement.Q<Button>("Start").UnregisterCallback<ClickEvent>(ev => StartGame());
    }

    public void ChangePauseState(bool isPaused)
    {
        //Quand l'état du jeu change on met en pause ou non selon le booléen
        gameManager.isPaused = isPaused;
        
        if(isPaused) Time.timeScale = 0f;
        else Time.timeScale = 1f;

        //Selon si l'écran est en pause, on baisse la saturation (on met en noir et blanc) pour le montrer
        ColorAdjustments colorAdjustments;
        if(!volume.profile.TryGet<ColorAdjustments>(out colorAdjustments)) throw new System.Exception();
        colorAdjustments.saturation.Override(isPaused ? -100 : 100);
        //On cache l'UI du jeu, et on affiche l'UI du menu pause
        GameHud.rootVisualElement.style.display = isPaused ? DisplayStyle.None : DisplayStyle.Flex;
        PauseHud.rootVisualElement.style.display = isPaused ? DisplayStyle.Flex : DisplayStyle.None;
    }

    //Les deux fonctions suivantes permettent d'activer/désactiver le son du jeu
    //elles changent l'icône affichée suivant si le son est ON ou OFF
    void SetVolumeOn()
    {
        GameHud.rootVisualElement.Q<Button>("VolumeON").style.display = DisplayStyle.Flex;
        GameHud.rootVisualElement.Q<Button>("VolumeOFF").style.display = DisplayStyle.None;
        AudioListener.volume = 1;
    }
    void SetVolumeOff()
    {
        GameHud.rootVisualElement.Q<Button>("VolumeON").style.display = DisplayStyle.None;
        GameHud.rootVisualElement.Q<Button>("VolumeOFF").style.display = DisplayStyle.Flex;
        AudioListener.volume = 0;
    }

    //Permet de déclencher le début du jeu (spawn des ennemis)
    void StartGame(){
        //Si la tourelle n'est pas placée on ne fait rien
        if(!gameManager.isTurretPlaced) return;

        //On active l'UI du jeu
        GameHud.gameObject.SetActive(true);
        //On active l'UI du menu pause
        PauseHud.gameObject.SetActive(true);
        //On désactive l'UI du placement de tourelle
        PlaceTurretHud.gameObject.SetActive(false);

        //Écoute des événements de click sur les différents boutons
        GameHud.rootVisualElement.Q<Button>("PauseButton").RegisterCallback<ClickEvent>(ev => ChangePauseState(true));
        PauseHud.rootVisualElement.Q<Button>("PlayButton").RegisterCallback<ClickEvent>(ev => ChangePauseState(false));
        GameHud.rootVisualElement.Q<Button>("VolumeON").RegisterCallback<ClickEvent>(ev => SetVolumeOff());
        GameHud.rootVisualElement.Q<Button>("VolumeOFF").RegisterCallback<ClickEvent>(ev => SetVolumeOn());

        ChangePauseState(false);
        gameManager.Spawner.enabled = true;
        pointCloudManager.enabled = false;
        pointCloudManager.GetComponent<ARTapToPlaceObject>().enabled = false;
        foreach (var item in pointCloudManager.trackables)
        {
            //On supprime tous les points du point cloud
            item.gameObject.SetActive(false);
        }
    }
}
