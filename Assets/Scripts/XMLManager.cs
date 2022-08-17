using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;
using System.IO;
public class XMLManager /*: MonoBehaviour*/ {
    private Leaderboard leaderboard;
    public XMLManager() {
        if (!Directory.Exists(Application.persistentDataPath + "/HighScores/")) {
            Directory.CreateDirectory(Application.persistentDataPath + "/HighScores/");
        }
    }
    public void SaveScores(List<HighScoreEntry> scoresToSave) {
        leaderboard.list = scoresToSave;
        XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
        FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Create);
        serializer.Serialize(stream, leaderboard);
        stream.Close();
    }
    public List<HighScoreEntry> LoadScores() {
        if (File.Exists(Application.persistentDataPath + "/HighScores/highscores.xml")) {
            XmlSerializer serializer = new XmlSerializer(typeof(Leaderboard));
            FileStream stream = new FileStream(Application.persistentDataPath + "/HighScores/highscores.xml", FileMode.Open);
            leaderboard = serializer.Deserialize(stream) as Leaderboard;
        }
        if (leaderboard == null) {
            leaderboard = new Leaderboard();
        }

        return leaderboard.list;
    }
}

public class Leaderboard {
    public List<HighScoreEntry> list = new List<HighScoreEntry>();
}
