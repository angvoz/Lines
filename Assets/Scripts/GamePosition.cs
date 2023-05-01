using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class GamePosition represents position of the balls on the board
public class GamePosition {
    private List<Ball> balls = new List<Ball>();

    public int dimensionX;
    public int dimensionY;
    public GamePosition(int dimX, int dimY) {
        dimensionX = dimX;
        dimensionY = dimY;

        for (int i = 0; i < dimX * dimY; i++) {
            balls.Add(null);
        }
    }

    private int index(Vector2Int cell) {
        return cell.y * dimensionX + cell.x;
    }

    private Vector2Int getCell(int index) {
        return new Vector2Int(index % dimensionX, index / dimensionX);
    }

    public Vector2Int findCell(Ball ball) {
        for (int i = 0; i < dimensionX * dimensionY; i++) {
            if (balls[i] == ball) {
                return getCell(i);
            }
        }
        throw new KeyNotFoundException("The ball was not found on the board!, ball=" + ball);
    }

    private bool valid(int x, int y) {
        return x >= 0 && x < dimensionX && y >= 0 && y < dimensionY;
    }

    public bool valid(Vector2Int cell) {
        return valid(cell.x, cell.y);
    }

    public void set(Vector2Int cell, Ball ball) {
        if (valid(cell) && get(cell) == null) {
            balls[index(cell)] = ball;
        }
    }
    private int emptyCount() {
        int count = 0;
        for (int i = 0; i < balls.Count; i++) {
            if (balls[i] == null) {
                count++;
            }
        }
        return count;
    }

    public bool isFull() {
        return emptyCount() == 0;
    }

    public Vector2Int getEmptyCell() {
        int n = Random.Range(0, emptyCount());
        for (int i = 0; i < balls.Count; i++) {
            if (balls[i] == null) {
                if (n == 0) {
                    return getCell(i);
                }
                n--;
            }
        }
        throw new KeyNotFoundException("No empty cell on the board found, emptyCount=" + emptyCount());
    }

    public void UnselectAll() {
        foreach (Ball ball in balls) {
            if (ball != null && ball.isSelected) {
                ball.Select(false);
            }
        }
    }

    public Ball get(Vector2Int cell) {
        if (valid(cell)) {
            return balls[index(cell)];
        }
        return null;
    }

    public int count() {
        int count = 0;
        for (int i = 0; i < balls.Count; i++) {
            if (balls[i] != null) {
                count++;
            }
        }
        return count;
    }

    public Ball getSelectedBall() {
        foreach (Ball ball in balls) {
            if (ball != null && ball.isSelected) {
                return ball;
            }
        }
        return null;
    }

    public Ball getMovingBall() {
        foreach (Ball ball in balls) {
            if (ball != null && ball.isMoving) {
                return ball;
            }
        }
        return null;
    }

    private int findIndex(Ball ball) {
        for (int i = 0; i < balls.Count; i++) {
            if (ReferenceEquals(ball, balls[i])) {
                return i;
            };
        }

        throw new KeyNotFoundException("Ball " + ball + " not found");
    }

    public void move(Ball ball, Vector2Int cell) {
        try {
            int ind = findIndex(ball);
            balls[ind] = null;
            balls[index(cell)] = ball;
        } catch (KeyNotFoundException) {
            // Ball not found
        }
    }

    public void clear() {
        for (int i = 0; i < dimensionX * dimensionY; i++) {
            if (balls[i] != null) {
                Ball.Destroy(balls[i].gameObject);
                balls[i] = null;
            }
        }
    }
}
