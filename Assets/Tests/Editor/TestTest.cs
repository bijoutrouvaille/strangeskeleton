using NUnit.Framework;
using System;

namespace sci.tests
{
		[TestFixture()]
		public class TestTest
		{
				[TestCase(3, Result = 3)]
				public int TestCase(int x)
				{
						return x;
			
					
				}
		}
}
	


