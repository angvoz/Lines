using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetButton  : MonoBehaviour { 
    private Board board;

    public static void create(GameObject prefab, Board board) {
        CameraHelper cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
        Vector3 position = cameraHelper.cellToCameraUnchecked(new Vector2Int(board.boardDimension + 3, board.boardDimension - 1), 0);
        position.x = position.x - 0.1f;
        position.y = position.y - 0.25f;
        GameObject resetButtonInstance = Board.Instantiate(prefab, position, Quaternion.identity, parent: board.transform);
        resetButtonInstance.name = "ResetButton";
        ResetButton resetButton = resetButtonInstance.GetComponentInChildren<ResetButton>();
        resetButton.setBoard(board);
    }

    private void setBoard(Board board) {
        this.board = board;
    }

    private void OnMouseUp() {
        if (Input.GetMouseButtonUp(0)) {
            board.reset();
        }
    }
}
