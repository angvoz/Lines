using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board : MonoBehaviour
{
    private LineRenderer lr;
    private Transform[] points;

    [SerializeField] private int boardDimension = 9;
    [SerializeField] private float cellSize = 1;
    [SerializeField] private GameObject ball;
    [SerializeField] private float ballScale = 1;

    private const int NUMBER_OF_COLORS = 7;

    private GamePosition gamePosition;
    private int ballNumber = 0;

    private readonly struct NamedColor
    {
        public readonly Color color;
        public readonly string name;

        public NamedColor(Color c, string n)
        {
            this.color = c;
            this.name = n;
        }
        
        public NamedColor(int r, int g, int b, string n)
        {
            this.color = new Color(r / 255f, g / 255f, b / 255f);
            this.name = n;
        }
    }

    private void CreateGrid()
    {
        float halfboard = (cellSize * boardDimension) / 2.0f;

        lr = GetComponent<LineRenderer>();
        lr.positionCount = 5 + (boardDimension - 1) * 8; // TODO


        int index = 0;
        lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, 0));
        lr.SetPosition(index++, new Vector3(-halfboard, +halfboard, 0));
        lr.SetPosition(index++, new Vector3(+halfboard, +halfboard, 0));
        lr.SetPosition(index++, new Vector3(+halfboard, -halfboard, 0));
        lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, 0));

        for (int i = 1; i < boardDimension; i++)
        {
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard + i * cellSize, 0));
            lr.SetPosition(index++, new Vector3(+halfboard, -halfboard + i * cellSize, 0));
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard + i * cellSize, 0));
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, 0));

            lr.SetPosition(index++, new Vector3(-halfboard + i * cellSize, -halfboard, 0));
            lr.SetPosition(index++, new Vector3(-halfboard + i * cellSize, +halfboard, 0));
            lr.SetPosition(index++, new Vector3(-halfboard + i * cellSize, -halfboard, 0));
            lr.SetPosition(index++, new Vector3(-halfboard, -halfboard, 0));
        }
    }

    private Ball CreateBall(int x, int y, NamedColor color)
    {
        GameObject ballObject = Instantiate(ball, new Vector3(x - boardDimension/2.0f + 0.5f, y - boardDimension/2.0f + 0.5f, -1), Quaternion.identity, /* parent */ this.transform);
        Ball newBall = ballObject.GetComponent<Ball>();
        string pretty = "+#;-#;0";
        newBall.name = color.name + " Ball " + ++ballNumber + " [" + x.ToString(pretty) + ", " + y.ToString(pretty) + " ]";

        Renderer renderer = ballObject.GetComponent<Renderer>();
        renderer.material.color = color.color;
        float scale = cellSize * ballScale;
        renderer.transform.localScale = new Vector3(scale, scale, scale);

        return newBall;
    }

    private Ball CreateBall(int x, int y)
    {
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
        Ball newBall = CreateBall(x, y, colors[Random.Range(0, numberOfColors)]);
        return newBall;
    }

    private void SpawnBall()
    {
        int index = gamePosition.getRandomIndex();
        if (index >= 0)
        {
            int x = gamePosition.indexToX(index);
            int y = gamePosition.indexToY(index);
            Ball newBall = CreateBall(x, y);
            gamePosition.set(x, y, newBall);
        }
    }

    private void SpawnBalls()
    {
        const int BATCH_SIZE = 3;
        for (int i = 0; i < BATCH_SIZE; i++)
        {
            SpawnBall();
        }
    }

    private void Awake()
    {
        CreateGrid();
        BoxCollider2D collider = GetComponent<BoxCollider2D>();
        collider.size = new Vector2(boardDimension, boardDimension);

        gamePosition = new GamePosition(boardDimension, boardDimension);
        SpawnBalls();
    }

    private void OnMouseUp()
    {
        SpawnBalls();
    }
}
