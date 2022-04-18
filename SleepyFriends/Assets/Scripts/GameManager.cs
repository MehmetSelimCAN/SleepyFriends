using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public static bool canMove = true;
    public static GameObject[] points;
    public static int receivedPoints;
    public static GameObject Fade;
    public static Camera cam;
    public static GameObject parentPlayer;

    private Button undoButton;
    private Button restartButton;
    private Button backToMenuButton;

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        points = GameObject.FindGameObjectsWithTag("Point");
        Fade = GameObject.Find("Fade");
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        parentPlayer = GameObject.Find("ParentPlayer");
        Vector3 screenPosition = cam.WorldToScreenPoint(parentPlayer.transform.position);
        Fade.transform.position = screenPosition;

        undoButton = GameObject.Find("UndoButton").GetComponent<Button>();
        restartButton = GameObject.Find("RestartButton").GetComponent<Button>();
        backToMenuButton = GameObject.Find("BackToMenuButton").GetComponent<Button>();

        undoButton.onClick.AddListener(() => {
            parentPlayer.GetComponent<PlayerMovement>().UndoCommand();
        });

        restartButton.onClick.AddListener(() => {
            RestartLevel();
        });

        backToMenuButton.onClick.AddListener(() => {
            BackToMenu();
        });
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.R)) {
            RestartLevel();
        }
        if (Input.GetKeyDown(KeyCode.Escape)) {
            BackToMenu();
        }
    }

    public void CheckPoints() {
        receivedPoints = 0;
        foreach (GameObject point in points) {
            if (point.name == "TokenPoint") {
                receivedPoints++;
            }
            else {              //If 1 or more points are white
                break;
            }
        }

        if (receivedPoints == points.Length) {
            StartCoroutine(Win());
        }
    }

    private IEnumerator Win() {
        Vector3 screenPosition = cam.WorldToScreenPoint(parentPlayer.transform.position);
        Fade.transform.position = screenPosition;
        Fade.GetComponent<Animator>().SetTrigger("FadeOut");
        parentPlayer.GetComponent<PlayerMovement>().enabled = false;
        if (PlayerPrefs.GetInt("LevelCompleted") < SceneManager.GetActiveScene().buildIndex) {
            PlayerPrefs.SetInt("LevelCompleted", SceneManager.GetActiveScene().buildIndex);
        }
        yield return new WaitForSeconds(1.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
    public void RestartLevel() {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void BackToMenu() {
        SceneManager.LoadScene(0);
    }
}
