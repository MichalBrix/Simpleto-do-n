using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Simpletodon.Data
{

    public class ProgramInitData
    {
        public List<string> LastFiles { get; set; } = new List<string>();
        public string? LatestFile { get; set; } = null;

        public void AddFile(string file)
        {
            if (LastFiles.Contains(file))
            {
                LastFiles.Remove(file);
            }
            LastFiles.Insert(0, file);
            while (LastFiles.Count > 5)
            {
                LastFiles.RemoveAt(LastFiles.Count - 1);
            }
            LatestFile = file;
        }

        public void Save()
        {
            string json = JsonSerializer.Serialize(this);
            System.IO.File.WriteAllText("Simpletodon.config", json);
        }

        public static ProgramInitData Load()
        {
            if (System.IO.File.Exists("Simpletodon.config"))
            {
                string json = System.IO.File.ReadAllText("Simpletodon.config");
                var possibleData = JsonSerializer.Deserialize<ProgramInitData>(json);
                if (possibleData != null)
                {
                    return possibleData;
                }
            }
            return new ProgramInitData();
        }
    }
}
