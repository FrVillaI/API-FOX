# API-FOX - Envío de Facturas por WhatsApp

MVP para leer archivos DBF de FoxPro y enviar facturas electrónicas por WhatsApp Web.

## Estructura del Proyecto

```
API-FOX/
├── src/
│   └── Apifox.App/           # Aplicación .NET 8 WinForms
│       ├── DTOs/              # Modelos de datos
│       ├── Services/         # Lógica de negocio
│       └── Utils/            # Utilidades
├── services/
│   └── whatsapp-service/     # Servicio Node.js para WhatsApp Web
└── README.md
```

## Requisitos

- .NET 8 SDK
- Node.js 18+
- npm

## Configuración

### 1. Servicio WhatsApp

```bash
cd services/whatsapp-service
npm install
node index.js
```

El servicio mostrará un código QR para escanear con WhatsApp.

### 2. Aplicación .NET

```bash
cd src/Apifox.App
dotnet build
dotnet run
```

## Uso

1. **Iniciar servicio WhatsApp**: Ejecutar `npm start` en `services/whatsapp-service`
2. **Escanear QR**: En la consola del servicio verás el QR
3. **Ejecutar app .NET**: Abrir la aplicación WinForms
4. **Ingresar ruta DBF**: Ejemplo `C:\Fenix\DataFenix\EMPRESA\comercial`
5. **Cargar facturas**: Click en "Cargar Recientes"
6. **Buscar/Seleccionar**: Elegir factura del grid
7. **Enviar**: Click en "Enviar por WhatsApp"

## Archivos DBF Esperados

- `facturas.dbf` - Encabezados de facturas
- `clientes.dbf` - Datos de clientes
- `articulos.dbf` - Catálogo de productos
- `tranfac.dbf` - Detalles de facturas
- `doc_electronicos.dbf` - Autorizaciones SRI

## Endpoints del Servicio WhatsApp

| Método | Endpoint | Descripción |
|--------|----------|-------------|
| GET | `/status` | Verificar estado de conexión |
| POST | `/send` | Enviar mensaje |

### Enviar Mensaje

```json
POST http://localhost:3000/send
{
  "phone": "593991234567",
  "message": "Hola, esta es una prueba"
}
```

## Validaciones

- Factura debe estar autorizada por el SRI
- Teléfono debe ser válido (Ecuador)
- Total mayor a cero
- Clave de acceso presente

## Notas

- El servicio WhatsApp usa whatsapp-web.js (no oficial)
- Se requiere escanear QR cada vez que reinicia el servicio
- Delay de 5 segundos entre envíos para evitar bloqueos