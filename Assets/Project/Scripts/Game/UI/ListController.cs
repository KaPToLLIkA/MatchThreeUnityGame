using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Game.UI
{
    public class ListController : MonoBehaviour
    {
        [SerializeField]
        private GameObject contentArea;
        [SerializeField]
        private GameObject listItemPrefab;

        private List<GameObject> allItems = new List<GameObject>();

        public void Clear()
        {
            allItems.ForEach(Destroy);
            allItems.Clear();
        }

        public void AddRow(string row)
        {
            var newItem = Instantiate(listItemPrefab, contentArea.transform);
            newItem.GetComponent<ListItemController>().SetText(row);
            allItems.Add(newItem);
        }

    }
}