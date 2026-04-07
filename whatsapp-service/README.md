# WhatsApp Facturación — Servicio de Producción

Microservicio Node.js para envío de mensajes WhatsApp (.NET 8).

---

## Estructura del proyecto

```
whatsapp-service/
├── server.js
├── install-service.js
├── uninstall-service.js
├── ecosystem.config.js
├── package.json
├── .env
└── src/
    ├── app.js
    ├── config/index.js
    ├── logger/logger.js
    ├── state/state.js
    ├── utils/phone.js
    ├── whatsapp/
    │   ├── client.js
    │   └── events.js
    ├── middleware/
    │   ├── auth.js
    │   └── rateLimit.js
    └── routes/
        ├── send.js
        ├── status.js
        ├── health.js
        ├── qr.js
        ├── logs.js
        └── restart.js
```

---

## Instalación — Windows (producción)

```bash
# 1. Instalar Node.js 18+ desde https://nodejs.org

# 2. Clonar/copiar el proyecto al servidor
cd C:\proyectos
# copiar carpeta whatsapp-service aquí

# 3. Instalar dependencias
npm install

# 4. Crear .env desde el ejemplo
copy .env.example .env
# Editar .env con el bloc de notas si es necesario

# 5. Instalar como servicio de Windows (ejecutar como Administrador)
npm install -g node-windows
npm link node-windows
node install-service.js
```

---

## Instalación — Linux (Ubuntu/Debian)

```bash
# 1. Instalar Node.js 18+
sudo apt update && sudo apt install -y nodejs npm chromium-browser

# 2. Copiar el proyecto
cd /opt
# copiar carpeta whatsapp-service aquí
cd whatsapp-service

# 3. Instalar dependencias
npm install

# 4. Crear .env
cp .env.example .env

# 5. Instalar PM2 y arrancar
npm install -g pm2
npm run pm2:start
pm2 startup
pm2 save
```

---

## Desarrollo local

```bash
# 1. Instalar dependencias
npm install

# 2. Crear .env
cp .env.example .env   # Linux/Mac
copy .env.example .env # Windows

# 3. Correr en modo desarrollo (logs en consola)
npm run dev
```

---

## Primer uso — escanear QR

1. Con el servicio corriendo, abre en el navegador: `http://localhost:3000/qr-page`
2. Escanea el código QR con WhatsApp del número que enviará las facturas
3. Una vez autenticado verás en los logs: `WhatsApp listo y conectado`
4. La sesión persiste en `./sessions/` — no necesitas re-escanear al reiniciar

---

## Endpoints

Sin autenticación (servicio local en localhost).

| Método | Ruta | Descripción |
|--------|------|-------------|
| GET | `/ping` | Health check básico |
| GET | `/status` | Estado + uptime |
| GET | `/health` | Estado completo + info del proceso |
| GET | `/qr` | QR en texto (solo si no está autenticado) |
| GET | `/qr-page` | Página HTML para escanear el QR |
| POST | `/send` | Envía un mensaje |
| POST | `/restart` | Reinicia el cliente WhatsApp |
| GET | `/logs?level=error&lines=100` | Logs del día actual |

### Ejemplo /send

```http
POST /send
Content-Type: application/json

{
  "phone": "593987654321",
  "message": "Factura #001 por $150.00"
}
```

---

## Logs

Los logs se guardan en `./logs/` con rotación diaria:
- `combined-YYYY-MM-DD.log` — todos los eventos
- `error-YYYY-MM-DD.log` — solo errores
- Se comprimen automáticamente al día siguiente
- Se eliminan automáticamente después de 30 días

Para ver los últimos errores sin acceder al servidor:
```
GET http://localhost:3000/logs?level=error&lines=50
```

---

## Gestión del servicio de Windows

```bash
# Instalar (ejecutar como Administrador)
node install-service.js

# Desinstalar (ejecutar como Administrador)
node uninstall-service.js

# También puedes gestionarlo desde:
# Panel de control > Herramientas administrativas > Servicios
# Buscar: "WhatsApp Facturacion"
```

---

## Gestión con PM2 (Linux)

```bash
npm run pm2:start      # Iniciar en producción
npm run pm2:stop       # Detener
npm run pm2:restart    # Reiniciar
npm run pm2:logs       # Ver logs en tiempo real
npm run pm2:status     # Ver estado
```

---

## Integración .NET 8

Usar la clase `WhatsAppService` en el proyecto Nexora:

```csharp
// Uso básico
var resultado = await _whatsApp.EnviarMensaje("593987654321", "Tu factura #001 está lista");

if (!resultado.Exito)
    logger.LogError("No se pudo enviar WhatsApp: {Error}", resultado.Mensaje);

// Verificar que el servicio está listo antes de enviar
var listo = await _whatsApp.CheckConexion();
if (!listo)
    throw new InvalidOperationException("El servicio WhatsApp no está disponible");
```

Configuración en `appsettings.json`:
```json
{
  "WhatsApp": {
    "BaseUrl": "http://localhost:3000",
    "TimeoutSeconds": 15
  }
}
```

---

## Troubleshooting

**El QR no aparece en `/qr-page`:**
```bash
# Ver logs recientes
GET http://localhost:3000/logs?level=combined&lines=30
```

**Error de Chromium / Puppeteer:**
Verifica que los args `--no-sandbox` y `--disable-dev-shm-usage` estén en `src/whatsapp/client.js`.

**Sesión expirada o corrompida:**
```bash
# Detener el servicio, borrar sesión y reiniciar
rm -rf sessions/        # Linux
rd /s /q sessions       # Windows (cmd como Admin)
# Volver a escanear QR desde /qr-page
```

**Ver estado completo desde .NET:**
```csharp
var health = await _whatsApp.GetHealth();
// health.Service.Status == "ready" | "qr" | "disconnected" | "error"
```

**El servicio de Windows no arranca:**
- Verificar que Node.js está instalado y en el PATH del sistema
- Revisar el visor de eventos de Windows: `eventvwr.msc`
- Revisar logs en `./logs/error-YYYY-MM-DD.log`