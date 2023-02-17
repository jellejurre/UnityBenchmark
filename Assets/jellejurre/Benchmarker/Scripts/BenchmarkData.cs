using System.Collections.Generic;

public class BenchmarkData
{
	public BenchmarkData(LinkedList<float>[] frameTimes)
	{
		this.frameTimes = frameTimes;
	}

	public LinkedList<float>[] frameTimes;
}