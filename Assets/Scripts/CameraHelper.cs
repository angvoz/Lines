using System;
using UnityEngine;

public class CameraHelper {
    public int boardDimensionX;
    public int boardDimensionY;

    public CameraHelper(int boardDimensionX, int boardDimensionY) {
        this.boardDimensionX = boardDimensionX;
        this.boardDimensionY = boardDimensionY;
    }

    public Vector3 cellToCameraUnchecked(int cellX, int cellY, float z) {
        float x = cellX - boardDimensionX / 2.0f + 0.5f;
        float y = cellY - boardDimensionY / 2.0f + 0.5f;

        return new Vector3(x, y, z);
    }

    public Vector3 cellToCamera(int cellX, int cellY, float z) {
        if (cellX < 0 || cellX >= boardDimensionX) {
            throw new ArgumentOutOfRangeException("cell (" + cellX + "," + cellY + "): x=" + cellX + " is out of range");
        }

        if (cellY < 0 || cellY >= boardDimensionY) {
            throw new ArgumentOutOfRangeException("cell (" + cellX + "," + cellY + "): y=" + cellY + " is out of range");
        }

        return cellToCameraUnchecked(cellX, cellY, z);
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
