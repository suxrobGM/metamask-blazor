using System;

namespace Ethereum.MetaMask.Blazor.Exceptions;

[Serializable]
public class MetaMaskException : Exception
{
	public MetaMaskException() { }
	public MetaMaskException(string message) : base(message) { }
	public MetaMaskException(string message, Exception inner) : base(message, inner) { }
	protected MetaMaskException(
	  System.Runtime.Serialization.SerializationInfo info,
	  System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
}
