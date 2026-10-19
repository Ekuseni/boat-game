[CmdletBinding()]
param(
    [Parameter(ValueFromRemainingArguments = $true)]
    [string[]]$UnityArgs
)

$projectRoot = $PSScriptRoot
$versionFile = Join-Path $projectRoot 'ProjectSettings\ProjectVersion.txt'
if (-not (Test-Path $versionFile)) {
    throw "Unity project version file not found: $versionFile"
}

$versionLine = Get-Content $versionFile | Where-Object { $_ -match '^m_EditorVersion:\s*(.+)$' } | Select-Object -First 1
if (-not $versionLine -or $versionLine -notmatch '^m_EditorVersion:\s*(.+)$') {
    throw "Could not read the Unity editor version from $versionFile"
}
$editorVersion = $Matches[1].Trim()

$editorPath = $env:UNITY_EDITOR_PATH
if (-not $editorPath) {
    $editorPath = Join-Path $env:ProgramFiles "Unity\Hub\Editor\$editorVersion\Editor\Unity.exe"
}
if (-not (Test-Path $editorPath -PathType Leaf)) {
    throw "Unity $editorVersion was not found at '$editorPath'. Install it with Unity Hub or set UNITY_EDITOR_PATH to Unity.exe."
}

& $editorPath -projectPath $projectRoot @UnityArgs
exit $LASTEXITCODE
