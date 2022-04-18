using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    private Transform mainMenu;
    private Transform levelMenu;
    private Transform levelButtons;
    private Transform OneSleepyPlayer;
    private Sprite awakePlayerSprite;

    private Transform Fade;
    public static Camera cam;

    private Button startButton;
    private Button levelSelectMenuButton;
    private Button backToMenuButton;

    private void Awake() {
        mainMenu = GameObject.Find("MainMenu").transform;
        levelMenu = GameObject.Find("LevelMenu").transform;
        levelButtons = GameObject.Find("LevelButtons").transform;
        OneSleepyPlayer = GameObject.Find("SleepyPlayer").transform;
        Fade = GameObject.Find("Fade").transform;
        awakePlayerSprite = Resources.Load<Sprite>("Sprites/Player/AwakePlayer");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();

        startButton = GameObject.Find("StartButton").GetComponent<Button>();
        levelSelectMenuButton = GameObject.Find("LevelSelectMenuButton").GetComponent<Button>();
        backToMenuButton = GameObject.Find("BackToMenuButton").GetComponent<Button>();

        startButton.onClick.AddListener(() => {
            StartGame();
        });

        levelSelectMenuButton.onClick.AddListener(() => {
            LevelSelectMenu();
        });

        backToMenuButton.onClick.AddListener(() => {
            BackToMainMenu();
        });


        for (int i = 1; i < 13; i++) {
            int x = i;
            levelButtons.GetChild(i-1).GetComponent<Button>().onClick.AddListener(() => {
                if (levelButtons.GetChild(x-1).GetComponent<Image>().color == new Color32(255, 126, 146, 255)) { //if level unlocked
                    StartCoroutine(LevelSelect(x-1));
                }
            });
        }

    }

    private void Start() {
        levelMenu.gameObject.SetActive(false);
        Fade.gameObject.SetActive(false);

        for (int i = 0; i < PlayerPrefs.GetInt("LevelCompleted") + 1; i++) {
            levelButtons.GetChild(i).GetComponent<Image>().sprite = awakePlayerSprite;
            levelButtons.GetChild(i).GetComponent<Image>().color = new Color32(255, 126, 146, 255);
            levelButtons.GetChild(i).Find("sleep").gameObject.SetActive(false);     //Sleep Effect
        }

    }

    public void StartGame() {
        SceneManager.LoadScene(PlayerPrefs.GetInt("LevelCompleted") + 1);
    }

    public void LevelSelectMenu() {
        levelMenu.gameObject.SetActive(true);
        mainMenu.gameObject.SetActive(false);
        OneSleepyPlayer.gameObject.SetActive(false);
    }

    public void BackToMainMenu() {
        levelMenu.gameObject.SetActive(false);
        mainMenu.gameObject.SetActive(true);
        OneSleepyPlayer.gameObject.SetActive(true);
    }

    private IEnumerator LevelSelect(int level) {
        levelButtons.GetChild(level).Find("wake").gameObject.SetActive(true);
        levelButtons.GetChild(level).GetComponent<Animator>().SetTrigger("wake");
        yield return new WaitForSeconds(0.5f);
        Fade.transform.position = levelButtons.GetChild(level).transform.position;
        Fade.gameObject.SetActive(true);
        Fade.GetComponent<Animator>().SetTrigger("FadeOut");
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene(level+1);

    }
}
