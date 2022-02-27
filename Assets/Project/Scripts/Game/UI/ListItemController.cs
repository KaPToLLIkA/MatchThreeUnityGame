using System;
using TMPro;
using UnityEngine;

namespace Project.Scripts.Game.UI
{
    [RequireComponent(typeof(TMP_Text))]
    public class ListItemController : MonoBehaviour
    {
        public void SetText(string text)
        {
            var field = GetComponent<TMP_Text>();
            field.text = text;
        }
    }
}