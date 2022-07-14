using System;
using UnityEngine;

public class CameraHelper {
    public int boardDimensionX;
    public int boardDimensionY;

    public CameraHelper(int boardDimensionX, int boardDimensionY) {
        this.boardDimensionX = boardDimensionX;
        this.boardDimensionY = boardDimensionY;
    }

    public Vector3 cellToCameraUnchecked(Vector2Int cell, float z) {
        float x = cell.x - boardDimensionX / 2.0f + 0.5f;
        float y = cell.y - boardDimensionY / 2.0f + 0.5f;

        return new Vector3(x, y, z);
    }

    public Vector3 cellToCamera(Vector2Int cell, float z) {
        if (cell.x < 0 || cell.x >= boardDimensionX) {
            throw new ArgumentOutOfRangeException("cell (" + cell.x + "," + cell.y + "): x=" + cell.x + " is out of range");
        }

        if (cell.y < 0 || cell.y >= boardDimensionY) {
            throw new ArgumentOutOfRangeException("cell (" + cell.x + "," + cell.y + "): y=" + cell.y + " is out of range");
        }

        return cellToCameraUnchecked(cell, z);
    }

    public Vector2Int getSelectedCell() {
        Vector3 cameraPoint = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int cellX = (int)(Math.Floor(cameraPoint.x + boardDimensionX / 2f));
        int cellY = (int)(Math.Floor(cameraPoint.y + boardDimensionY / 2f));

        if (cellX < 0 || cellX >= boardDimensionX) {
            throw new ArgumentOutOfRangeException("cell (" + cellX + "," + cellY + "): x=" + cellX + " is out of range");
        }

        if (cellY < 0 || cellY >= boardDimensionY) {
            throw new ArgumentOutOfRangeException("cell (" + cellX + "," + cellY + "): y=" + cellY + " is out of range");
        }

        return new Vector2Int(cellX, cellY);
    }
}
