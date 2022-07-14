using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    private const int NUMBER_OF_COLORS = 7;
    
    public int color = 0;
    static private int ballNumber = 0;

    public Animator animator;
    public bool isSelected = false;
    public bool isMoving = false;

    private Board board;
    private GamePosition gamePosition;

    private Vector3 moveDestination;
    private float speed;

    public const float BALL_LEVEL = -1.0f;

    private readonly struct NamedColor {
        public readonly Color color;
        public readonly string name;

        public NamedColor(Color c, string n) {
            this.color = c;
            this.name = n;
        }

        public NamedColor(int r, int g, int b, string n) {
            this.color = new Color(r / 255f, g / 255f, b / 255f);
            this.name = n;
        }
    }

    static public Ball create(GameObject prefab, float scale, Board board, Vector2Int cell) {
        NamedColor[] colors = {
            new NamedColor(255, 51, 51, "Red"),
            new NamedColor(Color.cyan, "Cyan"),
            new NamedColor(255, 0, 255, "Purple"),
            new NamedColor(30, 144, 255, "Blue"),
            new NamedColor(Color.green, "Green"),
            new NamedColor(230, 128, 0, "Brown"),
            new NamedColor(Color.yellow, "Yellow"),
            new NamedColor(Color.black, "Black"),
        };
        int numberOfColors = NUMBER_OF_COLORS <= colors.Length ? NUMBER_OF_COLORS : colors.Length;
        int colorIndex = Random.Range(0, numberOfColors);
        NamedColor color = colors[colorIndex];
        CameraHelper cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
        Vector3 position = cameraHelper.cellToCamera(cell, BALL_LEVEL);
        GameObject ballObject = Instantiate(prefab, position, Quaternion.identity, parent: board.transform);

        Ball ball = ballObject.GetComponent<Ball>();
        ball.color = colorIndex;
        ball.name = color.name + " Ball " + ++ballNumber + " [" + cell.x + ", " + cell.y + " ]";

        Renderer renderer = ballObject.GetComponent<Renderer>();
        renderer.material.color = color.color;
        renderer.transform.localScale = new Vector3(scale, scale, scale);
        return ball;
    }

    static private Ball spawn(GameObject prefab, float scale, Board board) {
        Ball ball = null;
        try {
            GamePosition gamePosition = board.GetGamePosition();
            Vector2Int cell = gamePosition.getEmptyCell();
            ball = create(prefab, scale, board, cell);
            gamePosition.set(cell, ball);
        } catch (KeyNotFoundException) {
            // Out of empty cells
        }
        return ball;
    }

    static public List<Ball> spawn(int n, GameObject prefab, float scale, Board board) {
        List<Ball> balls = new List<Ball>();
        for (int i = 0; i < n; i++) {
            Ball b = Ball.spawn(prefab, scale, board);
            if (b != null) {
                balls.Add(b);
            }
        }
        return balls;
    }

    private void Awake() {
        GameObject boardObj = GameObject.Find("Board");
        board = boardObj.GetComponent<Board>();
        gamePosition = board.GetGamePosition();
        speed = board.ballSpeed;

        moveDestination = transform.position;
    }

    public void Select(bool select = true) {
        isSelected = select;
        if (animator != null) {
            animator.SetBool("selected", isSelected);
        }

    }

    List<Vector3> moveDestinationPath = new List<Vector3>();
    public void move(Vector3 destination) {
        moveDestination = destination;
    }

    public void MoveTo(List<Vector3> destinationPath) {
        moveDestinationPath = destinationPath;
        if (moveDestinationPath.Count > 0) {
            moveDestination = destinationPath[0];
        }
    }

    private void OnMouseUp() {
        if (!isMoving && gamePosition.getMovingBall() == null) {
            bool select = !isSelected;
            if (select) {
                gamePosition.UnselectAll();
            }
            Select(select);
        }
    }


    // Update is called once per frame
    void Update() {
        if (isMoving && transform.position == moveDestination) {
            // Arrived to moveDestination
            if (moveDestinationPath.Count > 0) {
                if (moveDestinationPath[0] == moveDestination) {
                    moveDestinationPath.RemoveAt(0);
                }
                if (moveDestinationPath.Count > 0) {
                    moveDestination = moveDestinationPath[0];
                } else {
                    Select(false);
                    board.notifyBallArrived();
                }
            } else {
                Select(false);
                board.notifyBallArrived();
            }
        }

        isMoving = transform.position != moveDestination;
        if (isMoving) {
            transform.position = Vector3.MoveTowards(transform.position, moveDestination, speed * Time.deltaTime);
        }
    }
}
