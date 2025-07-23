# --------------------------------------------------------------------------------

Write-Host "Make sure that all these files are in one folder."

# --------------------------------------------------------------------------------

$FrameworkVer = Read-Host "Enter version of UnityNeuroSpeech(e.g. 1.0.0): "

# --------------------------------------------------------------------------------

$ScriptDir = Split-Path -Parent $MyInvocation.MyCommand.Definition
$SevenZip = Join-Path $ScriptDir "7z/7z.exe"

# --------------------------------------------------------------------------------

Write-Host "Unpacking ""UnityNeuroSpeech.$FrameworkVer.rar""..."

$UNSRar = Join-Path $ScriptDir "UnityNeuroSpeech.$FrameworkVer.rar"
$UNSOut = $ScriptDir

& $SevenZip x $UNSRar -o"$UNSOut" -y > $null 2>&1

Write-Host "Done!"

# --------------------------------------------------------------------------------

Write-Host "Unpacking ""default.venv.rar""..."

$VenvRar = Join-Path $ScriptDir "default.venv.rar"
$VenvOut = Join-Path $ScriptDir "UnityNeuroSpeech $FrameworkVer\Server"

& $SevenZip x $VenvRar -o"$VenvOut" -y > $null 2>&1

Write-Host "Done!"

# --------------------------------------------------------------------------------

Write-Host "Unpacking ""TTSModel.rar""..."

$TTSRar = Join-Path $ScriptDir "TTSModel.rar"
$TTSOut = Join-Path $ScriptDir "UnityNeuroSpeech $FrameworkVer\Server"

& $SevenZip x $TTSRar -o"$TTSOut" -y > $null 2>&1

Write-Host "Done!"

# --------------------------------------------------------------------------------