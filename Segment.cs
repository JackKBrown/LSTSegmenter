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

		public Segment(string rawSegment)
		{
			if (rawSegment[0] == ':')
			{
				numbered = false;
				rawSegment = rawSegment.Substring(1);
			}
			else
			{
				numbered = true;
			}
			char[] charSeparators = new char[] { '|' };
			string[] subsegments = rawSegment.Split(charSeparators);

			sampleString = rawSegment;
			subSegments = new List<string>(subsegments);
		}


		public override string ToString()
		{
			return sampleString;
		}
	}
}
