using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public AIController aiController;
    public MapController mapController;
    public TurnController turnController;

    public List<Color> playerColors;
    public int playerCount = 2;
    public int aiPlayerCount = 0;
    public List<Strategy> defaultStrategies;

    public GameObject armyPrefab;
    public Tile borderTile;
    public Tile highlightTile;

    private string menuScene = "Menu";
    private SceneController sceneController;
    private GameSetupData gameSetupData;
    private List<Player> orderedPlayers = new List<Player>();
    private Player currentPlayer;

    private void Awake() {
        sceneController = FindObjectOfType<SceneController>();
        gameSetupData = FindObjectOfType<GameSetupData>();

        if (!gameSetupData || !gameSetupData.isGameLoaded) {
            LoadGameData();
            mapController.ProcessWorldTiles();
            GeneratePlayers();
            StartGame();
        }
    }

    private void Update() {
        if (Input.GetKeyDown(KeyCode.Escape) && sceneController) {
            //TODO: returning from pause menu doesn't work properly
            sceneController.FadeAndLoadScene(menuScene);
        }
    }

    private void LoadGameData() {
        if (gameSetupData) {
            playerCount = gameSetupData.playerCount;
            gameSetupData.isGameLoaded = true;
        }
    }

    private void GeneratePlayers() {
        for (int i = 0; i < playerCount; i++) {
            Player player = new Player(i+1, playerColors[i], defaultStrategies, armyPrefab, borderTile, highlightTile, RemovePlayerFromGame);
            Army army = player.CreateArmy(5);
            mapController.RandomlyPlaceArmy(army);

            orderedPlayers.Add(player);
        }
        for (int i = playerCount; i < playerCount + aiPlayerCount; i++) {
            Player player = new AIPlayer(i + 1, playerColors[i], defaultStrategies, armyPrefab, borderTile, highlightTile, RemovePlayerFromGame);
            Army army = player.CreateArmy(5);
            mapController.RandomlyPlaceArmy(army);

            orderedPlayers.Add(player);
        }
    }

    private void RemovePlayerFromGame(Player player) {
        orderedPlayers.Remove(player);
    }

    private void StartGame() {
        currentPlayer = orderedPlayers.Find(player => player.playerNum == 1);

        SetMapTiles();
        mapController.SetCurrentPlayer(currentPlayer);
        turnController.StartTurn(currentPlayer);
    }

    private void SetMapTiles() {
            mapController.highlightTile = currentPlayer.hovorTile;
            mapController.selectTile = currentPlayer.selectedTile;
    }

    public void EndTurn() {
        mapController.ClearSelection();
        
        int nextPlayerNum = currentPlayer.playerNum == orderedPlayers.Count ? 1 : currentPlayer.playerNum + 1;
        while (!orderedPlayers.Exists(player => player.playerNum == nextPlayerNum)) {
            nextPlayerNum = nextPlayerNum == orderedPlayers.Count ? 1 : nextPlayerNum + 1;
        }

        currentPlayer = orderedPlayers.Find(player => player.playerNum == nextPlayerNum);
        SetMapTiles();

        //if (currentPlayer is AIPlayer) {
        //    aiController.DoAITurn(currentPlayer as AIPlayer);
        //} else {
        mapController.SetCurrentPlayer(currentPlayer);
        turnController.StartTurn(currentPlayer);
        //}
    }

    public Player GetArmiesPlayer(Army army) {
        foreach (Player player in orderedPlayers) {
            if (player.ControlsArmy(army)) return player;
        }

        return null;
    }
}
