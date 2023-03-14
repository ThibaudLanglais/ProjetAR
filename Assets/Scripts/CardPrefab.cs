using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Functions {
    MoreAmmo,
    HigherResistance,
    HigherShootingSpeed,
    RegenerateHP,
    SlowEnemies,
    MoreHP,
};

//Cette classe permet de définir les préfabs qui représentent les différents effets du jeu
//C'est la solution que j'ai trouvé pour pouvoir activer les effets avec la même fonction ("Use")

public class CardPrefab : MonoBehaviour
{
    //Sauvegarde de la texture utilisée pour comparer les noms de la texture détectée par la caméra avec l'effet correspondant
    public Texture2D texture;
    //Fonction à activer lorsque l'on appelle la fonction "Use" dans le script ImageRecognition
    public Functions functionToActivate;

    private GameManager gameManager;

    public void MoreAmmo(){
        FindObjectOfType<GameManager>().UpdatePlayerMaxAmmo(15);
    }
    public void HigherResistance(){
        FindObjectOfType<GameManager>().PlayerResistance = 3f;
    }
    public void HigherShootingSpeed(){
        FindObjectOfType<GameManager>().weaponScript.FireRate = 15f;
    }
    public void RegenerateHP(){
        FindObjectOfType<GameManager>().RegenerateHP();
    }
    public void SlowEnemies(){
        FindObjectOfType<GameManager>().SlowEnemies(0.5f);
    }
    public void MoreHP(){
        FindObjectOfType<GameManager>().UpdatePlayerMaxHP(50);
    }
}
