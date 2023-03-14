using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEngine.SceneManagement;

//Cette classe gère le tirage aléatoire des cartes que le joueur aura le droit d'utiliser pendant sa partie
public class CardsController : MonoBehaviour
{

    public static CardsController instance;
    public UIDocument cardsHud;
    public GameObject[] playableCards;
    public AudioClip[] slotClips;
    public AudioClip winClip;
    private int numberOfCardsDrawed = 3;
    public List<GameObject> selectedCards = new List<GameObject>();
    private AudioSource audioSource;

    private void Awake() {
        instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        StartCoroutine(AnimateCards());
    }

    void Destroy(){
        cardsHud.rootVisualElement.Q<Button>("PlayGame")?.UnregisterCallback<ClickEvent>(ev => StartGame());
    }

    IEnumerator AnimateCards()
    {
        VisualElement randomContainer = cardsHud.rootVisualElement.Q<VisualElement>("RandomContainer");
        List<IMGUIContainer> selectedCardsContainers = cardsHud.rootVisualElement.Query<IMGUIContainer>().ToList();
        Button playButton = cardsHud.rootVisualElement.Q<Button>("PlayGame");

        //On sélectionne autant de cartes que l'on veut (ici, 3)
        for (int i = 0; i < numberOfCardsDrawed; i++)
        {
            //La liste des cartes "playableCards" dans laquelle le programme va piocher aléatoirement est une liste des préfabs qui contiennent le script CardPrefab
            int index = Mathf.RoundToInt(Random.Range(0, playableCards.Length));
            GameObject pickedCard = playableCards[index];

            //On s'assure que la carte n'a pas déjà été piochée
            while(selectedCards.IndexOf(pickedCard) != -1){
                index = Mathf.RoundToInt(Random.Range(0, playableCards.Length));
                pickedCard = playableCards[index];
            }
            selectedCards.Add(pickedCard);

            //Ce int length permet juste de boucler plus de fois que nécessaire pour faire une animation de tirage aléatoire
            //avec les cartes qui défilent les unes après les autres.
            int length = playableCards.Length * 2 + index + 1;

            for (int j = 0; j < length; j++)
            {
                //On parcourt chaque carte parmi les cartes disponibles, à chaque fois on change l'affichage, 
                //en jouant un son de casino, jusqu'à ralentir et finalement afficher la carte tirée avec un son différent
                float easeOutTiming = Mathf.Lerp(0f, 1.5f, (j+5-length)/(float)length) * 3;
                randomContainer.style.backgroundImage = new StyleBackground(playableCards[j%playableCards.Length].GetComponent<CardPrefab>().texture);
                audioSource.PlayOneShot(j < length-3 ? slotClips[Mathf.FloorToInt(Random.Range(0, slotClips.Length))] : winClip);
                yield return new WaitForSeconds(0.08f + easeOutTiming);
            }
            selectedCardsContainers[i].style.backgroundImage = new StyleBackground(playableCards[index].GetComponent<CardPrefab>().texture);
            
            //Après 2 secondes de pause, on tire la prochaine carte
            yield return new WaitForSeconds(2f);
        }

        //Quand toutes les cartes sont tirées, la couleur du bouton change et le joueur peut commencer la partie
        playButton.style.backgroundColor = new Color(255/256f,127/256f,0/256f,1f);
        playButton.focusable = true;
        playButton.RegisterCallback<ClickEvent>(ev => StartGame());
    }

    void StartGame(){
        SceneManager.LoadScene("GameScene");
    }

}
