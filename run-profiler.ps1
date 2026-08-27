$projectPath = Join-Path $PSScriptRoot "src\OnnxRuntimeSharp.Profiler\OnnxRuntimeSharp.Profiler.csproj"
dotnet run -c Release -f net10.0 --project $projectPath
