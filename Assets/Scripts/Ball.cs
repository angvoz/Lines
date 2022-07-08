using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    private const int NUMBER_OF_COLORS = 7;

    public const int COLOR_RED = 0;
    public const int COLOR_CYAN = 1;
    public const int COLOR_PURPLE = 2;
    public const int COLOR_BLUE = 3;
    public const int COLOR_GREEN = 4;
    public const int COLOR_BROWN = 5;
    public const int COLOR_YELLOW = 6;
    public const int COLOR_BLACK = 7;

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

    private static NamedColor[] colors = new NamedColor[NUMBER_OF_COLORS + 1];

    static Ball() {
        colors[COLOR_RED] = new NamedColor(255, 51, 51, "Red");
        colors[COLOR_CYAN] = new NamedColor(Color.cyan, "Cyan");
        colors[COLOR_PURPLE] = new NamedColor(255, 0, 255, "Purple");
        colors[COLOR_BLUE] = new NamedColor(30, 144, 255, "Blue");
        colors[COLOR_GREEN] = new NamedColor(Color.green, "Green");
        colors[COLOR_BROWN] = new NamedColor(230, 128, 0, "Brown");
        colors[COLOR_YELLOW] = new NamedColor(Color.yellow, "Yellow");
        colors[COLOR_BLACK] = new NamedColor(Color.black, "Black");
    }

    public int color = 0;
    static private int ballNumber = 0;

    public Animator animator;
    public bool isSelected = false;

    public const float BALL_LEVEL = -1.0f;

    static private Ball create(GameObject prefab, float scale, Board board, int x, int y, int colorIndex = -1) {
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
        if (colorIndex < 0 || colorIndex >= numberOfColors) {
            colorIndex = Random.Range(0, numberOfColors);
        }
        NamedColor color = colors[colorIndex];
        GameObject ballObject = Instantiate(prefab, board.cellToCamera(x, y, BALL_LEVEL), Quaternion.identity, /* parent */ board.transform);

        Ball ball = ballObject.GetComponent<Ball>();
        ball.color = colorIndex;
        ball.name = color.name + " Ball " + ++ballNumber + " [" + x + ", " + y + " ]";

        Renderer renderer = ballObject.GetComponent<Renderer>();
        renderer.material.color = color.color;
        Vector3 prefabScale = renderer.transform.localScale;
        renderer.transform.localScale = new Vector3(prefabScale.x * scale, prefabScale.y * scale, prefabScale.z * scale);
        return ball;
    }

    static public Ball spawn(GameObject prefab, float scale, Board board) {
        int x = Random.Range(0, board.boardDimension);
        int y = Random.Range(0, board.boardDimension);

        Ball ball = create(prefab, scale, board, x, y);
        return ball;
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
