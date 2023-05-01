using System;
using TMPro;
using UnityEngine;

public class GameOverBoard : MonoBehaviour {
    public const float GAME_OVER_LEVEL = -4.0f;

    private Board board;

    internal static GameOverBoard createGameOverBoard(GameObject gameOverPrefab, Board board) {
        CameraHelper cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
        Vector3 position = cameraHelper.cellToCameraUnchecked(new Vector2Int(board.boardDimension / 2, board.boardDimension / 2), GAME_OVER_LEVEL);
        GameObject gameoverboardObject = Instantiate(gameOverPrefab, position, Quaternion.identity);
        GameOverBoard gameOverBoard = gameoverboardObject.GetComponentInChildren<GameOverBoard>();
        gameOverBoard.setBoard(board);
        return gameOverBoard;
    }

    void setBoard(Board board) {
        this.board = board;
    }

    void Update() {
        if (Input.GetMouseButtonUp(0)) {
            board.reset();
        }
    }
}
