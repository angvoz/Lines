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
            textPlateText.text = text;
        }
        textPlateList.Add(textPlateObject);
        return textPlateObject;
    }
}
