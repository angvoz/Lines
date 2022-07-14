using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Trail {
    private List<GameObject> textPlateList = new List<GameObject>();

    private Board board;
    private CameraHelper cameraHelper;
    private GameObject textPlatePrefab;

    public Trail(GameObject prefab, Board parent) {
        textPlatePrefab = prefab;
        board = parent;
        cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
    }

    public void clear() {
        foreach (GameObject plate in textPlateList) {
            Board.Destroy(plate);
        }
        textPlateList = new List<GameObject>();
    }

    public GameObject createPlate(Vector2Int cell, string text) {
        Vector3 position = cameraHelper.cellToCamera(cell, 0);
        GameObject textPlateObject = Board.Instantiate(textPlatePrefab, position, Quaternion.identity, parent: board.transform);
        textPlateObject.name = "Text Plate " + cell;
        TextMeshPro textPlateText = textPlateObject.GetComponentInChildren<TextMeshPro>();
        if (textPlateText != null) {
            textPlateText.text = "";
        }
        textPlateList.Add(textPlateObject);
        return textPlateObject;
    }

    private GameObject createPlate(Vector2Int cell, Vector2Int? cellTo) {
        string text = "";
        if (cellTo.HasValue) {
            text = cellTo.ToString();
        }
        GameObject textPlateObject = createPlate(cell, text);
        return textPlateObject;
    }

    public void create(List<Vector2Int> path) {
        for (int i = 0; i < path.Count - 1; i++) {
            Vector2Int from = path[i];
            Vector2Int to = path[i + 1];
            if (from.x == to.x) {
                Range rangeY = new Range(from.y, to.y);
                for (int y = rangeY.start; rangeY.includes(y) && y != rangeY.end; y = rangeY.next(y)) {
                    createPlate(new Vector2Int(from.x, y), to);
                }
            }
            if (from.y == to.y) {
                Range rangeX = new Range(from.x, to.x);
                for (int x = rangeX.start; rangeX.includes(x) && x != rangeX.end; x = rangeX.next(x)) {
                    createPlate(new Vector2Int(x, from.y), to);
                }
            }
        }
    }
}
