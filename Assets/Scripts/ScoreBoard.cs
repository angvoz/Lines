using TMPro;
using UnityEngine;

public class ScoreBoard {
    private int scoreCount = 0;
    private TextMeshPro scoreboardMesh;

    public ScoreBoard(GameObject prefab, Board board) {
        CameraHelper cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
        Vector3 position = cameraHelper.cellToCameraUnchecked(new Vector2Int(board.boardDimension + 1, 0), 0);
        GameObject scoreBoardinstance = Board.Instantiate(prefab, position, Quaternion.identity, parent: board.transform);
        scoreBoardinstance.name = "Scoreboard";
        scoreboardMesh = scoreBoardinstance.GetComponentInChildren<TextMeshPro>();
        scoreboardMesh.text = "0";
    }

    private int triangularNumber(int number) {
        // Triangular number is a thing in math
        return number * (number + 1) / 2;
    }

    public void score(int cnt) {
        scoreCount = scoreCount + cnt * triangularNumber(cnt - 4) * 2;
        scoreboardMesh.text = scoreCount.ToString();
    }
}
