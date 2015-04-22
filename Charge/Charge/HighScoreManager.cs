using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Charge
{
    /// <summary>
    /// Manages all high score tasks:
    ///     Creating High Score File
    ///     Retrieving High Scores
    ///     Writing Back to High Score File
    /// </summary>
    class HighScoreManager
    {
        //For reading and writing files
        StreamWriter streamWriter;
        //File name of the high scores list
        String fileName = "HighScores.txt";
        //Top scores
        List<Int32> highScores;

        /// <summary>
        /// Sets up the HighScoreManager
        /// Creates a new High Score file if one doesn't already exist
        /// Otherwise, it loads in the stored high scores.
        /// </summary>
        public HighScoreManager()
        {
            if (!File.Exists(fileName))
            {
                streamWriter = new StreamWriter("HighScores.txt");
                for (int i = 0; i < 9; i++)
                    streamWriter.Write("0 ");
                streamWriter.Write("0");
                streamWriter.Close();
            }

            //Processing data in the list of scores
            highScores = new List<Int32>();
            StreamReader file = new StreamReader("HighScores.txt");
            String line = file.ReadLine();
            String[] data = line.Split(' ');
            foreach (String str in data)
            {
                highScores.Add(Convert.ToInt32(str));
            }
            file.Close();
        }

        /// <summary>
        /// Updates the high score list given the final score
        /// Will essentially do nothing if score is not a high score
        /// </summary>
        public void updateHighScore(int finalScore)
        {
            highScores.Add(finalScore);
            highScores.Sort();
            highScores.Reverse();
            if (highScores.Count() >= GameplayVars.NumScores)
            {
                highScores.RemoveAt(GameplayVars.NumScores);
            }
            streamWriter = new StreamWriter("HighScores.txt");
            for (int i = 0; i < highScores.Count() - 1; i++)
                streamWriter.Write(highScores[i] + " ");
            streamWriter.Write(highScores[GameplayVars.NumScores - 1]);
            streamWriter.Close();
        }

        /// <summary>
        /// Returns the rank'th high score
        /// Rank 0 = Highest Score
        /// Rank 1 = Second Highest Score
        /// And so on...
        /// If rank is beyond the recorded high score size, returns 0
        /// </summary>
        /// <param name="rank">The place to check in the high score table</param>
        /// <returns>High score at rank</returns>
        public int getHighScore(int rank)
        {
            if (rank >= highScores.Count()) return 0;
            else return highScores[rank];
        } 

    }
}
