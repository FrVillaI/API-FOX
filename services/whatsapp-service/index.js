const express = require('express');
const { Client, LocalAuth } = require('whatsapp-web.js');
const winston = require('winston');
const fs = require('fs');

const app = express();
app.use(express.json());

/* =========================
   LOGGER
========================= */
const logger = winston.createLogger({
    level: 'info',
    format: winston.format.json(),
    transports: [
        new winston.transports.File({ filename: 'error.log', level: 'error' }),
        new winston.transports.File({ filename: 'combined.log' })
    ]
});

/* =========================
   ESTADO DEL SERVICIO
========================= */
let serviceState = {
    status: 'initializing', // initializing | qr | ready | disconnected | error
    lastError: null,
    lastDisconnectReason: null,
    lastUpdate: new Date()
};

let clientReady = false;
let readyTime = null;
let currentQR = null;

/* =========================
   CLIENTE WHATSAPP
========================= */
const client = new Client({
    authStrategy: new LocalAuth({
        clientId: "facturacion-bot",
        dataPath: "./sessions"
    }),
    puppeteer: {
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    }
});

/* =========================
   EVENTOS
========================= */
client.on('qr', (qr) => {
    currentQR = qr;

    serviceState.status = 'qr';
    serviceState.lastUpdate = new Date();

    logger.info('QR generado');
});

client.on('ready', () => {
    clientReady = true;
    readyTime = Date.now();

    serviceState.status = 'ready';
    serviceState.lastError = null;
    serviceState.lastUpdate = new Date();

    logger.info('WhatsApp conectado');
});

client.on('disconnected', (reason) => {
    clientReady = false;

    serviceState.status = 'disconnected';
    serviceState.lastDisconnectReason = reason;
    serviceState.lastUpdate = new Date();

    logger.error(`Desconectado: ${reason}`);

    client.initialize(); // auto-reconexión
});

client.on('auth_failure', (msg) => {
    serviceState.status = 'error';
    serviceState.lastError = msg;
    serviceState.lastUpdate = new Date();

    logger.error(`Auth failure: ${msg}`);
});

client.on('message', (msg) => {
    logger.info(`Mensaje recibido de ${msg.from}`);
});

/* =========================
   INICIALIZACIÓN
========================= */
logger.info('Inicializando WhatsApp...');
client.initialize();

/* =========================
   ENDPOINTS
========================= */

// Estado básico (compatibilidad)
app.get('/status', (req, res) => {
    if (clientReady) {
        const uptime = readyTime
            ? Math.floor((Date.now() - readyTime) / 1000)
            : 0;

        return res.json({
            status: 'ready',
            uptime: uptime + ' segundos'
        });
    }

    res.status(503).json({
        status: 'not_ready',
        message: 'Esperando QR'
    });
});

// Estado avanzado
app.get('/health', (req, res) => {
    res.json({
        ...serviceState,
        uptime: process.uptime()
    });
});

// Obtener QR
app.get('/qr', (req, res) => {
    if (!currentQR) {
        return res.status(404).json({
            message: 'QR no disponible o ya autenticado'
        });
    }

    res.json({ qr: currentQR });
});

// Enviar mensaje
app.post('/send', async (req, res) => {
    const { phone, message } = req.body;

    if (!phone || !message) {
        return res.status(400).json({
            success: false,
            message: 'Se requiere phone y message'
        });
    }

    if (!clientReady) {
        return res.status(503).json({
            success: false,
            message: 'WhatsApp no listo'
        });
    }

    try {
        const chatId = phone.includes('@c.us')
            ? phone
            : `${phone}@c.us`;

        await client.sendMessage(chatId, message);

        logger.info(`Mensaje enviado a ${phone}`);

        res.json({
            success: true,
            message: 'Mensaje enviado'
        });

    } catch (error) {
        logger.error(`Error enviando mensaje: ${error.message}`);

        res.status(500).json({
            success: false,
            message: error.message
        });
    }
});

// Reiniciar servicio
app.post('/restart', async (req, res) => {
    try {
        await client.destroy();
        await client.initialize();

        logger.info('Servicio reiniciado');

        res.json({ success: true });
    } catch (err) {
        logger.error(`Error reiniciando: ${err.message}`);

        res.status(500).json({
            success: false,
            message: err.message
        });
    }
});

// Ver logs (opcional)
app.get('/logs', (req, res) => {
    try {
        const logs = fs.readFileSync('combined.log', 'utf-8');
        res.send(logs);
    } catch (err) {
        res.status(500).send('No se pudo leer logs');
    }
});

/* =========================
   SERVIDOR
========================= */
const PORT = process.env.PORT || 3000;

app.listen(PORT, () => {
    logger.info(`Servicio corriendo en puerto ${PORT}`);
});