using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Project.Scripts.Game.Git
{
    [Serializable]
    public class GitDifferenceEntry
    {
        [SerializeField] private int _x;
        [SerializeField] private int _y;
        [SerializeField] private CellType _from;
        [SerializeField] private CellType _to;

        public int x => _x;
        public int y => _y;
        public CellType From => _from;
        public CellType To => _to;
        
        public GitDifferenceEntry(int x, int y, CellType from, CellType to)
        {
            _x = x;
            _y = y;
            _from = from;
            _to = to;
        }
    }
    
    [Serializable]
    public class GitNode
    {
        [SerializeField] private int _id;
        [SerializeField] private int _parentId;
        [SerializeField] private int _scoreDiff;
        [SerializeField] private string _commitText;
        [SerializeField] private List<GitDifferenceEntry> _difference;
        [SerializeField] private List<int> _childs;
        public int Id => _id;
        public int ParentId => _parentId;

        public List<int> Childs => _childs;

        public string CommitText => _commitText;

        public GitNode(int id, GitNode parent, string msg, GitLastState lastState, Field curState)
        {
            _childs = new List<int>();
            _difference = new List<GitDifferenceEntry>();
            _id = id;
            _parentId = parent == null ? -1 : parent.Id;
            _commitText = msg;

            _scoreDiff = PlayerPrefs.GetInt("player_score", 0) - lastState.Score;

            for (int y = 0; y < curState.Rows; ++y)
            {
                for (int x = 0; x < curState.Columns; ++x)
                {
                    if (curState[x, y] != null && lastState[x, y] != curState[x, y].Type)
                    {
                        _difference.Add(new GitDifferenceEntry(x, y, lastState[x, y], curState[x, y].Type));
                        Debug.Log($"DIF: {x} {y} {lastState[x, y]} {curState[x, y].Type}");
                    }
                }
            }
        }

        public void ApplyChanges(Field field)
        {
            int curScore = PlayerPrefs.GetInt("player_score", 0);
            PlayerPrefs.SetInt("player_score", curScore + _scoreDiff);
            
            _difference.ForEach(diff => field.ChangeItemAt(diff.x, diff.y, diff.To));
        }

        public void UndoChanges(Field field)
        {
            int curScore = PlayerPrefs.GetInt("player_score", 0);
            PlayerPrefs.SetInt("player_score", curScore - _scoreDiff);
            
            _difference.ForEach(diff => field.ChangeItemAt(diff.x, diff.y, diff.From));
        }

        public void AddChild(GitNode child)
        {
            _childs.Add(child.Id);
        }

        public override string ToString()
        {
            string childs = "";
            _childs.ForEach(child => childs += $"{child}, ");
            return $"ID: {_id} CHANGES: {_difference.Count} {_scoreDiff} TEXT: {_commitText} CHILDS: {childs}";
        }
    }
}