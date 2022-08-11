using System.Collections.Generic;
using UnityEngine;

public class PathFinder
{
    private GamePosition gamePosition;
    private Vector2Int?[,] pathPlan;
    private List<Vector2Int> toDoLanes;
    private List<Vector2Int> processed;

    public PathFinder(GamePosition position) {
        this.gamePosition = position;
    }

    bool makeDirectPath(Vector2Int cellFrom, Vector2Int cellTo, bool inclusive) {
        if (cellFrom == cellTo) {
            return true;
        }

        if (cellFrom.x == cellTo.x || cellFrom.y == cellTo.y) {
            Range rangeX = new Range(cellFrom.x, cellTo.x);
            Range rangeY = new Range(cellFrom.y, cellTo.y);
            for (int x = rangeX.start; rangeX.includes(x); x = rangeX.next(x)) {
                for (int y = rangeY.start; rangeY.includes(y); y = rangeY.next(y)) {
                    if (!pathPlan[x, y].HasValue) {
                        Ball ball = gamePosition.get(new Vector2Int(x, y));

                        if (x == cellTo.x && y == cellTo.y) {
                            if (inclusive || ball == null) {
                                pathPlan[x, y] = cellFrom;
                                {
                                    Vector2Int cell = new Vector2Int(x, y);
                                    if (!processed.Contains(cell)) {
                                        toDoLanes.Add(cell);
                                    }
                                }
                            }
                            return true;
                        }
                        if (ball != null) {
                            return false;
                        }
                        pathPlan[x, y] = cellFrom;
                        {
                            Vector2Int cell = new Vector2Int(x, y);
                            if (!processed.Contains(cell)) {
                                toDoLanes.Add(cell);
                            }
                        }
                    }
                }
            }
        }

        return false;
    }

    private bool makePathSegment(Vector2Int cellFrom, Vector2Int cellTo, bool inclusive) {
        // Try straight path on a row or a column first
        bool done = makeDirectPath(cellFrom, cellTo, inclusive);

        if (!done) {
            // Try to make path with a single turn
            Vector2Int corner1 = new Vector2Int(cellTo.x, cellFrom.y);
            Vector2Int corner2 = new Vector2Int(cellFrom.x, cellTo.y);

            Vector2Int[] corners = { corner1, corner2 };
            foreach (Vector2Int corner in corners) {
                if (!done) {
                    if (gamePosition.get(corner) == null) {
                        done = makeDirectPath(cellFrom, corner, false) && makeDirectPath(corner, cellTo, inclusive);
                    }
                }

            }
        }

        if (!done) {
            // just populate the map with straight path from every cell of the current row or current column
            Vector2Int cellBottom = new Vector2Int(cellFrom.x, 0);
            Vector2Int cellTop = new Vector2Int(cellFrom.x, gamePosition.dimensionY - 1);
            Vector2Int cellFarLeft = new Vector2Int(0, cellFrom.y);
            Vector2Int cellFarRight = new Vector2Int(gamePosition.dimensionX - 1, cellFrom.y);

            Vector2Int[] borderCells = {
                cellBottom,
                cellTop,
                cellFarLeft,
                cellFarRight,
            };
            foreach (Vector2Int borderCell in borderCells) {
                makeDirectPath(cellFrom, borderCell, false);
            }
        }

        return done;
    }

    private bool makePath(Vector2Int cellFrom, Vector2Int cellTo) {
        bool inclusive = true;
        bool done = makePathSegment(cellFrom, cellTo, inclusive);

        if (!done) {
            toDoLanes.Add(cellFrom);

            int antiloop1 = gamePosition.dimensionX * gamePosition.dimensionY;
            int antiloop2 = gamePosition.dimensionX * gamePosition.dimensionY;

            while (toDoLanes.Count > 0 && !done) {
                if (antiloop1-- < 0) {
                    Debug.Log("GamePositions.makePath: Error: Loop1 exceeded antiloop measure");
                    break;
                }

                List<Vector2Int> cellsToProcess = new List<Vector2Int>();
                foreach (Vector2Int cell in toDoLanes) {
                    cellsToProcess.Add(cell);
                }

                foreach (Vector2Int cell in cellsToProcess) {
                    if (antiloop2-- < 0) {
                        Debug.Log("GamePositions.makePath: Error: Loop2 exceeded antiloop measure");
                        break;
                    }

                    done = makePathLanes(cell, cellTo, inclusive);
                    toDoLanes.Remove(cell);
                    processed.Add(cell);

                    if (done) {
                        break;
                    }
                }
            }
        }

        pathPlan[cellFrom.x, cellFrom.y] = null; // new Vector2Int(-1, -1);
        return done;
    }

