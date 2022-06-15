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

    private ArrayList balls = new ArrayList();

    static private int ballNumber = 0;

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

    private Ball CreateBall(int x, int y, Color color)
    {
        GameObject ballObject = Instantiate(ball, new Vector3(x, y, -1), Quaternion.identity, /* parent */ this.transform);
        Ball newBall = ballObject.GetComponent<Ball>();
        string pretty = "+#;-#;0";
        newBall.name = "Ball " + ++ballNumber + " [" + x.ToString(pretty) + ", " + y.ToString(pretty) + " ]";
        balls.Add(newBall);

        Renderer renderer = ballObject.GetComponent<Renderer>();
        renderer.material.color = color;
        float scale = cellSize * ballScale;
        renderer.transform.localScale = new Vector3(scale, scale, scale);

        return newBall;
    }

    private void SpawnBall()
    {
        int x = Random.Range(-boardDimension / 2, boardDimension / 2);
        int y = Random.Range(-boardDimension / 2, boardDimension / 2);
        if (balls.Count < boardDimension * boardDimension)
        {
            CreateBall(x, y, Color.green);
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

        SpawnBalls();
    }

    private void OnMouseUp()
    {
        SpawnBalls();
    }
}
