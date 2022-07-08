using System;
using System.Threading.Tasks;
using Ethereum.MetaMask.Blazor;
using Microsoft.AspNetCore.Components;

namespace BlazorWasm.Pages
{
    public partial class Index : IDisposable
    {
        [Inject]
        public IMetaMaskService MetaMask { get; set; }

        public bool HasMetaMask { get; set; }
        public string SelectedAccount { get; set; }
        public string SelectedChain { get; set; }
        public string Balance { get; set; }
        public string USDTBalance { get; set; }
        public string RpcResult { get; set; }

        protected override async Task OnInitializedAsync()
        {
            HasMetaMask = await MetaMask.IsMetaMaskAvailableAsync();

            var isSiteConnected = await MetaMask.IsSiteConnectedAsync();
            if (isSiteConnected)
            {
                await GetSelectedAddress();
                await GetSelectedNetwork();
                await GetBalance();
                await GetUSDTBalance();
            }

            MetaMask.AccountsChanged += OnAccountsChanged;
            MetaMask.ChainChanged += OnChainChanged;
        }

        private async void OnAccountsChanged(object sender, string[] accounts)
        {
            SelectedAccount = accounts[0];
            await GetBalance();
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
        
        public async Task ChangeAccount()
        {
            SelectedAccount = await MetaMask.ChangeAccountAsync();
        }

        public async Task GetSelectedAddress()
        {
            SelectedAccount = await MetaMask.GetSelectedAccountAsync();
        }

        public async Task GetSelectedNetwork()
        {
            SelectedChain = await MetaMask.GetSelectedChainAsync();
        }

        public async Task GetBalance()
        {
            var balanceWei = await MetaMask.GetBalanceAsync();
            Balance = balanceWei.ToString();
        }

        public async Task GetUSDTBalance()
        {
            // USDT token in BSC Testnet
            const string tokenAddress = "0x7d43aabc515c356145049227cee54b608342c0ad";
            var balanceWei = await MetaMask.GetTokenBalanceAsync(tokenAddress);
            USDTBalance = balanceWei.ToString();
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