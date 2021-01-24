using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lewisstupidthingy
{
	//could probably be more elegantly written but meh ¯\_(ツ)_/¯
	public class Segment
	{
		public string sampleString { get; set; }
		public List<string> subSegments { get; set; }
		public bool numbered { get; set; }
		public bool isSubSegment;
		public string numberedText { get; set; }

		public Segment(string rawSegment, List<Segment> availableSubSegments)
		{
			subSegments = new List<string>();
			//none numbered
			if (rawSegment[0] == ':')
			{
				numbered = false;
				numberedText = "";
				rawSegment = rawSegment.Substring(1);
			}
			else
			{
				numbered = true;
				numberedText = "# ";
			}

			//subseg list
			if (rawSegment[0] == '@')
			{
				isSubSegment = true;
			}
			else
			{
				isSubSegment = false;
			}


			char[] charSeparators = new char[] { '|' };
			string[] subsegments = rawSegment.Split(charSeparators);

			foreach(string subseg in subsegments)
			{
				Console.WriteLine(subseg);
				string[] stringSeperators = new string[] { "@@" };
				string[] split = subseg.Split(stringSeperators, StringSplitOptions.None);
				try
				{
					split[1] = availableSubSegments[int.Parse(split[1])].sampleString;
				}
				catch(Exception excpt)
				{
					Console.WriteLine("couldn't parse");
				}
				subSegments.Add(string.Concat(split));
			}
			sampleString = string.Concat(subSegments);
			
		}

		private string insertAvailable(string subseg)
		{
			return "";
		}


		public override string ToString()
		{
			return sampleString;
		}
	}
}
