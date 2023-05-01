using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class LeaderBoard {
    private const float margin = 0.2f;

    private TextMeshPro[] scoreLines;
    private List<int> topScores = new List<int>();

    List<HighScoreEntry> scores;

    XMLManager xmlManager = new XMLManager();

    public LeaderBoard(GameObject prefab, Board board) {
        CameraHelper cameraHelper = new CameraHelper(board.boardDimension, board.boardDimension);
        Vector3 position = cameraHelper.cellToCameraUnchecked(new Vector2Int(board.boardDimension + 1, 5), 0);
        GameObject scoreBoardinstance = Board.Instantiate(prefab, position, Quaternion.identity, parent: board.transform);
        scoreBoardinstance.name = "TopScoreBoard";
        scoreLines = scoreBoardinstance.GetComponentsInChildren<TextMeshPro>();
        Renderer renderer = scoreBoardinstance.GetComponent<Renderer>();
        if (scoreLines != null) {
            Vector3 parentPosition = scoreBoardinstance.transform.position;

            float vertical_delta = (renderer.bounds.extents.y - margin) / scoreLines.Length *2f;
            float top = parentPosition.y + (scoreLines.Length -1) /2f * vertical_delta;

            for (int i = 0; i < scoreLines.Length; i++) {
                scoreLines[i].text = "";
                Vector3 pos = scoreLines[i].transform.position;
                scoreLines[i].transform.position = new Vector3(pos.x, top - vertical_delta*i, pos.z);
            }
            
            Load();
            if (topScores.Count == 0) {
                topScores.Add(0);
            }
            renderTopScores(topScores);
        }
    }

    private void renderTopScores(List<int> topScores, int currentScore = -1) {
        topScores.Sort();
        for (int i = 0; i < scoreLines.Length; i++) {
            int j = topScores.Count - 1 - i;
            if (j < 0) {
                break;
            }
            if (currentScore >= 0 && topScores[j] == currentScore) {
                scoreLines[i].text = "<b>" + topScores[j].ToString() + "</b>";
            } else {
                scoreLines[i].text = topScores[j].ToString();
            }
        }
    }

    public void registerFinalScore(int finalScore) {
        if (finalScore > 0) {
            topScores.Add(finalScore);
            renderTopScores(topScores);
        }
    }

    public void updateTopScore(int scoreRunning) {
        List<int> topScoresRunning = new List<int>(topScores);
        if (topScoresRunning.Count < scoreLines.Length) {
            if (topScoresRunning.Count > 0 && topScoresRunning[0] == 0) {
                topScoresRunning.RemoveAt(0);
            }
            topScoresRunning.Add(scoreRunning);
            renderTopScores(topScoresRunning, scoreRunning);
        } else if (topScoresRunning[0] <= scoreRunning) {
            topScoresRunning[0] = scoreRunning;
            renderTopScores(topScoresRunning, scoreRunning);
        }

        Save(topScoresRunning);
    }


    void Load() {
        scores = xmlManager.LoadScores();

        topScores = new List<int>();
        foreach (var scoreEntry in scores) {
            topScores.Add(scoreEntry.score);
        }
    }

    void Save(List<int> topScores) {
        List<HighScoreEntry> newScores = new List<HighScoreEntry>();
        foreach (int score in topScores) {
            if (score != 0) {
                newScores.Add(new HighScoreEntry("Andre", score));
            }
        }

        xmlManager.SaveScores(newScores);

        PlayerPrefs.SetString("forceSaveWorkaround", string.Empty);
        PlayerPrefs.Save();
    }
}
