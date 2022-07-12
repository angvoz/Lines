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
    [SerializeField] private float ballScale = 1;
    [SerializeField] public float ballSpeed = 1;

    [SerializeField] private GameObject movePlatePrefab;
    [SerializeField] private GameObject textPlatePrefab;

    private const float BOARD_LEVEL = 0f;

    private GamePosition gamePosition;
    private CameraHelper cameraHelper;
    private PathFinder pathFinder;

    private MovePlate movePlate;
    private Trail trail;

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

    private void SpawnBall(Vector2Int cell) {
        try {
            Ball newBall = Ball.create(ball, cellSize * ballScale, this, cell);
            gamePosition.set(cell, newBall);
        } catch (KeyNotFoundException) {
            // Out of empty cells
        }
    }

    private void SpawnBallsSample() {
        SpawnBall(new Vector2Int(0, 0));
        SpawnBall(new Vector2Int(2, 0));
        SpawnBall(new Vector2Int(1, 6));
        SpawnBall(new Vector2Int(4, 6));
        SpawnBall(new Vector2Int(4, 8));
        SpawnBall(new Vector2Int(7, 6));
        SpawnBall(new Vector2Int(0, 4));
        SpawnBall(new Vector2Int(0, 8));
        SpawnBall(new Vector2Int(3, 4));
        SpawnBall(new Vector2Int(3, 5));
        SpawnBall(new Vector2Int(8, 1));
        SpawnBall(new Vector2Int(7, 0));
        SpawnBall(new Vector2Int(6, 6));
        SpawnBall(new Vector2Int(6, 5));
        SpawnBall(new Vector2Int(4, 1));
        SpawnBall(new Vector2Int(8, 3));
        SpawnBall(new Vector2Int(0, 7));
        SpawnBall(new Vector2Int(1, 5));
        SpawnBall(new Vector2Int(2, 1));
        SpawnBall(new Vector2Int(2, 7));
        SpawnBall(new Vector2Int(4, 0));
        SpawnBall(new Vector2Int(8, 7));
    }

    private void SpawnBalls(int n) {
        List<Ball> balls = Ball.spawn(n, ballPrefab, cellSize * ballScale, this);
    }

    private void Awake() {
        CreateGrid();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(boardDimension, boardDimension);

        gamePosition = new GamePosition(boardDimension, boardDimension);
        cameraHelper = new CameraHelper(boardDimension, boardDimension);
        pathFinder = new PathFinder(gamePosition);
        movePlate = new MovePlate(movePlatePrefab, this);
        trail = new Trail(textPlatePrefab, this);

        //SpawnBalls(5);
        SpawnBallsSample();
    }

    private void OnMouseUp() {
        try {
            Vector2Int selectedCell = cameraHelper.getSelectedCell();

            if (gamePosition.getMovingBall() == null) {
                movePlate.clear();
                trail.clear();
            
                Ball selectedBall = gamePosition.getSelectedBall();
                if (selectedBall != null) {
                    List<Vector2Int> path = pathFinder.getPath(selectedBall, selectedCell);
                    trail.create(path);
                    //trail.createDebug(path, pathFinder);

                    movePlate.create(selectedCell);
                }

                //if (selectedBall != null && gamePosition.get(selectedCell) == null)
                //{
                //    selectedBall.move(cameraHelper.cellToCamera(selectedCell, Ball.BALL_LEVEL));
                //    gamePosition.move(selectedBall, selectedCell);
                //}
            }

            //SpawnBalls(3);
        } catch {
            // Ignore invalid mouse location
        }
    }
}
