# Script para ejecutar servidor SOAP y cliente simultáneamente
Write-Host "=== Iniciando Sistema de Inventario Ferretería ===" -ForegroundColor Cyan
Write-Host ""

# Función para verificar si el servidor está listo
function Wait-ForServer {
    param([int]$MaxAttempts = 30)
    
    $attempt = 0
    Write-Host "Esperando que el servidor SOAP inicie..." -ForegroundColor Yellow
    
    while ($attempt -lt $MaxAttempts) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5233/InventarioService.asmx" -TimeoutSec 1 -ErrorAction SilentlyContinue
            if ($response.StatusCode -eq 200) {
                Write-Host "? Servidor SOAP iniciado correctamente" -ForegroundColor Green
                return $true
            }
        }
        catch {
            Start-Sleep -Seconds 1
            $attempt++
        }
    }
    
    Write-Host "? Timeout esperando al servidor" -ForegroundColor Red
    return $false
}

# Iniciar el servidor SOAP en segundo plano
Write-Host "1. Iniciando Servidor SOAP..." -ForegroundColor Cyan
$serverProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project InventarioFerreteria.SoapService" -PassThru

# Esperar a que el servidor esté listo
if (Wait-ForServer) {
    # Dar un segundo adicional para estabilización
    Start-Sleep -Seconds 2
    
    # Iniciar el cliente
    Write-Host ""
    Write-Host "2. Iniciando Cliente..." -ForegroundColor Cyan
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PSScriptRoot'; dotnet run --project InventarioFerreteria.Client"
    
    Write-Host ""
    Write-Host "=== Sistema iniciado correctamente ===" -ForegroundColor Green
    Write-Host ""
    Write-Host "Servidor SOAP: http://localhost:5233/InventarioService.asmx" -ForegroundColor Yellow
    Write-Host "Cliente: Ventana de consola separada" -ForegroundColor Yellow
    Write-Host ""
    Write-Host "Presiona Ctrl+C en cada ventana para detener" -ForegroundColor Gray
}
else {
    Write-Host "Error: No se pudo iniciar el servidor. Verifica PostgreSQL y la configuración." -ForegroundColor Red
    if ($serverProcess -and !$serverProcess.HasExited) {
        Stop-Process -Id $serverProcess.Id -Force
    }
}
