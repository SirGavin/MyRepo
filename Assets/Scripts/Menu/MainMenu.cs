using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

    public string gameSceneName = "hexmap";
    public Dropdown playerCountDD;
    public Button startButton;
    public Button resumeButton;

    private SceneController sceneController;
    private GameSetupData gameSetupData;

    public void Awake() {
        sceneController = FindObjectOfType<SceneController>();
        gameSetupData = FindObjectOfType<GameSetupData>();

        if (gameSetupData.isGameLoaded) {
            startButton.gameObject.SetActive(false);
            resumeButton.gameObject.SetActive(true);
        } else {
            startButton.gameObject.SetActive(true);
            resumeButton.gameObject.SetActive(false);
        }
    }

    public void PlayGame() {
        sceneController.FadeAndLoadScene(gameSceneName);
    }

    public void QuitGame() {
        Application.Quit();
    }

    public void SetPlayerCount(int index) {
        gameSetupData.playerCount = int.Parse(playerCountDD.options[index].text);
    }

    public void SetAIPlayerCount(int aiPlayerCount) {
        gameSetupData.aiPlayerCount = aiPlayerCount;
    }
}
