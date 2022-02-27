using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using Project.Scripts.Game.UI;
using UnityEngine;

namespace Project.Scripts.Game.Git
{
    public class GitInstance
    {
        private Field _field;

        private GitStorage _gitStorage;

        public bool IsRedoStateEnded { get; private set; } = true;

        public GitStorage Storage => _gitStorage;
        
        public GitInstance(Field field)
        {
            _field = field;

            _gitStorage = new GitStorage();
            _gitStorage.LastState = new GitLastState(field);
            _gitStorage.Branches.Add(_gitStorage.Head);
            _gitStorage.Commits.Add(new GitNode(0, null, "root", _gitStorage.LastState, field));

            if (File.Exists(GitStorage.GitMainDataPath))
            {
                Stream stream = File.Open(GitStorage.GitMainDataPath, FileMode.Open);

                BinaryFormatter formatter = new BinaryFormatter();

                _gitStorage = (GitStorage)formatter.Deserialize(stream);
                stream.Close();

                Reset();
                
                Debug.Log($"Loaded from: {GitStorage.GitMainDataPath}");
            }
        }

        public void Serialize()
        {
            _gitStorage.LastState = new GitLastState(_field);
            
            Stream stream = File.Open(GitStorage.GitMainDataPath, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, _gitStorage);
            stream.Close();
        }
        
        public string Commit(string msg)
        {
            var parent = _gitStorage.Commits[_gitStorage.Head];
            var newId = _gitStorage.Commits.Count;

            _gitStorage.Commits.Add(new GitNode(newId, parent, msg, _gitStorage.LastState, _field));
            
            parent.AddChild(_gitStorage.Commits[newId]);

            if (parent.Childs.Count == 1)
            {
                _gitStorage.Branches.Remove(_gitStorage.Head);
            }
            _gitStorage.Head = _gitStorage.Commits.Count - 1;
            _gitStorage.Branches.Add(_gitStorage.Head);
            
            Serialize();

            string branches = "";
            
            _gitStorage.Branches.ForEach(branch => branches += $"{branch}, ");
            
            return $"Success. Commit id: {_gitStorage.Commits.Count - 1} " +
                   $"Cur head: {_gitStorage.Commits.Count - 1} " +
                   $"Branches: {branches}";
        }

        public string Reset()
        {
            PlayerPrefs.SetInt("player_score", _gitStorage.LastState.Score);
            _field.InstantiateItemsFrom(_gitStorage.LastState.Cells);
            return "Success. Reset all unsaved changes.";
        }
        
        public List<string> Log()
        {
            var list = new List<string>();
            list.Add($"Head: {_gitStorage.Head}");
            _gitStorage.Commits.ForEach(commit => list.Add(commit.ToString()));
            return list;
        }

        public List<string> Undo()
        {
            var list = new List<string>();
            list.Add(Reset());

            var curNode = _gitStorage.Commits[_gitStorage.Head];
            
            if (curNode.ParentId != -1)
            {
                curNode.UndoChanges(_field);
                _gitStorage.Head = curNode.ParentId;
                list.Add("Success. Undo applied");
            }
            else
            {
                list.Add($"You have reached the root of the tree.");
            }
            
            Serialize();

            return list;
        }
        
        public List<string> PreRedo() {
            var list = new List<string>();
            list.Add(Reset());
            
            var curNode = _gitStorage.Commits[_gitStorage.Head];

            if (curNode.Childs.Count > 1)
            {
                IsRedoStateEnded = false;
                list.Add($"You have more than one child commit. " +
                         $"Please select one of them. " +
                         $"Type number of one of these rows and then press \"Redo\" button: ");

                for (var index = 0; index < curNode.Childs.Count; index++)
                {
                    var curNodeChild = curNode.Childs[index];
                    list.Add($"{index + 1} : MSG : {_gitStorage.Commits[curNodeChild].CommitText}");
                }
            }
            else if (curNode.Childs.Count == 0)
            {
                list.Add($"You have reached the leaf of the tree.");
            }
            else
            {
                _gitStorage.Head = curNode.Childs[0];
                _gitStorage.Commits[_gitStorage.Head].ApplyChanges(_field);
                Serialize();
                list.Add($"Success. Redo applied.");
            }

            return list;
        }
        
        public List<string> AfterRedo(int index = 0) {
            var list = new List<string>();
            list.Add(Reset());
            
            var curNode = _gitStorage.Commits[_gitStorage.Head];
            _gitStorage.Head = curNode.Childs[index];
            _gitStorage.Commits[_gitStorage.Head].ApplyChanges(_field);
            list.Add($"Success. Redo applied with commit: {_gitStorage.Commits[_gitStorage.Head].CommitText}.");
            Serialize();
            return list;
        }
        
        public List<string> Redo(int index = 0) {
            var list = new List<string>();
            
            if (!IsRedoStateEnded)
            {
                list.AddRange(AfterRedo(index));
                IsRedoStateEnded = true;
                return list;
            }
            
            if (IsRedoStateEnded)
            {
                list.AddRange(PreRedo());
            }
            
            return list;
        }
    }
}