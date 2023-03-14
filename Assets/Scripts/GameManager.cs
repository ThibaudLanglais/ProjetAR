using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManager : MonoBehaviour
{
    public UIDocument Hud;
    public float MaxPlayerHP = 500f;
    public bool isPaused = false;
    private float PlayerHP;
    public MenuController menuController;
    public MyWeaponScript weaponScript;
    public SpawnEnemies Spawner;
    public float PlayerResistance = 1f;
    private Color GreenColor = new Color(84f / 256f, 180f / 256f, 53f / 256f, 1f);
    private Color RedColor = new Color(199f / 256f, 0f / 256f, 57f / 256f, 1f);
    public List<CardPrefab> selectedCards;
    public int enemiesCount = 0;
    public bool isTurretPlaced = false;

    private void Awake() {
        PlayerHP = MaxPlayerHP;
        //Le CardsController est un singleton dont la propriété selectedCards est initalisée pendant l'écran de tirage aléatoire des cartes
        //L'objet a persisté entre les deux scènes.
        foreach (GameObject item in CardsController.instance.selectedCards)
        {   
            selectedCards.Add(item.GetComponent<CardPrefab>());
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        isPaused = false;
        //Mise à jour de la barre de vie pour l'initialiser à 100%
        UpdateHealthBar();
    }

    //Permet de mettre à jour l'élement de l'UI qui affiche le nombre de balles dans le chargeur
    public void UpdateCurrentAmmo(int quantity)
    {
        if(!Hud) return;
        Label currentAmmo = Hud.rootVisualElement.Q<Label>("CurrentAmmo");
        if(currentAmmo == null) return;
        currentAmmo.text = quantity.ToString();
    }

    //Permet de mettre à jour l'élement de l'UI qui affiche le nombre de balles maximum dans le chargeur
    public void UpdateMaxAmmo(int quantity)
    {
        if(!Hud) return;
        Label maximumAmmo = Hud.rootVisualElement.Q<Label>("MaximumAmmo");
        if(maximumAmmo == null) return;
        maximumAmmo.text = "/" + quantity.ToString();
    }

    //Met à jour les points de vie du joueur suivant les dégâts reçus, en prenant en compte sa résistance, puis met à jour l'UI
    public void UpdatePlayerHP(float damage){
        PlayerHP = PlayerHP - damage/PlayerResistance < 0 ? 0 : PlayerHP - damage/PlayerResistance;
        UpdateHealthBar();
    }


    //met à jour l'UI
    public void UpdateHealthBar()
    {
        if(!Hud) return;
        VisualElement bar = Hud.rootVisualElement.Q<VisualElement>("HealthBar");
        VisualElement fill = bar.Q<VisualElement>("Fill");
        
        if(fill == null) throw new System.Exception();
        
        fill.style.backgroundColor = Color.Lerp(RedColor, GreenColor, PlayerHP / MaxPlayerHP);
        fill.style.width = new StyleLength(new Length((100 * PlayerHP / MaxPlayerHP), LengthUnit.Percent));
        bar.Q<Label>("Text").text = Mathf.RoundToInt(PlayerHP).ToString() + " HP";
    }

    public float GetPlayerHP()
    {
        return PlayerHP;
    }

    //Remet les points de vie au max
    public void RegenerateHP()
    {
        PlayerHP = MaxPlayerHP;
        UpdateHealthBar();
    }

    //Augmente les points de vie max du joueur
    public void UpdatePlayerMaxHP(float value){
        MaxPlayerHP += value;
        PlayerHP += value;
        UpdateHealthBar();
    }

    //Augmente le nombre maximum de balles dans le chargeur du joueur
    public void UpdatePlayerMaxAmmo(int value){
        weaponScript.maxAmmo += value;
        weaponScript.currentAmmo += value;
        UpdateMaxAmmo(weaponScript.maxAmmo);
        UpdateCurrentAmmo(weaponScript.currentAmmo);
    }

    //Ralentit la vitesse de déplacement des ennemis
    public void SlowEnemies(float newSpeed){
        Spawner.EnemyMovingSpeed = newSpeed;
    }
}