    private bool makePathLanes(Vector2Int cellFrom, Vector2Int cellTo, bool inclusive) {
        bool done = false;

        Lane laneRight = new Lane(new Range(cellFrom.x + 1, gamePosition.dimensionX - 1), new Range(cellFrom.y, cellFrom.y));
        Lane laneLeft = new Lane(new Range(cellFrom.x - 1, 0), new Range(cellFrom.y, cellFrom.y));
        Lane laneUp = new Lane(new Range(cellFrom.x, cellFrom.x), new Range(cellFrom.y + 1, gamePosition.dimensionY - 1));
        Lane laneDown = new Lane(new Range(cellFrom.x, cellFrom.x), new Range(cellFrom.y - 1, 0));

        List<Lane> lanes = new List<Lane>();
        if (cellFrom.x <= cellTo.x) {
            lanes.Add(laneRight);
            lanes.Add(laneLeft);
        } else {
            lanes.Add(laneLeft);
            lanes.Add(laneRight);
        }
        if (cellFrom.y <= cellTo.y) {
            lanes.Add(laneUp);
            lanes.Add(laneDown);
        } else {
            lanes.Add(laneDown);
            lanes.Add(laneUp);
        }

        foreach (Lane lane in lanes) {
            Range rangeX = lane.rangeX;
            Range rangeY = lane.rangeY;
            for (int x = rangeX.start; rangeX.includes(x) && !done; x = rangeX.next(x)) {
                for (int y = rangeY.start; rangeY.includes(y) && !done; y = rangeY.next(y)) {
                    Vector2Int cell = new Vector2Int(x, y);
                    if (gamePosition.valid(cell) && pathPlan[x, y] == cellFrom) {
                        if (!processed.Contains(cell)) {
                            done = makePathSegment(cell, cellTo, inclusive);
                        }
                    }
                }
            }
        }

        return done;
    }
    public List<Vector2Int> getPath(Ball ball, Vector2Int cellTo) {
        List<Vector2Int> path = new List<Vector2Int>();
        pathPlan = new Vector2Int?[gamePosition.dimensionX, gamePosition.dimensionY];
        toDoLanes = new List<Vector2Int>();
        processed = new List<Vector2Int>();

        Vector2Int cellFrom = gamePosition.findCell(ball);
        // Making path in reverse order
        bool done = makePath(cellTo, cellFrom);
        if (done) {
            path.Add(cellFrom);
            for (Vector2Int node = cellFrom; pathPlan[node.x, node.y] != null; node = pathPlan[node.x, node.y].GetValueOrDefault()) {
                Vector2Int nodeTo = pathPlan[node.x, node.y].GetValueOrDefault();
                if (gamePosition.valid(nodeTo)) {
                    path.Add(nodeTo);
                } else {
                    Debug.Log("GamePosition.getPath: Error: invalid nodeTo=" + nodeTo);
                    break;
                }
            }
        }

        return path;
    }

