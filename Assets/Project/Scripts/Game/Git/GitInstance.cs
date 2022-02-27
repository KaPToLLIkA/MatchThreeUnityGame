using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Project.Scripts.Game.UI;
using UnityEngine;

namespace Project.Scripts.Game.Git
{
    public class GitInstance
    {
        private Field _field;

        private GitStorage _gitStorage;

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
                
                PlayerPrefs.SetInt("player_score", _gitStorage.LastState.Score);
                
                _field.InstantiateItemsFrom(_gitStorage.LastState.Cells);
            }
        }

        public string Commit(string msg)
        {
            var parent = _gitStorage.Commits[_gitStorage.Head];
            var newId = _gitStorage.Commits.Count;

            _gitStorage.Commits.Add(new GitNode(newId, parent, msg, _gitStorage.LastState, _field));
            
            parent.AddChild(_gitStorage.Commits[newId]);
            
            _gitStorage.Branches.Remove(_gitStorage.Head);
            _gitStorage.Head = _gitStorage.Commits.Count - 1;
            _gitStorage.Branches.Add(_gitStorage.Head);
            
            _gitStorage.LastState = new GitLastState(_field);
            
            Stream stream = File.Open(GitStorage.GitMainDataPath, FileMode.Create);

            BinaryFormatter formatter = new BinaryFormatter();

            formatter.Serialize(stream, _gitStorage);
            stream.Close();

            string branches = "";
            
            _gitStorage.Branches.ForEach(branch => branches += $"{branch}, ");
            
            return $"Success. Commit id: {_gitStorage.Commits.Count - 1} " +
                   $"Cur head: {_gitStorage.Commits.Count - 1} " +
                   $"Branches: {branches}";
        }

        public List<string> Log()
        {
            var list = new List<string>();
            _gitStorage.Commits.ForEach(commit => list.Add(commit.ToString()));
            return list;
        }
    }
}