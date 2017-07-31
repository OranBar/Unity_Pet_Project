using System;
using System.Runtime.Serialization;

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