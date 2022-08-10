using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Class GamePosition represents position of the balls on the board
public class GamePosition {
    private ArrayList balls = new ArrayList();

    private int dimensionX;
    private int dimensionY;
    public GamePosition(int dimX, int dimY) {
        dimensionX = dimX;
        dimensionY = dimY;

        for (int i = 0; i < dimX * dimY; i++) {
            balls.Add(null);
        }
    }

    private int index(int x, int y) {
        return y * dimensionX + x;
    }

    public int indexToX(int index) {
        return index % dimensionX;
    }
    public int indexToY(int index) {
        return index / dimensionX;
    }

    private bool valid(int x, int y) {
        return x >= 0 && x < dimensionX && y >= 0 && y < dimensionY;
    }

    public void set(int x, int y, Ball ball) {
        if (valid(x, y) && get(x, y) == null) {
            balls[index(x, y)] = ball;
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
    public int getRandomIndex() {
        int n = Random.Range(0, emptyCount());
        for (int i = 0; i < balls.Count; i++) {
            if (balls[i] == null) {
                if (n == 0) {
                    return i;
                }
                n--;
            }
        }
        return -1;
    }

    public void UnselectAll() {
        foreach (Ball ball in balls) {
            if (ball != null && ball.isSelected) {
                ball.Select(false);
            }
        }
    }

    public Ball get(int x, int y) {
        if (valid(x, y)) {
            return (Ball)balls[index(x, y)];
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

    public void move(Ball ball, int x, int y) {
        try {
            int ind = findIndex(ball);
            balls[ind] = null;
            balls[index(x, y)] = ball;
        } catch (KeyNotFoundException) {
            // Ball not found
        }
    }
}
