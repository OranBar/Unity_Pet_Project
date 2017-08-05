using System;

namespace TonRan.Continuum
{
	[Serializable]
	internal class ContinuumNotInitializedException : Exception
	{
		public ContinuumNotInitializedException() : base("Continuum sense was not initialized. Please call the method Init()")
		{

		}

		public ContinuumNotInitializedException(string msg) : base(msg)
		{

		}
	} 
}