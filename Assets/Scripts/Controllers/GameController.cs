using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameController : MonoBehaviour {

    public AIController aiController;
    public MapController mapController;
    public TurnController turnController;

    public List<Player> humanPlayers;
    public List<AIPlayer> aiPlayers;
    public List<Strategy> defaultStrategies;

    public GameObject armyPrefab;
    public Tile borderTile;
    public Tile highlightTile;

    private List<Player> orderedPlayers;
    private Player currentPlayer;

    private void Awake() {
        mapController.ProcessWorldTiles();
        GenerateTurnOrder();
        GeneratePlayers();
        StartGame();
    }

    private void GenerateTurnOrder() {
        orderedPlayers = new List<Player>();
        orderedPlayers.AddRange(humanPlayers);

        foreach (AIPlayer ai in aiPlayers) {
            orderedPlayers.Add(ai);
        }
    }

    private void GeneratePlayers() {
        foreach (Player player in orderedPlayers) {
            currentPlayer = player;

            player.Init(defaultStrategies, armyPrefab, borderTile, highlightTile);

            Army army = player.CreateArmy(5);
            mapController.RandomlyPlaceArmy(army);
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
