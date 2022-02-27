using System;
using Project.Scripts.Game.Git;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Game.UI
{
    public class MenuController : MonoBehaviour
    {
        public GameObject overlay;
        public ListController listController;
        public GameController gameController;
        public TMP_InputField inputField;

        private GitInstance _git;

        private void Start()
        {
            _git = new GitInstance(gameController.Field);
        }

        public void ShowMenu()
        {
            if (gameController.Field.ContainsAnyEmpty() || gameController.Field.ContainsAnyMovable()) return;
            gameController.CellSelector.CanSelect = false;
            overlay.SetActive(true);
        }

        public void HideMenu()
        {
            gameController.CellSelector.CanSelect = true;
            overlay.SetActive(false);
        }

        public void Commit()
        {
            var msg = inputField.text == "" ? "No message" : inputField.text;

            listController.Clear();
            listController.AddRow(_git.Commit(msg));
        }

        public void Log()
        {
            listController.Clear();
            _git.Log().ForEach(listController.AddRow);
        }
    }
}