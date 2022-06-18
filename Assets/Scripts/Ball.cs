using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Animator animator;
    public bool isSelected = false;
    public bool isMoving = false;

    private GamePosition gamePosition;

    private Vector3 moveDestination;
    private const float SPEED = 0.7f;

    private void Awake()
    {
        GameObject boardObj = GameObject.Find("Board");
        Board board = boardObj.GetComponent<Board>();
        gamePosition = board.GetGamePosition();

        moveDestination = transform.position;
    }

    public void Select(bool select = true)
    {
        isSelected = select;
        if (animator != null)
        {
            animator.SetBool("selected", isSelected);
        }

    }

    public void MoveTo(float x, float y)
    {
        moveDestination = new Vector3(x, y, 0);
    }

    private void OnMouseUp()
    {
        if (!isMoving && gamePosition.getMovingBall() == null)
        {
            bool select = !isSelected;
            if (select)
            {
                gamePosition.UnselectAll();
            }
            Select(select);
        }
    }

    
    // Update is called once per frame
    void Update()
    {
        if (isMoving && transform.position == moveDestination)
        {
            Select(false);
        }

        isMoving = transform.position != moveDestination;
        if (isMoving)
        {
            transform.position = Vector3.MoveTowards(transform.position, moveDestination, SPEED * Time.deltaTime);
        }
    }
}
