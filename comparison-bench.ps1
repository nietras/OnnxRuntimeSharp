param(
    [string]$filter = "*"
)
$benchmarksDirectory = Join-Path $PSScriptRoot "benchmarks"
New-Item -ItemType Directory -Path $benchmarksDirectory -Force | Out-Null
dotnet run -c Release -f net10.0 --project src\OnnxRuntimeSharp.Profiler\OnnxRuntimeSharp.Profiler.csproj |
    Set-Content (Join-Path $benchmarksDirectory "mnist-8.txt")
