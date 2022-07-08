using System;
using Ethereum.MetaMask.Blazor.Models;

namespace Ethereum.MetaMask.Blazor;

/// <summary>
/// MetaMask events.
/// </summary>
public interface IMetaMaskEvents
{
    /// <summary>
    /// Event when user changes account.
    /// </summary>
    event EventHandler<string[]>? AccountsChanged;

    /// <summary>
    /// Event when user changes chain.
    /// </summary>
    event EventHandler<string>? ChainChanged;

    /// <summary>
    /// Event when it receives some message that the consumer should be notified of.
    /// The kind of message is identified by the type string.
    /// </summary>
    event EventHandler<ProviderMessage>? MessageReceived;

    /// <summary>
    /// Event when user connects to MetaMask.
    /// </summary>
    event EventHandler<ConnectInfo>? Connect;

    /// <summary>
    /// Event when user disconnects from MetaMask.
    /// </summary>
    event EventHandler<ProviderRpcError>? Disconnect;

    /// <summary>
    /// Raises <see cref="AccountsChanged"/> event.
    /// </summary>
    /// <param name="accounts">Accounts list</param>
    void OnAccountsChanged(string[] accounts);

    /// <summary>
    /// Raises <see cref="ChainChanged"/> event.
    /// </summary>
    /// <param name="chainId">Current chain ID</param>
    void OnChainChanged(string chainId);

    /// <summary>
    /// Raises <see cref="MessageReceived"/> event.
    /// </summary>
    /// <param name="message">Provider message.</param>
    void OnMessageReceived(ProviderMessage message);

    /// <summary>
    /// Raises <see cref="Connect"/> event.
    /// </summary>
    /// <param name="connectInfo">Connection info.</param>
    void OnConnect(ConnectInfo connectInfo);

    /// <summary>
    /// Raises <see cref="Disconnect"/> event.
    /// </summary>
    /// <param name="rpcError">RPC error description with code.</param>
    void OnDisconnect(ProviderRpcError rpcError);
}