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

    private void SpawnBall(int x, int y) {
        Ball.spawn(ballPrefab, cellSize, this, new Vector2Int(x, y));
    }

    private void SpawnBallsSample() {
        SpawnBall(0, 0);
        SpawnBall(2, 0);
        SpawnBall(1, 6);
        SpawnBall(4, 6);
        SpawnBall(4, 8);
        SpawnBall(7, 6);
        SpawnBall(0, 4);
        SpawnBall(0, 8);
        SpawnBall(3, 4);
        SpawnBall(3, 5);
        SpawnBall(8, 1);
        SpawnBall(7, 0);
        SpawnBall(6, 6);
        SpawnBall(6, 5);
        SpawnBall(4, 1);
        SpawnBall(8, 3);
        SpawnBall(0, 7);
        SpawnBall(1, 5);
        SpawnBall(2, 1);
        SpawnBall(2, 7);
        SpawnBall(4, 0);
        SpawnBall(8, 7);
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
        pathFinder = new PathFinder(gamePosition);
        movePlate = new MovePlate(movePlatePrefab, this);
        trail = new Trail(textPlatePrefab, this);

        //SpawnBalls(5);
        SpawnBallsSample();
    }

    private void OnMouseUp() {
        try {
            if (gamePosition.getMovingBall() == null) {
                movePlate.clear();
                trail.clear();
            
                Ball selectedBall = gamePosition.getSelectedBall();
                if (selectedBall != null) {
                    Vector2Int selectedCell = cameraHelper.getSelectedCell();
                    if (gamePosition.valid(selectedCell) && gamePosition.get(selectedCell) == null) {
                        List<Vector2Int> path = pathFinder.getPath(selectedBall, selectedCell);
                        if (path.Count > 0) {
                            movePlate.create(selectedCell);
                            trail.create(path);
                            List<Vector3> cameraPath = new List<Vector3>();
                            for (int i = 1; i < path.Count; i++) {
                                cameraPath.Add(cameraHelper.cellToCamera(path[i], Ball.BALL_LEVEL));
                            }
                            selectedBall.MoveTo(cameraPath);
                            gamePosition.move(selectedBall, selectedCell);
                        }
                    }
                }
            }
        } catch {
            // Ignore invalid mouse location
        }
    }

    bool eventBallArrived = false;

    public void notifyBallArrived() {
        eventBallArrived = true;
    }

    // Update is called once per frame
    void Update() {
        if (eventBallArrived) {
            eventBallArrived = false;

            if (gamePosition.getMovingBall() == null) {
                trail.clear();
                movePlate.clear();
                SpawnBalls(3);
            }
        }
    }
}
