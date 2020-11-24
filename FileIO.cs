using System;
using System.Collections.Generic;
using System.IO;

namespace lewisstupidthingy
{
	class FileIO
	{
		public FileIO()
		{
		}

		public Dictionary<string, string> parseInput(string path)
		{
			Dictionary<string, string> nameRawPairs = new Dictionary<string, string>();

			if(File.Exists(path))
			{
				using (StreamReader sr = File.OpenText(path))
				{
					string s;
					while ((s = sr.ReadLine()) != null)
					{
						// lines that start with # are treat as comments
						if (string.IsNullOrEmpty(s)) continue;
						if (s[0] == '#') continue;
						string[] line = s.Split(new char[] { '-' }, 2);
						if (line.Length != 2)
						{
							Console.WriteLine("couldn't get name and segment pair skipping line");
							continue;
						}
						nameRawPairs.Add(line[0].Trim(), line[1].Trim());
					}
				}
			}

			return nameRawPairs;
		}

		public bool WriteOutput(string path, string[] outlines)
		{
			//TODO maybe some checking to make sure the file doesn't already exist?
			using (StreamWriter sw = File.CreateText(path))
			{
				foreach(string line in outlines)
				{
					sw.WriteLine(line);
				}
			}
			return true;
		}
	}
}
