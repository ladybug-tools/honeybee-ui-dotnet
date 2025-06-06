[![Build](https://github.com/ladybug-tools/honeybee-ui-dotnet/workflows/CD/badge.svg)](https://github.com/ladybug-tools/honeybee-ui-dotnet/actions) [![NuGet Version and Downloads count](https://buildstats.info/nuget/Honeybee.UI?dWidth=50)](https://www.nuget.org/packages/Honeybee.UI)

# honeybee-ui-dotnet

This is the UI library with ETO dialogs and forms for editing Honeybee Schema (DotNet) on both Windows and Mac system.

![2020-06-22-17-47-41](https://user-images.githubusercontent.com/9031066/85338696-c6c0f680-b4b0-11ea-82ac-0c1108f10966.gif)

Same experience as in OpenStuio App
![2020-06-22-17-54-20](https://user-images.githubusercontent.com/9031066/85339163-929a0580-b4b1-11ea-8842-874be866f688.gif)

## Installation

Run the following command or via Visual Studio Nuget Package Manager to install the library

- [dotnet] `dotnet add package Honeybee.UI`

Then include the DLL (under the `bin` folder) in the C# project, and use the namespaces:

```csharp
using HoneybeeSchema;
using Honeybee.UI;
```

## Getting Started

```csharp
var energyProp = new HoneybeeSchema.RoomEnergyPropertiesAbridged();
var dialog = new Honeybee.UI.Dialog_RoomEnergyProperty(energyProp);
var dialog_rc = dialog.ShowModal();
if (dialog_rc != null)
{
    Console.WriteLine(dialog_rc.ToJson());
}
```

### A demo for standalone App:

- [Windows] `src\Honeybee.UI.ConsoleApp\Honeybee.UI.ConsoleApp.csproj`
- [Mac OS] `src/Honeybee.UI.ConsoleApp/Honeybee.UI.ConsoleAppMac.csproj` (not maintained)
