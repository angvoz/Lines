using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ball : MonoBehaviour
{
    public Animator animator;
    public bool isSelected = false;

    static public void UnselectAll()
    {
        foreach (Ball ball in GameObject.FindObjectsOfType<Ball>())
        {
            ball.Select(false);
        }
    }

    public void Select(bool select = true)
    {
        isSelected = select;
        if (animator != null)
        {
            animator.SetBool("selected", isSelected);
        }

    }

    private void OnMouseUp()
    {
        bool select = !isSelected;
        if (select)
        {
            UnselectAll();
        }
        Select(select);
    }
}
