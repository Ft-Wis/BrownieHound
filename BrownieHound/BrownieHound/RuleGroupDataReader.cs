using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace BrownieHound
{
    public class RuleGroupDataReader
    {
        public static List<ruleGroupData> Read(string path)
        {
            List<ruleGroupData> data = new List<ruleGroupData>();
            if (!Directory.Exists(path))
            {
                Directory.CreateDirectory(path);
            }
            DirectoryInfo di = new DirectoryInfo(path);
            IEnumerable<FileInfo> ruleFiles = di.EnumerateFiles("*.txt", SearchOption.TopDirectoryOnly).OrderBy(f => f.CreationTime).ToList();
            foreach (var ruleFile in ruleFiles.Select((Value, Index) => new { Value, Index }))
            {
                string ruleGroupName = ruleFile.Value.Name.Remove(ruleFile.Value.Name.Length - 4);
                ruleGroupData ruleGroup = new ruleGroupData(ruleFile.Index, ruleGroupName);
                string filePath = $"{path}\\{ruleFile.Value.Name}";
                try
                {
                    using (StreamReader sr = new StreamReader(filePath, Encoding.GetEncoding("UTF-8")))
                    {
                        while (sr.Peek() != -1)
                        {
                            ruleGroup.ruleSet(sr.ReadLine());
                        }
                    }
                }
                catch
                {
                    // エラーハンドリングを追加
                }
                data.Add(ruleGroup);
            }
            return data;
        }
    }
}
