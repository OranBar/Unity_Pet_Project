using System;
using Debug = TonRan.Continuum.Continuum_ImmediateDebug;

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