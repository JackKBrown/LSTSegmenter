using System;
using System.Collections.Generic;
using System.IO;

namespace lewisstupidthingy
{
	public class FileIO
	{
		public FileIO()
		{
		}

		public List<string> parseInput(string path)
		{
			List<string> nameRawPairs = new List<string>();

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
						nameRawPairs.Add(s);
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
