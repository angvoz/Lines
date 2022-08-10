using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Board : MonoBehaviour {
    private LineRenderer lr;
    private Transform[] points;

    [SerializeField] public int boardDimension = 9;
    [SerializeField] private float cellSize = 1;
    [SerializeField] private GameObject ballPrefab;
    [SerializeField] public float ballSpeed = 1;

    [SerializeField] private GameObject movePlatePrefab;

    private const float BOARD_LEVEL = 0f;

    private GamePosition gamePosition;
    private CameraHelper cameraHelper;

    private MovePlate movePlate;

    public GamePosition GetGamePosition() {
        return gamePosition;
    }

    void Start() {
        QualitySettings.vSyncCount = 0;
        Application.targetFrameRate = 90;
    }

    private void CreateGrid() {
        float halfboard = (cellSize * boardDimension) / 2.0f;

        lr = GetComponent<LineRenderer>();
        lr.positionCount = 5 + (boardDimension - 1) * 8; // TODO


        int index = 0;
        lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, BOARD_LEVEL));
        lr.SetPosition(index++, new Vector3(-halfboard, +halfboard, BOARD_LEVEL));
        lr.SetPosition(index++, new Vector3(+halfboard, +halfboard, BOARD_LEVEL));
        lr.SetPosition(index++, new Vector3(+halfboard, -halfboard, BOARD_LEVEL));
        lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, BOARD_LEVEL));

        for (int i = 1; i < boardDimension; i++) {
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard + i * cellSize, BOARD_LEVEL));
            lr.SetPosition(index++, new Vector3(+halfboard, -halfboard + i * cellSize, BOARD_LEVEL));
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard + i * cellSize, BOARD_LEVEL));
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, BOARD_LEVEL));

            lr.SetPosition(index++, new Vector3(-halfboard + i * cellSize, -halfboard, BOARD_LEVEL));
            lr.SetPosition(index++, new Vector3(-halfboard + i * cellSize, +halfboard, BOARD_LEVEL));
            lr.SetPosition(index++, new Vector3(-halfboard + i * cellSize, -halfboard, BOARD_LEVEL));
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, BOARD_LEVEL));
        }
    }

    private void SpawnBalls(int n) {
        for (int i = 0; i < n; i++) {
            Ball.spawn(ballPrefab, cellSize, this);
        }
    }

    private void Awake() {
        CreateGrid();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(boardDimension, boardDimension);

        gamePosition = new GamePosition(boardDimension, boardDimension);
        cameraHelper = new CameraHelper(boardDimension, boardDimension);
        movePlate = new MovePlate(movePlatePrefab, this);

        SpawnBalls(5);
    }

    private void OnMouseUp() {
        try {
            Vector2Int selectedCell = cameraHelper.getSelectedCell();

            if (gamePosition.getMovingBall() == null) {
                Ball selectedBall = gamePosition.getSelectedBall();
                if (selectedBall != null) {
                    movePlate.create(selectedCell);
                }

                if (selectedBall != null && gamePosition.get(selectedCell) == null) {
                    selectedBall.move(cameraHelper.cellToCamera(selectedCell, Ball.BALL_LEVEL));
                    gamePosition.move(selectedBall, selectedCell);
                }
            }

            //SpawnBalls(3);
        } catch {
            // Ignore invalid mouse location
        }
    }
}
