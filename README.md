## BlazorCore GoogleMap
[![Package Version](https://img.shields.io/nuget/v/BlazorCore.GoogleMap?label=Latest%20Version)](https://www.nuget.org/packages/BlazorCore.GoogleMap/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorCore.GoogleMap?label=Downloads)](https://www.nuget.org/packages/BlazorCore.GoogleMap/)
[![License](https://img.shields.io/badge/License-MIT-green.svg)](https://github.com/hancheester/BlazorCore.GoogleMap/blob/master/LICENSE)

## About
Blazor components that render Google Maps, encapsulating map control and management within .NET code. These components seamlessly function with both WebAssembly and Server-hosted Blazor models.

## Installation
**BlazorCore.GoogleMap** is available on [NuGet](https://www.nuget.org/packages/BlazorCore.GoogleMap).
```sh
dotnet add package BlazorCore.GoogleMap
```

## Usage
Add using statement to your Blazor `<component/page>.razor` file. Or globally reference it into _Imports.razor file.

```csharp
using BlazorCore.GoogleMap;
using BlazorCore.JSInterop;
...
public static async Task Main(string[] args)
{
  var builder = WebAssemblyHostBuilder.CreateDefault(args);

  builder.Services.AddBlazorCoreJsInterop();
  builder.Services.AddGMapComponent();
  ...
}
```

### Dependences
**BlazorCore.GoogleMap** package depends on other BlazorGore Nuget packages:
- [BlazorCore.JSInterop](https://www.nuget.org/packages/BlazorCore.JSInterop)
which handles JS Interop for Geolocation services.
