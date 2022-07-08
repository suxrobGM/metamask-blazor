# Ethereum.MetaMask.Blazor
Interop library for simplifying MetaMask's API for Blazor WebAssembly and Server apps.

## Getting Started
Register MetaMask service in `IServiceCollection` container.

In Blazor WebAssembly or Server-Side, just simply call the `AddMetaMaskBlazor()` method in services container.
```csharp
builder.Services.AddMetaMaskBlazor();
```

Inject `IMetaMaskService` in razor pages
```csharp
@inject IMetaMaskService MetaMaskService
```

or in razor class files (.razor.cs files)
```csharp
[Inject]
public IMetaMaskService MetaMaskService { get; set; }
```

### Warning
Do not call interop methods in `OnInitialized` when using Blazor server pre-rendering. Call interop methods only after rendering, otherwise it throws an exception. [See more details](https://docs.microsoft.com/en-us/aspnet/core/blazor/javascript-interoperability/call-javascript-from-dotnet?view=aspnetcore-5.0#detect-when-a-blazor-server-app-is-prerendering-1)

## Available methods
- `Connect` - Connects to the wallet.
- `ChangeAccount` - Requests to change account.
- `IsMetaMaskAvailable` - Checks whether MetaMask is installed or not in browser.
- `IsSiteConnected` - Checks whether the site is connected to MetaMask or not.
- `GetSelectedAddress` - Gets current address.
- `GetSelectedChain` - Gets current chain.
- `GetBalance` - Gets balance of the given address.
- `GetTokenBalance` - Gets balance of the custom token.
- `SendTransaction` - Creates a new message call transaction.
- `RequestRpc` - Makes an RPC call.

## Events
- `AccountsChanged` - Raises when user changes account.
- `ChainChanged` - Raises when user changes chain.
- `Connect` - Raises when user connects to MetaMask.
- `Disconnect` - Raises when user disconnects from MetaMask.

## Samples
Check out sample blazor wasm and server applications [here](https://github.com/suxrobGM/metamask-blazor/tree/master/samples)
