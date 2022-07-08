using System;
using System.Numerics;
using System.Threading.Tasks;

namespace Ethereum.MetaMask.Blazor;

/// <summary>
/// Interface of the MetaMask API.
/// </summary>
public interface IMetaMaskService : IMetaMaskEvents, IAsyncDisposable
{
    /// <summary>
    /// Checks whether MetaMask available or not.
    /// </summary>
    /// <returns>True if available, otherwise false.</returns>
    ValueTask<bool> IsMetaMaskAvailableAsync();

    /// <summary>
    /// Checks whether the site is connected to MetaMask or not.
    /// </summary>
    /// <returns>True if site connected, otherwise false.</returns>
    ValueTask<bool> IsSiteConnectedAsync();

    /// <summary>
    /// Connects to the wallet.
    /// </summary>
    /// <returns>Account number in hexadecimal format.</returns>
    ValueTask<string> ConnectAsync();
    
    /// <summary>
    /// Requests to change account
    /// </summary>
    /// <returns>Account number in hexadecimal format.</returns>
    ValueTask<string> ChangeAccountAsync();

    /// <summary>
    /// Gets current selected address.
    /// </summary>
    /// <returns>Account number in hexadecimal format.</returns>
    ValueTask<string> GetSelectedAccountAsync();

    /// <summary>
    /// Gets current selected chain.
    /// </summary>
    /// <returns>Chain number in hexadecimal format.</returns>
    ValueTask<string> GetSelectedChainAsync();

    /// <summary>
    /// Gets balance of the account of the given address.
    /// </summary>
    /// <param name="address">Address in hexadecimal format.
    /// The default value (null) is currently selected address.</param>
    /// <returns>Integer of the current balance in wei.</returns>
    ValueTask<BigInteger> GetBalanceAsync(string address = null);

    /// <summary>
    /// Gets balance of the custom token.
    /// </summary>
    /// <param name="tokenAddress">Token or contract address.</param>
    /// <param name="account">Wallet address.
    ///     Default value is currently selected address in MetaMask.</param>
    /// <returns>Integer of the token balance in wei.</returns>
    ValueTask<BigInteger> GetTokenBalanceAsync(string tokenAddress, string account = null);

    /// <summary>
    /// Creates a new message call transaction or a contract creation, if the data field contains code.
    /// </summary>
    /// <param name="to">Receiver account number in hexadecimal format.</param>
    /// <param name="value">Integer of the value sent with this transaction.</param>
    /// <param name="data">The compiled code of a contract OR the hash of the invoked method signature and encoded parameters.</param>
    /// <returns>Transaction hash in hexadecimal format.</returns>
    ValueTask<string> SendTransactionAsync(string to, BigInteger value, string data);

    /// <summary>
    /// Make a generic ethereum RPC request. See available methods https://eth.wiki/json-rpc/API
    /// </summary>
    /// <param name="method">RPC method name.</param>
    /// <param name="args">Parameters.</param>
    /// <returns>Ethereum RPC response message.</returns>
    ValueTask<dynamic> RequestRpcAsync(string method, params object[] args);
}