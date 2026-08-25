$files = Get-ChildItem -Filter *.cs -Recurse | Where-Object { $_.FullName -notmatch "\\obj\\" -and $_.FullName -notmatch "\\bin\\" }

foreach ($file in $files) {
    $content = Get-Content $file.FullName -Raw
    $modified = $false

    # 1. Replace "using global::" with "using "
    if ($content -match "using global::") {
        $content = $content -replace "using global::", "using "
        $modified = $true
    }

    # Helper function to replace inline globals and collect required usings
    $requiredUsings = @()

    if ($content -match "global::ERP\.Application\.Abstractions\.Caching\.ICacheService") {
        $content = $content -replace "global::ERP\.Application\.Abstractions\.Caching\.ICacheService", "ICacheService"
        $requiredUsings += "ERP.Application.Abstractions.Caching"
        $modified = $true
    }

    if ($content -match "global::Microsoft\.Extensions\.Options\.IOptions") {
        $content = $content -replace "global::Microsoft\.Extensions\.Options\.IOptions", "IOptions"
        $requiredUsings += "Microsoft.Extensions.Options"
        $modified = $true
    }

    if ($content -match "global::ERP\.Application\.Common\.Caching\.CacheSettings") {
        $content = $content -replace "global::ERP\.Application\.Common\.Caching\.CacheSettings", "CacheSettings"
        $requiredUsings += "ERP.Application.Common.Caching"
        $modified = $true
    }

    if ($content -match "global::ERP\.Application\.Common\.Caching\.CacheKeys") {
        $content = $content -replace "global::ERP\.Application\.Common\.Caching\.CacheKeys", "CacheKeys"
        $requiredUsings += "ERP.Application.Common.Caching"
        $modified = $true
    }

    if ($modified) {
        # Add required usings
        foreach ($ns in $requiredUsings | Select-Object -Unique) {
            $usingLine = "using $ns;"
            if (-not ($content -match "(?m)^using\s+$([regex]::Escape($ns))\s*;")) {
                # Find the first using statement
                if ($content -match "(?m)^using\s+([A-Za-z0-9_\.]+);") {
                    $content = $content -replace "(?m)^using\s+([A-Za-z0-9_\.]+);", "$usingLine`r`nusing `$1;"
                } elseif ($content -match "(?m)^namespace\s+[A-Za-z0-9_\.]+;") {
                    $content = $content -replace "(?m)^namespace\s+([A-Za-z0-9_\.]+);", "namespace `$1;`r`n`r`n$usingLine"
                } else {
                    $content = "$usingLine`r`n$content"
                }
            }
        }
        
        Set-Content -Path $file.FullName -Value $content -NoNewline
        Write-Host "Updated $($file.FullName)"
    }
}
