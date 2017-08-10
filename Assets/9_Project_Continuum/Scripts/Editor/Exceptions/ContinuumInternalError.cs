using System;
using System.Runtime.Serialization;
using Debug = TonRan.Continuum.Continuum_ImmediateDebug;

[Serializable]
internal class ContinuumInternalError : Exception
{
	public ContinuumInternalError()
	{
	}

	public ContinuumInternalError(string message) : base(message)
	{
	}

	public ContinuumInternalError(string message, Exception innerException) : base(message, innerException)
	{
	}

	protected ContinuumInternalError(SerializationInfo info, StreamingContext context) : base(info, context)
	{
	}
}