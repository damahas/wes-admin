param(
    [string]$OutDir = "Wes.WebApi\DataProtection"
)

# 解析为相对于脚本所在目录往上三级（项目根目录）
$scriptRoot = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Resolve-Path (Join-Path $scriptRoot "..\..\..")
$outDir = Join-Path $projectRoot $OutDir

if (-not (Test-Path $outDir)) {
    New-Item -ItemType Directory -Path $outDir -Force | Out-Null
}

$keyId     = [Guid]::NewGuid().ToString("D")
$now       = [DateTime]::UtcNow
$expire    = $now.AddDays(90)
$timeFmt   = "yyyy-MM-ddTHH:mm:ss.fffffffZ"

$rng       = New-Object System.Security.Cryptography.RNGCryptoServiceProvider
$randBytes = New-Object byte[] 64
$rng.GetBytes($randBytes)
$masterKey = [Convert]::ToBase64String($randBytes)
$rng.Dispose()

$xml = @"
<?xml version="1.0" encoding="utf-8"?>
<key id="$keyId" version="1">
  <creationDate>$($now.ToString($timeFmt))</creationDate>
  <activationDate>$($now.ToString($timeFmt))</activationDate>
  <expirationDate>$($expire.ToString($timeFmt))</expirationDate>
  <descriptor deserializerType="Microsoft.AspNetCore.DataProtection.AuthenticatedEncryption.ConfigurationModel.AuthenticatedEncryptorDescriptorDeserializer, Microsoft.AspNetCore.DataProtection">
    <descriptor>
      <encryption algorithm="AES_256_CBC" />
      <validation algorithm="HMACSHA256" />
      <masterKey>
        <value>$masterKey</value>
      </masterKey>
    </descriptor>
  </descriptor>
</key>
"@

$filePath = Join-Path $outDir "key-$keyId.xml"
$xml | Out-File -FilePath $filePath -Encoding utf8

Write-Host "OK: $filePath"
Write-Host "Copy this directory into Docker image or mount via -v"
