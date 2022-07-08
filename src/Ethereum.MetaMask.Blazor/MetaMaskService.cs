using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.JSInterop;
using Ethereum.MetaMask.Blazor.Models;
using Ethereum.MetaMask.Blazor.Exceptions;

namespace Ethereum.MetaMask.Blazor;

internal class MetaMaskService : IMetaMaskService
{
    private readonly Lazy<Task<IJSObjectReference>> _jsModule;

    private DotNetObjectReference<MetaMaskService>? _dotNetObjRef;
    private bool _createdJsObj;
    private bool _eventsBound;

    public event EventHandler<string[]>? AccountsChanged;
    public event EventHandler<string>? ChainChanged;
    public event EventHandler<ProviderMessage>? MessageReceived;
    public event EventHandler<ConnectInfo>? Connect;
    public event EventHandler<ProviderRpcError>? Disconnect;

    public MetaMaskService(IJSRuntime jsRuntime)
    {
        if (jsRuntime is null)
            throw new ArgumentNullException(nameof(jsRuntime));

        _jsModule = new Lazy<Task<IJSObjectReference>>(() => LoadScripts(jsRuntime).AsTask());
    }

    #region Internal methods

    private ValueTask<IJSObjectReference> LoadScripts(IJSRuntime jsRuntime)
    {
        return jsRuntime.InvokeAsync<IJSObjectReference>("import",
            "./_content/Ethereum.MetaMask.Blazor/MetaMaskService.js");
    }

    private async Task Init()
    {
        await SetDotNetObjRef();
        await CreateMetaMaskJsObj();
        await BindEvents();
    }

    private async ValueTask CreateMetaMaskJsObj()
    {
        if (_createdJsObj)
            return;

        var module = await _jsModule.Value;
        try
        {
            await module.InvokeVoidAsync("createMetaMaskObj");
            _createdJsObj = true;
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not initialize MetaMaskService", ex);
        }
    }

    private async ValueTask SetDotNetObjRef()
    {
        if (_dotNetObjRef != null)
            return;

        var module = await _jsModule.Value;
        try
        {
            _dotNetObjRef = DotNetObjectReference.Create(this);
            await module.InvokeVoidAsync("setDotnetReference", _dotNetObjRef);
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not initialize MetaMaskService", ex);
        }
    }

    private async ValueTask BindEvents()
    {
        if (_eventsBound)
            return;

        var module = await _jsModule.Value;
        try
        {
            await module.InvokeVoidAsync("bindEvents");
            _eventsBound = true;
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not initialize MetaMaskService", ex);
        }
    }

    #endregion

    #region Implementation of the IMetaMaskService

    public async ValueTask<bool> IsMetaMaskAvailableAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            return await module.InvokeAsync<bool>("isMetaMaskAvailable");
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async ValueTask<bool> IsSiteConnectedAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            return await module.InvokeAsync<bool>("isSiteConnected");
        }
        catch (Exception)
        {
            return false;
        }
    }

    public async ValueTask<string> ConnectAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            return await module.InvokeAsync<string>("connect");
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not connect to the wallet", ex);
        }
    }

    public async ValueTask<string> ChangeAccountAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            return await module.InvokeAsync<string>("changeAccount");
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not change the account", ex);
        }
    }

    public async ValueTask<string> GetSelectedAccountAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            return await module.InvokeAsync<string>("getSelectedAccount");
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not get an account address", ex);
        }
    }

    public async ValueTask<string> GetSelectedChainAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            return await module.InvokeAsync<string>("getSelectedChain");
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not get a chain address", ex);
        }
    }

    public async ValueTask<BigInteger> GetBalanceAsync(string address = "")
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            var balanceHex = await module.InvokeAsync<string>("getBalance", address);
            return balanceHex.HexToBigInteger();
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not get an account balance", ex);
        }
    }

    public async ValueTask<BigInteger> GetTokenBalanceAsync(string tokenAddress, string account = "")
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            var balanceHex = await module.InvokeAsync<string>("getTokenBalance", tokenAddress, account);
            return balanceHex.HexToBigInteger();
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not get a token balance", ex);
        }
    }

    public async ValueTask<string> SendTransactionAsync(string to, BigInteger value, string data)
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            return await module.InvokeAsync<string>("sendTransaction", to, value.ToHexString(), data);
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not send transaction to specified address", ex);
        }
    }

    public async ValueTask<dynamic> RequestRpcAsync(string method, params object[] args)
    {
        var module = await _jsModule.Value;
        try
        {
            await Init();
            return await module.InvokeAsync<dynamic>("requestRpc", method, args);
        }
        catch (Exception ex)
        {
            throw new MetaMaskException("Could not make an RPC request", ex);
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (_jsModule.IsValueCreated)
        {
            var module = await _jsModule.Value;
            await module.DisposeAsync();
        }
        _dotNetObjRef?.Dispose();
    }

    #endregion

    #region Event handlers

    [JSInvokable]
    public void OnAccountsChanged(string[] accounts)
    {
        AccountsChanged?.Invoke(this, accounts);
    }

    [JSInvokable]
    public void OnChainChanged(string chainId)
    {
        ChainChanged?.Invoke(this, chainId);
    }

    [JSInvokable]
    public void OnMessageReceived(ProviderMessage providerMessage)
    {
        MessageReceived?.Invoke(this, providerMessage);
    }

    [JSInvokable]
    public void OnConnect(ConnectInfo connectInfo)
    {
        Connect?.Invoke(this, connectInfo);
    }

    [JSInvokable]
    public void OnDisconnect(ProviderRpcError rpcError)
    {
        Disconnect?.Invoke(this, rpcError);
    }

    #endregion
}