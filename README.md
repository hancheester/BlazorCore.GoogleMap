<div id="top"></div>

## BlazorCore GoogleMap
[![Package Version](https://img.shields.io/nuget/v/BlazorCore.GoogleMap?label=Latest%20Version)](https://www.nuget.org/packages/BlazorCore.GoogleMap/)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BlazorCore.GoogleMap?label=Downloads)](https://www.nuget.org/packages/BlazorCore.GoogleMap/)
[![License](https://img.shields.io/badge/License-Apache_2.0-blue.svg)](https://github.com/hancheester/BlazorCore.GoogleMap/blob/master/LICENSE)
[![LinkedIn](https://img.shields.io/badge/Linkedin-%230077B5.svg?logo=linkedin&logoColor=white)](https://www.linkedin.com/in/hanchee)

## About
Blazor components that render Google Maps, encapsulating map control and management within .NET code. These components seamlessly function with both WebAssembly and Server-hosted Blazor models.

## Installation
**BlazorCore.GoogleMap** is available on [NuGet](https://www.nuget.org/packages/BlazorCore.GoogleMap).
```sh
dotnet add package BlazorCore.GoogleMap
```

<p align="right">(<a href="#top">back to top</a>)</p>

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

<p align="right">(<a href="#top">back to top</a>)</p>

### Dependences
**BlazorCore.GoogleMap** package depends on other BlazorGore Nuget packages:
- [BlazorCore.JSInterop](https://www.nuget.org/packages/BlazorCore.JSInterop)
which handles JS Interop for Geolocation services.

<p align="right">(<a href="#top">back to top</a>)</p>

## License
Distributed under the Apache License. See `LICENSE` for more information.

<p align="right">(<a href="#top">back to top</a>)</p>

## Contact
Han Chee - [@hancheester](https://x.com/hancheester) - hanchee@codecultivation.com

Project Link: https://github.com/hancheester/BlazorCore.GoogleMap

<p align="right">(<a href="#top">back to top</a>)</p>

