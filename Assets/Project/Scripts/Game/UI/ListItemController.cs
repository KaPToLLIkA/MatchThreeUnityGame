using System;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Game.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class ListItemController : MonoBehaviour
    {
        private TMP_Text field;

        private void Start()
        {
            field = GetComponent<TMP_Text>();
        }

        public void SetText(string text)
        {
            field.text = text;
        }
    }
}