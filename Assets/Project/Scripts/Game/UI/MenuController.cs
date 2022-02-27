using System;
using Project.Scripts.Game.Git;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Project.Scripts.Game.UI
{
    public class MenuController : MonoBehaviour
    {
        public GameObject overlay;
        public ListController listController;
        public GameController gameController;
        public TMP_InputField inputField;

        public GameObject resetButton;
        public GameObject commitButton;
        public GameObject undoButton;
        public GameObject redoButton;
        public GameObject logButton;
        public GameObject exitButton;

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
            var msg = inputField.text == "" ? $"No message {_git.Storage.Commits.Count}" : inputField.text;

            inputField.text = "";
            listController.Clear();
            listController.AddRow(_git.Commit(msg));
        }

        public void Log()
        {
            listController.Clear();
            _git.Log().ForEach(listController.AddRow);
        }

        public void Undo()
        {
            listController.Clear();
            _git.Undo().ForEach(listController.AddRow);
        }

        public void ResetState()
        {
            listController.Clear();
            listController.AddRow(_git.Reset());
        }

        public void Redo()
        {
            listController.Clear();

            if (_git.IsRedoStateEnded)
            {
                _git.Redo().ForEach(listController.AddRow);

                if (!_git.IsRedoStateEnded)
                {
                    resetButton.GetComponent<Button>().enabled = false;
                    undoButton.GetComponent<Button>().enabled = false;
                    logButton.GetComponent<Button>().enabled = false;
                    commitButton.GetComponent<Button>().enabled = false;
                    exitButton.GetComponent<Button>().enabled = false;

                    resetButton.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    undoButton.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    logButton.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    commitButton.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                    exitButton.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 1f);
                }
            }
            else
            {
                var input = inputField.text;

                int index = 0;
                try
                {
                    index = Int32.Parse(input);

                    if (index < 1 || index > _git.Storage.Commits[_git.Storage.Head].Childs.Count)
                    {
                        listController.AddRow($"Wrong index. Try again. Print index into the input field!");
                        inputField.text = "";
                        return;
                    }
                }
                catch
                {
                    listController.AddRow($"Wrong index. Try again. Print index into the input field!");
                    inputField.text = "";
                    return;
                }

                _git.Redo(index - 1).ForEach(listController.AddRow);

                resetButton.GetComponent<Button>().enabled = true;
                undoButton.GetComponent<Button>().enabled = true;
                logButton.GetComponent<Button>().enabled = true;
                commitButton.GetComponent<Button>().enabled = true;
                exitButton.GetComponent<Button>().enabled = true;

                resetButton.GetComponent<Image>().color = Color.white;
                undoButton.GetComponent<Image>().color = Color.white;
                logButton.GetComponent<Image>().color = Color.white;
                commitButton.GetComponent<Image>().color = Color.white;
                exitButton.GetComponent<Image>().color = Color.white;
            }
        }
    }
}