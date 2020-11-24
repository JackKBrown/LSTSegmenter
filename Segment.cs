using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lewisstupidthingy
{
	//could probably be more elegantly written but meh ¯\_(ツ)_/¯
	class Segment
	{
		public string name { get; set; }
		public string sampleString { get; set; }
		public List<string> subSegments { get; set; }



		public Segment(string new_name, string rawSegment)
		{
			char[] charSeparators = new char[] { '[', ']' };
			string[] subsegments = rawSegment.Split(charSeparators);

			name = new_name;
			sampleString = rawSegment;
			subSegments = new List<string>(subsegments);
		}



		public override string ToString()
		{
			return name;
		}
	}
}