    /*
     * Collapse all lines having 5 or more balls in the row
     */
    public int CollapseLines(Ball ball) {
        if (ball == null) {
            return 0;
        }

        const int COLLAPSE_COUNT = 5;

        List<Vector2Int> cellsToDelete = new List<Vector2Int>();
        try {
            List<Vector2Int> cellsCandidate = new List<Vector2Int>();
            Vector2Int cellStart = gamePosition.findCell(ball);

            // horizontal and vertical lanes
            Lane laneRight = new Lane(new Range(cellStart.x + 1, gamePosition.dimensionX - 1), new Range(cellStart.y, cellStart.y));
            Lane laneLeft = new Lane(new Range(cellStart.x - 1, 0), new Range(cellStart.y, cellStart.y));
            Lane laneUp = new Lane(new Range(cellStart.x, cellStart.x), new Range(cellStart.y + 1, gamePosition.dimensionY - 1));
            Lane laneDown = new Lane(new Range(cellStart.x, cellStart.x), new Range(cellStart.y - 1, 0));

            List<Lane> lanes = new List<Lane>();

            // left/right
            lanes.Add(laneRight);
            lanes.Add(laneLeft);

            foreach (Lane lane in lanes) {
                Range rangeX = lane.rangeX;
                Range rangeY = lane.rangeY;
                bool done = false;
                for (int x = rangeX.start; rangeX.includes(x) && !done; x = rangeX.next(x)) {
                    for (int y = rangeY.start; rangeY.includes(y) && !done; y = rangeY.next(y)) {
                        Vector2Int cell = new Vector2Int(x, y);
                        Ball b = gamePosition.get(cell);
                        if (b == null || b.color != ball.color) {
                            done = true;
                        } else if (b.color == ball.color) {
                            cellsCandidate.Add(cell);
                        }
                    }
                }
            }
            if (cellsCandidate.Count >= COLLAPSE_COUNT - 1) {
                cellsToDelete.AddRange(cellsCandidate);
            }
            cellsCandidate.Clear();
            lanes.Clear();

            // up/down
            lanes.Add(laneUp);
            lanes.Add(laneDown);
            foreach (Lane lane in lanes) {
                Range rangeX = lane.rangeX;
                Range rangeY = lane.rangeY;
                bool done = false;
                for (int x = rangeX.start; rangeX.includes(x) && !done; x = rangeX.next(x)) {
                    for (int y = rangeY.start; rangeY.includes(y) && !done; y = rangeY.next(y)) {
                        Vector2Int cell = new Vector2Int(x, y);
                        Ball b = gamePosition.get(cell);
                        if (b == null || b.color != ball.color) {
                            done = true;
                        } else if (b.color == ball.color) {
                            cellsCandidate.Add(cell);
                        }
                    }
                }
            }
            if (cellsCandidate.Count >= COLLAPSE_COUNT - 1) {
                cellsToDelete.AddRange(cellsCandidate);
            }
            cellsCandidate.Clear();
            lanes.Clear();

            // diagonals
            List<Vector2Int> diagonalIncrements = new List<Vector2Int>();
            diagonalIncrements.Add(new Vector2Int(+1, +1));
            diagonalIncrements.Add(new Vector2Int(-1, -1));

            foreach (Vector2Int inc in diagonalIncrements) {
                bool done = false;
                for (Vector2Int cell = new Vector2Int(cellStart.x + inc.x, cellStart.y + inc.y); gamePosition.valid(cell) && !done; cell = new Vector2Int(cell.x + inc.x, cell.y + inc.y)) {
                    Ball b = gamePosition.get(cell);
                    if (b == null || b.color != ball.color) {
                        done = true;
                    } else if (b.color == ball.color) {
                        cellsCandidate.Add(cell);
                    }
                }
            }
            if (cellsCandidate.Count >= COLLAPSE_COUNT - 1) {
                cellsToDelete.AddRange(cellsCandidate);
            }
            cellsCandidate.Clear();
            diagonalIncrements.Clear();

            diagonalIncrements.Add(new Vector2Int(+1, -1));
            diagonalIncrements.Add(new Vector2Int(-1, +1));
            foreach (Vector2Int inc in diagonalIncrements) {
                bool done = false;
                for (Vector2Int cell = new Vector2Int(cellStart.x + inc.x, cellStart.y + inc.y); gamePosition.valid(cell) && !done; cell = new Vector2Int(cell.x + inc.x, cell.y + inc.y)) {
                    Ball b = gamePosition.get(cell);
                    if (b == null || b.color != ball.color) {
                        done = true;
                    } else if (b.color == ball.color) {
                        cellsCandidate.Add(cell);
                    }
                }
            }
            if (cellsCandidate.Count >= COLLAPSE_COUNT - 1) {
                cellsToDelete.AddRange(cellsCandidate);
            }
            cellsCandidate.Clear();
            diagonalIncrements.Clear();

            // destroy all balls found
            if (cellsToDelete.Count > 0) {
                // Add the original ball that is causing the collapse
                cellsToDelete.Insert(0, cellStart);
                foreach (Vector2Int cell in cellsToDelete) {
                    Ball b = gamePosition.get(cell);
                    if (b != null) {
                        Ball.Destroy(b.gameObject);
                        gamePosition.set(cell, null);
                    }
                }
            }
        } catch (KeyNotFoundException) {
            // Possible when the ball was already collapsed via another ball
        }

        return cellsToDelete.Count;
    }
}
