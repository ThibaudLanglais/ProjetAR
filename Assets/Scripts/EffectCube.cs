using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Ce script permet simplement de faire tourner un cube en 3D quand le booleen isUsed est true
public class EffectCube : MonoBehaviour
{
    // Start is called before the first frame update
    public bool isUsed = false;
    public int index;
    public GameManager gameManager;
    private Renderer renderer;

    private float xSpeed = 20, ySpeed = 20, zSpeed = 20;
    private float xDir, yDir, zDir;

    void Awake(){
        renderer = GetComponent<Renderer>();
    }

    void Start()
    {
        xDir = Random.Range(-1f, 1f) > 0 ? 1 : -1;
        yDir = Random.Range(-1f, 1f) > 0 ? 1 : -1;
        zDir = Random.Range(-1f, 1f) > 0 ? 1 : -1;
        renderer.material.mainTexture = gameManager.selectedCards[index].texture;
    }

    // Update is called once per frame
    void Update()
    {
        if(isUsed && !gameManager.isPaused){
            transform.Rotate(new Vector3(xSpeed * xDir, ySpeed * yDir, zSpeed * zDir) * Time.deltaTime);
        }
    }

    public void Use(){
        isUsed = true;
        renderer.material.color = Color.white;
        CardPrefab card = gameManager.selectedCards[index].GetComponent<CardPrefab>();
        card.Invoke(card.functionToActivate.ToString(), 0);
    }
}
