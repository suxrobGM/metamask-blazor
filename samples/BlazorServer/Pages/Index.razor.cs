using System;
using System.Threading.Tasks;
using Ethereum.MetaMask.Blazor;
using Microsoft.AspNetCore.Components;

namespace BlazorServer.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        public IMetaMaskService MetaMask { get; set; }

        public bool HasMetaMask { get; set; }
        public string SelectedAccount { get; set; }
        public string SelectedChain { get; set; }
        public string RpcResult { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
                return;

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
            SelectedAccount = await MetaMask.ConnectAsync();
        }

        public async Task GetSelectedAddress()
        {
            SelectedAccount = await MetaMask.GetSelectedAccountAsync();
        }

        public async Task GetSelectedNetwork()
        {
            SelectedChain = await MetaMask.GetSelectedChainAsync();
        }

        public async Task GenericRpc()
        {
            var result = await MetaMask.RequestRpcAsync("eth_requestAccounts");
            RpcResult = $"RPC result: {result}";
        }

        public void Dispose()
        {
            MetaMask.AccountsChanged -= OnAccountsChanged;
            MetaMask.ChainChanged -= OnChainChanged;
        }
    }
}