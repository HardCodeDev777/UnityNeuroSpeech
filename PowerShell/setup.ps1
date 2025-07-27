# --------------------------------------------------------------------------------

Write-Host "Make sure that all these files are in one folder."

# --------------------------------------------------------------------------------

$FrameworkVer = Read-Host "Enter version of UnityNeuroSpeech(e.g. 1.0.0): "
$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition

# --------------------------------------------------------------------------------

Write-Host "Unpacking ""UnityNeuroSpeech.$FrameworkVer.zip""..."

$UNSZip = Join-Path $ScriptDir "UnityNeuroSpeech.$FrameworkVer.zip"
$UNSOut = Join-Path $ScriptDir "UnityNeuroSpeech.$FrameworkVer"

Expand-Archive -Path $UNSZiP -DestinationPath $UNSOut -Force

Write-Host "Done!"

# --------------------------------------------------------------------------------

Write-Host "Unpacking ""TTSModel.zip""..."

$TTSZip = Join-Path $ScriptDir "TTSModel.zip"
$TTSOut = Join-Path $ScriptDir "UnityNeuroSpeech.$FrameworkVer\Server"

Expand-Archive -Path $TTSZip -DestinationPath $TTSOut -Force

Write-Host "Done!"

# --------------------------------------------------------------------------------