# NetLah.Extensions.Logging.Reference - .NET Library

[NetLah.Extensions.Logging.Reference](https://www.nuget.org/packages/NetLah.Extensions.Logging.Reference/) is library contains GetAppLogLogger to get logger in the indirect reference way.

## Nuget package

[![NuGet](https://img.shields.io/nuget/v/NetLah.Extensions.Logging.Reference.svg?style=flat-square&label=nuget&colorB=00b200)](https://www.nuget.org/packages/NetLah.Extensions.Logging.Reference/)

## Build Status

[![Build Status](https://img.shields.io/endpoint.svg?url=https%3A%2F%2Factions-badge.atrox.dev%2FNetLah%2Fserilog%2Fbadge%3Fref%3Dmain&style=flat)](https://actions-badge.atrox.dev/NetLah/serilog/goto?ref=main)

## Getting started

Add/Update PackageReference to .csproj

```xml
<ItemGroup>
  <PackageReference Include="NetLah.Extensions.Logging.Reference" Version="0.2.0" />
</ItemGroup>
```

Get logger by category name

```csharp
ILogger logger = AppLogReference.GetAppLogLogger("App");
```

Get logger by class as category name

```csharp
ILogger logger = AppLogReference.GetAppLogLogger<Program>();
```
