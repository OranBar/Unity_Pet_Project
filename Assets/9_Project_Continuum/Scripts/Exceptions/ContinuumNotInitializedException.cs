using System;
using System.Runtime.Serialization;

[Serializable]
internal class ContinuumNotInitializedException : Exception
{
	public ContinuumNotInitializedException() : base("Continuum sense was not initialized. Please call the method Init()")
	{
		
	}
}