using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    private const int NUMBER_OF_COLORS = 7;
    
    public int color = 0;
    static private int ballNumber = 0;

    public Animator animator;
    public bool isSelected = false;

    private Board board;
    private GamePosition gamePosition;

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

    static private Ball create(GameObject prefab, float scale, Board board, int x, int y) {
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
        GameObject ballObject = Instantiate(prefab, board.cellToCamera(x, y, BALL_LEVEL), Quaternion.identity, /* parent */ board.transform);

        Ball ball = ballObject.GetComponent<Ball>();
        ball.color = colorIndex;
        ball.name = color.name + " Ball " + ++ballNumber + " [" + x + ", " + y + " ]";

        Renderer renderer = ballObject.GetComponent<Renderer>();
        renderer.material.color = color.color;
        renderer.transform.localScale = new Vector3(scale, scale, scale);
        return ball;
    }

    static private Ball spawn(GameObject prefab, float scale, Board board) {
        Ball ball = null;
        GamePosition gamePosition = board.GetGamePosition();
        int index = gamePosition.getRandomIndex();
        if (index >= 0) {
            int x = gamePosition.indexToX(index);
            int y = gamePosition.indexToY(index);
            ball = create(prefab, scale, board, x, y);;
            gamePosition.set(x, y, ball);
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
    }

    static public void UnselectAll() {
        foreach (Ball ball in GameObject.FindObjectsOfType<Ball>()) {
            ball.Select(false);
        }
    }

    public void Select(bool select = true) {
        isSelected = select;
        if (animator != null) {
            animator.SetBool("selected", isSelected);
        }

    }

    private void OnMouseUp() {
        bool select = !isSelected;
        if (select) {
            UnselectAll();
        }
        Select(select);
    }
}
