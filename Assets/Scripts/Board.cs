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
    [SerializeField] private GameObject scoreBoardPrefab;
    [SerializeField] private GameObject leaderBoardPrefab;

    private const float BOARD_LEVEL = 0f;

    private GamePosition gamePosition;
    private CameraHelper cameraHelper;
    private PathFinder pathFinder;
    private ScoreBoard scoreboard;
    private LeaderBoard leaderboard;

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

    private void SpawnBalls(int n) {
        for (int i = 0; i < n; i++) {
            Ball ball = Ball.spawn(ballPrefab, cellSize, this);
            pathFinder.CollapseLines(ball);
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
        scoreboard = new ScoreBoard(scoreBoardPrefab, this);
        leaderboard = new LeaderBoard(leaderBoardPrefab, this);

        SpawnBalls(5);
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

    Ball ballArrived = null;

    public void notifyBallArrived(Ball ball) {
        ballArrived = ball;
    }

    public void notifyBallSelected(Ball ball) {
        movePlate.clear();
        trail.clear();
    }

    // Update is called once per frame
    void Update() {
        if (ballArrived != null) {
            Ball ball = ballArrived;
            ballArrived = null;

            if (gamePosition.getMovingBall() == null) {
                trail.clear();
                movePlate.clear();

                int cnt = pathFinder.CollapseLines(ball);
                if (cnt > 0) {
                    int score = scoreboard.score(cnt);
                    leaderboard.updateTopScore(score);
                } else {
                    SpawnBalls(3);
                }
            }
        }
    }
}
