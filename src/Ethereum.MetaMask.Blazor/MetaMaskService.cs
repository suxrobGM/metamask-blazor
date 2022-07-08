using System;
using System.Numerics;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Ethereum.MetaMask.Blazor.Models;

namespace Ethereum.MetaMask.Blazor;

public class MetaMaskService : IMetaMaskService
{
    private readonly Lazy<Task<IJSObjectReference>> _jsModule;
    private readonly ILogger<MetaMaskService> _logger;
    private DotNetObjectReference<MetaMaskService> _dotNetObjRef;
    private bool _createdJsObj;
    private bool _eventsBound;

    public event EventHandler<string[]> AccountsChanged;
    public event EventHandler<string> ChainChanged;
    public event EventHandler<ProviderMessage> Message;
    public event EventHandler<ConnectInfo> Connect;
    public event EventHandler<ProviderRpcError> Disconnect;

    public MetaMaskService(IJSRuntime jsRuntime, ILogger<MetaMaskService> logger = null!)
    {
        if (jsRuntime is null)
            throw new ArgumentNullException(nameof(jsRuntime));

        _jsModule = new Lazy<Task<IJSObjectReference>>(() => LoadScripts(jsRuntime).AsTask());
        _logger = logger;
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
            _logger?.LogError("Thrown exception in MetaMaskService.CreateMetaMaskJsObj(): {Exception}", ex);
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
            _logger?.LogError("Thrown exception in MetaMaskService.SetDotNetObjRef(): {Exception}", ex);
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
            _logger?.LogError("Thrown exception in MetaMaskService.BindEvents(): {Exception}", ex);
        }
    }

    #endregion

    #region Implementation of IMetaMaskService

    public async ValueTask<bool> IsMetaMaskAvailableAsync()
    {
        var module = await _jsModule.Value;
        try
        {
            return await module.InvokeAsync<bool>("isMetaMaskAvailable");
        }
        catch (Exception ex)
        {
            _logger?.LogError("Thrown exception in MetaMaskService.IsMetaMaskAvailableAsync(): {Exception}", ex);
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
        catch (Exception ex)
        {
            _logger?.LogError("Thrown exception in MetaMaskService.IsSiteConnectedAsync(): {Exception}", ex);
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
            _logger?.LogError("Thrown exception in MetaMaskService.ConnectAsync(): {Exception}", ex);
            return string.Empty;
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
            _logger?.LogError("Thrown exception in MetaMaskService.ChangeAccountAsync(): {Exception}", ex);
            return string.Empty;
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
            _logger?.LogError("Thrown exception in MetaMaskService.GetSelectedAccountAsync(): {Exception}", ex);
            return string.Empty;
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
            _logger?.LogError("Thrown exception in MetaMaskService.GetSelectedChainAsync(): {Exception}", ex);
            return string.Empty;
        }
    }

    public async ValueTask<BigInteger> GetBalanceAsync(string address = null)
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
            _logger?.LogError("Thrown exception in MetaMaskService.GetBalanceAsync(): {Exception}", ex);
            return BigInteger.Zero;
        }
    }

    public async ValueTask<BigInteger> GetTokenBalanceAsync(string tokenAddress, string account = null)
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
            _logger?.LogError("Thrown exception in MetaMaskService.GetTokenBalance(): {Exception}", ex);
            return BigInteger.Zero;
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
            _logger?.LogError("Thrown exception in MetaMaskService.SendTransactionAsync(): {Exception}", ex);
            return string.Empty;
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
            _logger?.LogError("Thrown exception in MetaMaskService.RequestRpcAsync(): {Exception}", ex);
            return null;
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
    public void OnMessage(ProviderMessage providerMessage)
    {
        Message?.Invoke(this, providerMessage);
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