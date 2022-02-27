using System;
using System.Collections.Generic;
using UnityEngine;

namespace Project.Scripts.Game.Git
{
    [Serializable]
    public class GitStorage
    {
        public static string GitMainDataPath = $"{Application.persistentDataPath}/repo.git";
        //public static string GitStatesDataPath = $"{Application.persistentDataPath}/states.git";

        [SerializeField] public List<GitNode> Commits = new List<GitNode>();
        [SerializeField] public GitLastState LastState;
        [SerializeField] public List<int> Branches = new List<int>();
        [SerializeField] public int Head = 0;
    }
}