# LTRLib

Legacy .NET libraries used by tools and applications by Olof Lagerkvist, LTR Data.

**The majority of this code base is very old. Many parts are around 20 years old and were originally developed in VB.NET.** Much of that code was subsequently converted to C#, while `LTRLib.Windows.Shell` still contains VB.NET source. Historical names, coding patterns and framework assumptions reflect that background. Modern target frameworks and recent compatibility fixes do not imply that every component has been redesigned.

## Role of this repository

LTRLib is retained primarily for existing applications and legacy code that cannot easily migrate to newer libraries or frameworks. Selected fixes and compatibility updates still occur here.

Frequently used, actively developed components have moved to [LTRData/Library](https://github.com/LTRData/Library), with corresponding changes to package names and namespaces. For new development, start with that repository and check whether it provides the functionality you need. LTRLib and Library are separate repositories; Library is not a drop-in replacement for every LTRLib API.

The relationship is also visible in the current dependencies: `LTRLib40` uses `LTRData.Extensions` on applicable targets, and `LTRLib.Windows` uses `LTRData.Geodesy`, `LTRData.MathExpression` and `LTRData.Net`. Modern expression parsing, function plotting and SkiaSharp rendering are documented in [Library](https://github.com/LTRData/Library#mathematical-expressions-and-plotting); the old `LTRLib.MathGraph.Surface` drawing helper remains here.

## Packages and components

The library projects use their project names as NuGet package IDs. The links below point to the current project files, which define dependencies and framework targets.

| Package / project | Contents |
| --- | --- |
| [LTRLib](https://github.com/LTRData/LTRLib/blob/main/LTRLib/LTRLib.csproj) | Aggregate project referencing the component libraries. Included components vary by target, especially for Windows desktop and LINQ to SQL support. |
| [LTRLib40](https://github.com/LTRData/LTRLib/blob/main/LTRLib40/LTRLib40.csproj) | General utilities: stream adapters, readers/writers, collections, text and date/time helpers, reflection/IL helpers, older networking helpers and Morse conversion. The name is historical; it does not limit the project to .NET Framework 4.0. |
| [LTRLib.NativeIo](https://github.com/LTRData/LTRLib/blob/main/LTRLib.NativeIo/LTRLib.NativeIo.csproj) | Primarily Windows native file, disk, volume, process and compression wrappers, plus console, marshaling, resource and task helpers. |
| [LTRLib.Management](https://github.com/LTRData/LTRLib/blob/main/LTRLib.Management/LTRLib.Management.csproj) | System.Management/WMI wrappers for Windows disks, volumes, optical drives, operating-system information and BitLocker volumes. |
| [LTRLib.CodeCompiler](https://github.com/LTRData/LTRLib/blob/main/LTRLib.CodeCompiler/LTRLib.CodeCompiler.csproj) | Legacy runtime compilation helper using CodeDOM providers and historical compiler-version selections. |
| [LTRLib.Data.Linq](https://github.com/LTRData/LTRLib/blob/main/LTRLib.Data.Linq/LTRLib.Data.Linq.csproj) | LINQ to SQL and Windows Forms binding/clipboard extensions for .NET Framework. |
| [LTRLib.Windows](https://github.com/LTRData/LTRLib/blob/main/LTRLib.Windows/LTRLib.Windows.csproj) | Windows Forms/WPF helpers, image geotag extraction, legacy graph drawing, Windows Terminal Services and Ghostscript interop. |
| [LTRLib.Windows.Shell](https://github.com/LTRData/LTRLib/blob/main/LTRLib.Windows.Shell/LTRLib.Windows.Shell.vbproj) | VB.NET dialogs, message-box helpers and Windows Script Host shell/shortcut integration. |

Most namespaces begin with `LTRLib`, but package and namespace names are not interchangeable. For example, the generated WMI types also use namespaces such as `ROOT.CIMV2.Win32`.

## Framework and platform notes

The current project files declare these targets:

| Projects | Targets |
| --- | --- |
| LTRLib40, LTRLib.CodeCompiler, LTRLib.Management | .NET Framework 2.0, 3.0, 3.5, 4.0, 4.6 and 4.8; .NET Standard 2.0/2.1; .NET 6/8/9/10 |
| LTRLib.NativeIo | .NET Framework 3.5, 4.0, 4.6 and 4.8; .NET Standard 2.0/2.1; .NET 6/8/9/10 |
| LTRLib.Data.Linq | .NET Framework 3.5, 4.0, 4.6 and 4.8 |
| LTRLib.Windows, LTRLib.Windows.Shell | .NET Framework 3.5, 4.0, 4.6 and 4.8; Windows-specific .NET 6/8/9/10 |
| LTRLib | .NET Framework 2.0, 3.0, 3.5, 4.0, 4.6 and 4.8; .NET Standard 2.0/2.1; .NET 6/8/9/10, including Windows-specific variants |

These are build targets. API availability also depends on conditional compilation, operating-system services and runtime support for the older APIs used by each component.

Windows Forms, WPF, WMI, Windows Script Host, Terminal Services and Win32/NT calls require the corresponding Windows facilities even when a containing package has a .NET Standard or general .NET target. The Ghostscript wrapper imports `gsdll32.dll` and requires that native library with a compatible process architecture. CodeCompiler depends on the selected CodeDOM provider and compiler being usable on the runtime where the application runs.

## Building and testing

Use the .NET 10 SDK for the current source. Build the specific project and framework needed by your consumer. For example, from the repository root:

```sh
dotnet build LTRLib40/LTRLib40.csproj -c Debug -f net10.0
dotnet build LTRLib.NativeIo/LTRLib.NativeIo.csproj -c Debug -f net10.0
dotnet test TestProject/TestProject.csproj -c Debug -f net10.0
```

Debug avoids automatic NuGet packaging. Release builds generate packages; [Directory.Build.props](https://github.com/LTRData/LTRLib/blob/main/Directory.Build.props) contains the shared package settings and accepts `LocalNuGetPath` for the package output directory. Building all targets also requires the corresponding framework reference assemblies. Several dependencies use floating package versions, so restore results can change over time.

For the Windows desktop components, use the appropriate Windows development tools. In particular, `LTRLib.Windows.Shell` has a Windows Script Host COM reference and should be built using Visual Studio/MSBuild on Windows with that type library available.

[The test project](https://github.com/LTRData/LTRLib/blob/main/TestProject/TestProject.csproj) targets .NET 8/9/10 and .NET Framework 4.6.2/4.8. Its current tests cover a small selection of text, process-awaiting and wait-handle helpers; they are not comprehensive coverage of the legacy code base.

[The solution](https://github.com/LTRData/LTRLib/blob/main/LTRLib.slnx) also retains Sandcastle documentation projects under [doc](https://github.com/LTRData/LTRLib/tree/main/doc). Those require additional tooling and still reference historical assemblies, so they need separate attention when rebuilding the old help files.

## License

See [LICENSE.txt](https://github.com/LTRData/LTRLib/blob/main/LICENSE.txt) for the repository's license terms, and retain applicable copyright and attribution notices in the source files.
