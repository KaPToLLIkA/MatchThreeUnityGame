using System;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Game.UI
{
    public class ScoreManager : MonoBehaviour
    {
        public TMP_Text score;
        public string scorePattern = "Score: {0}";
        
        private int _curScore;

        private void Update()
        {
            _curScore = PlayerPrefs.GetInt("player_score", 0);
            score.text = String.Format(scorePattern, _curScore);
        }
    }
}