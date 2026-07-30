param(
    [string]$runtime = "win-arm64"
)
dotnet publish src/OnnxRuntimeSharp.Tester/OnnxRuntimeSharp.Tester.csproj -c Release -r "$runtime" -f net10.0 --self-contained true /p:PublishAot=true /p:DebugSymbols=true
dumpbin /DISASM /SYMBOLS "artifacts\publish\OnnxRuntimeSharp.Tester\release_$runtime\OnnxRuntimeSharp.Tester.exe" > "artifacts\publish\OnnxRuntimeSharp.Tester\release_$runtime\disassembly.asm"
