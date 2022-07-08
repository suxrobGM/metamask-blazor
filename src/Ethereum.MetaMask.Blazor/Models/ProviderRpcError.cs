namespace Ethereum.MetaMask.Blazor.Models;

public class ProviderRpcError
{
    public string Message { get; set; }
    public int Code { get; set; }
    public object Data { get; set; }
}