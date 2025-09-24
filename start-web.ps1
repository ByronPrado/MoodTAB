# --- start-web.ps1 ---

# 1️⃣ Guardar ruta actual
$root = Get-Location

# 2️⃣ Ir a la carpeta del servidor web
Set-Location "$root\WebConTablas\WebConTablas"

# 3️⃣ Ejecutar dotnet run en un nuevo proceso
Start-Process powershell -ArgumentList "dotnet run" -NoNewWindow

# Esperar unos segundos para que el servidor web se inicie
Start-Sleep -Seconds 5

# 4️⃣ Ir a la carpeta de ngrok
Set-Location "$root\Ngrok"

# 5️⃣ Ejecutar ngrok con config
Start-Process powershell -ArgumentList "ngrok start web --config .\ngrok.yml" -NoNewWindow
