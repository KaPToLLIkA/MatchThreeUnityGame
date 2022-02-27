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

        public void ShowMenu()
        {
            gameController.CellSelector.CanSelect = false;
            overlay.SetActive(true);
        }

        public void HideMenu()
        {
            gameController.CellSelector.CanSelect = true;
            overlay.SetActive(false);
        }

        public void ProcessCommand()
        {
            Debug.Log($"Processing {inputField.text}");
            
        }
    }
}