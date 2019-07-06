﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public AIController aiController;
    public MapController mapController;
    public TurnController turnController;

    public List<Color> playerColors;
    public int playerCount = 2;
    public List<Player> humanPlayers;
    public List<AIPlayer> aiPlayers;
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
            //GenerateTurnOrder();
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
            Player player = new Player(i+1, playerColors[i], defaultStrategies, armyPrefab, borderTile, highlightTile);

            Army army = player.CreateArmy(5);
            mapController.RandomlyPlaceArmy(army);

            orderedPlayers.Add(player);
        }
    }

    private void GenerateTurnOrder() {
        orderedPlayers = new List<Player>();
        orderedPlayers.AddRange(humanPlayers);

        foreach (AIPlayer ai in aiPlayers) {
            orderedPlayers.Add(ai);
        }
    }

    private void StartGame() {
        currentPlayer = orderedPlayers.Find(player => player.playerNum == 1);

        SetMapTiles();
        turnController.StartTurn(currentPlayer);
    }

    private void SetMapTiles() {
        mapController.highlightTile = currentPlayer.hovorTile;
        mapController.selectTile = currentPlayer.selectedTile;
    }

    public void EndTurn() {
        mapController.ClearSelection();

        int nextPlayerNum = currentPlayer.playerNum == orderedPlayers.Count ? 1 : currentPlayer.playerNum + 1;
        currentPlayer = orderedPlayers.Find(player => player.playerNum == nextPlayerNum);
        SetMapTiles();

        if (currentPlayer is AIPlayer) {
            aiController.DoAITurn(currentPlayer as AIPlayer);
        } else {
            turnController.StartTurn(currentPlayer);
        }
    }
}