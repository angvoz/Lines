using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour {
    static private int ballNumber = 0;

    public Animator animator;
    public bool isSelected = false;

    public const float BALL_LEVEL = -1.0f;

    static private Ball create(GameObject prefab, float scale, Board board, int x, int y) {
        GameObject ballObject = Instantiate(prefab, board.cellToCamera(x, y, BALL_LEVEL), Quaternion.identity, /* parent */ board.transform);

        Ball ball = ballObject.GetComponent<Ball>();
        ball.name = "Ball " + ++ballNumber + " [" + x + ", " + y + " ]";

        Renderer renderer = ballObject.GetComponent<Renderer>();
        renderer.material.color = Color.green;
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
