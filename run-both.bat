@echo off
echo ========================================
echo  Sistema Inventario Ferreteria
echo ========================================
echo.

echo [1/2] Iniciando Servidor SOAP...
start "Servidor SOAP" cmd /k "dotnet run --project InventarioFerreteria.SoapService"

echo [2/2] Esperando 5 segundos...
timeout /t 5 /nobreak > nul

echo [2/2] Iniciando Cliente...
start "Cliente" cmd /k "dotnet run --project InventarioFerreteria.Client"

echo.
echo ========================================
echo  Sistema iniciado correctamente
echo ========================================
echo.
echo Servidor: http://localhost:5233
echo Cliente: Ventana separada
echo.
echo Cierra las ventanas para detener.
echo.
pause
