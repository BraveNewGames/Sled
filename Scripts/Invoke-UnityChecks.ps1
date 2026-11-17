param(
    [ValidateSet('Compile', 'EditMode')]
    [string]$Mode = 'EditMode',
    [string]$UnityPath
)

$ErrorActionPreference = 'Stop'
$projectRoot = (Resolve-Path -LiteralPath (Join-Path $PSScriptRoot '..')).Path
$versionFile = Join-Path $projectRoot 'ProjectSettings\ProjectVersion.txt'
if (-not (Test-Path -LiteralPath $versionFile)) {
    throw "Unity project version file is missing: $versionFile"
}

$versionLine = Get-Content -LiteralPath $versionFile | Where-Object { $_ -match '^m_EditorVersion: ' } | Select-Object -First 1
if (-not $versionLine) { throw 'ProjectSettings/ProjectVersion.txt has no Editor version.' }
$version = ($versionLine -split ': ', 2)[1].Trim()
if (-not $UnityPath) {
    $UnityPath = Join-Path 'C:\Program Files\Unity\Hub\Editor' "$version\Editor\Unity.exe"
}
if (-not (Test-Path -LiteralPath $UnityPath -PathType Leaf)) {
    throw "Unity $version was not found at $UnityPath. Pass -UnityPath with the verified Editor executable."
}

$outputDir = Join-Path $projectRoot '.artifacts\unity'
New-Item -ItemType Directory -Path $outputDir -Force | Out-Null
$logPath = Join-Path $outputDir ("$($Mode.ToLowerInvariant()).log")
$resultsPath = Join-Path $outputDir 'editmode-results.xml'
Remove-Item -LiteralPath $logPath -ErrorAction SilentlyContinue
if ($Mode -eq 'EditMode') { Remove-Item -LiteralPath $resultsPath -ErrorAction SilentlyContinue }

$arguments = @('-batchmode', '-nographics', '-projectPath', "`"$projectRoot`"", '-logFile', "`"$logPath`"")
if ($Mode -eq 'Compile') {
    $arguments += '-quit'
} else {
    $arguments += @('-runTests', '-testPlatform', 'EditMode', '-testResults', "`"$resultsPath`"")
}

Write-Host "Unity $version | $Mode | $projectRoot"
$process = Start-Process -FilePath $UnityPath -ArgumentList $arguments -PassThru -WindowStyle Hidden
$process.WaitForExit()
$exitCode = $process.ExitCode

if (-not (Test-Path -LiteralPath $logPath)) { throw "Unity wrote no log: $logPath" }
$log = Get-Content -LiteralPath $logPath -Raw
$compileError = $log -match '(?m)\berror CS\d{4}\b|Scripts have compiler errors|Compilation failed'
if ($exitCode -ne 0 -or $compileError) {
    Get-Content -LiteralPath $logPath -Tail 50 | Write-Host
    throw "Unity $Mode failed (exit $exitCode). See $logPath"
}

if ($Mode -eq 'EditMode') {
    if (-not (Test-Path -LiteralPath $resultsPath)) {
        throw "Unity wrote no test results. See $logPath"
    }
    [xml]$results = Get-Content -LiteralPath $resultsPath -Raw
    $run = $results.'test-run'
    if (-not $run) { throw "Test results have no test-run element: $resultsPath" }
    $total = [int]$run.total
    $failed = [int]$run.failed
    if ($total -lt 1 -or $failed -gt 0 -or $run.result -ne 'Passed') {
        throw "EditMode tests did not pass (total $total, failed $failed, result $($run.result)). See $resultsPath"
    }
    Write-Host "EditMode tests passed: $total. Results: $resultsPath"
} else {
    Write-Host "Compilation passed. Log: $logPath"
}
