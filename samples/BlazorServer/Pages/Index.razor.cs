using System;
using System.Threading.Tasks;
using Ethereum.MetaMask.Blazor;
using Ethereum.MetaMask.Blazor.Exceptions;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.Pages;

public partial class Index : IDisposable
{
    [Inject]
    public IMetaMaskService MetaMask { get; set; }

    public bool HasMetaMask { get; set; }
    public string SelectedAccount { get; set; }
    public string SelectedChain { get; set; }
    public string RpcResult { get; set; }
    public string Error { get; set; }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (!firstRender)
            return;

        Error = string.Empty;
        HasMetaMask = await MetaMask.IsMetaMaskAvailableAsync();

        var isSiteConnected = await MetaMask.IsSiteConnectedAsync();
        if (isSiteConnected)
        {
            await GetSelectedAddress();
            await GetSelectedNetwork();
        }

        MetaMask.AccountsChanged += OnAccountsChanged;
        MetaMask.ChainChanged += OnChainChanged;
        StateHasChanged();
    }

    private void OnAccountsChanged(object sender, string[] accounts)
    {
        SelectedAccount = accounts[0];
        StateHasChanged();
    }

    private void OnChainChanged(object sender, string chainId)
    {
        SelectedChain = chainId;
        StateHasChanged();
    }

    public async Task ConnectMetaMask()
    {
        try
        {
            SelectedAccount = await MetaMask.ConnectAsync();
        }
        catch (MetaMaskException ex)
        {
            Error = ex.Message;
        }
    }

    public async Task GetSelectedAddress()
    {
        try
        {
            SelectedAccount = await MetaMask.GetSelectedAccountAsync();
        }
        catch (MetaMaskException ex)
        {
            Error = ex.Message;
        }
    }

    public async Task GetSelectedNetwork()
    {
        try
        {
            SelectedChain = await MetaMask.GetSelectedChainAsync();
        }
        catch (MetaMaskException ex)
        {
            Error = ex.Message;
        }
    }

    public async Task GenericRpc()
    {
        try
        {
            var result = await MetaMask.RequestRpcAsync("eth_requestAccounts");
            RpcResult = $"RPC result: {result}";
        }
        catch (MetaMaskException ex)
        {
            Error = ex.Message;
        }
    }

    public void Dispose()
    {
        MetaMask.AccountsChanged -= OnAccountsChanged;
        MetaMask.ChainChanged -= OnChainChanged;
    }
}