using UnityEngine;

public class MovePlate
{
    public const float PLATE_LEVEL = -0.1f;

    private Board board;
    private GameObject movePlatePrefab;
    private GameObject movePlateInstance;

    private CameraHelper cameraHelper;

    public MovePlate(GameObject prefab, Board parent) {
        movePlatePrefab = prefab;
        board = parent;
        cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
    }

    public void create(int x, int y) {
        if (movePlateInstance != null) {
            Board.Destroy(movePlateInstance);
        }

        Vector3 position = cameraHelper.cellToCamera(x, y, PLATE_LEVEL);
        movePlateInstance = Board.Instantiate(movePlatePrefab, position, Quaternion.identity, parent: board.transform);
        movePlateInstance.name = "Move Plate";
    }

    public void clear() {
        if (movePlateInstance != null) {
            Board.Destroy(movePlateInstance);
        }
        movePlateInstance = null;
    }
}
