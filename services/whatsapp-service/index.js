const express = require('express');
const { Client } = require('whatsapp-web.js');
const qrcode = require('qrcode-terminal');

const app = express();
app.use(express.json());

let clientReady = false;
let readyTime = null;

const client = new Client({
    puppeteer: {
        headless: true,
        args: ['--no-sandbox', '--disable-setuid-sandbox']
    }
});

client.on('qr', (qr) => {
    console.log('\n=== ESCANEA ESTE QR CON WHATSAPP ===\n');
    qrcode.generate(qr, { small: true });
    console.log('\n======================================\n');
    clientReady = false;
});

client.on('ready', () => {
    console.log('WhatsApp Web conectado y listo!');
    clientReady = true;
    readyTime = Date.now();
});

client.on('disconnected', (reason) => {
    console.log('WhatsApp desconectado:', reason);
    clientReady = false;
    client.initialize();
});

client.on('auth_failure', (msg) => {
    console.error('Error de autenticación:', msg);
});

client.on('message', (msg) => {
    console.log('Mensaje recibido:', msg.from);
});

console.log('Inicializando WhatsApp Web...');
client.initialize();

app.get('/status', (req, res) => {
    if (clientReady) {
        const tiempoActivo = readyTime ? Math.floor((Date.now() - readyTime) / 1000) : 0;
        res.json({
            status: 'ready',
            uptime: tiempoActivo + ' segundos'
        });
    } else {
        res.status(503).json({
            status: 'not_ready',
            message: 'Esperando escaneo de QR'
        });
    }
});

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
            message: 'WhatsApp no está listo. Escanear QR primero.'
        });
    }

    try {
        const chatId = phone.includes('@c.us') ? phone : `${phone}@c.us`;
        
        await client.sendMessage(chatId, message);
        
        console.log(`Mensaje enviado a ${phone}`);
        
        await new Promise(resolve => setTimeout(resolve, 5000));
        
        res.json({
            success: true,
            message: 'Mensaje enviado exitosamente'
        });
    } catch (error) {
        console.error('Error al enviar mensaje:', error.message);
        res.status(500).json({
            success: false,
            message: error.message
        });
    }
});

const PORT = process.env.PORT || 3000;

app.listen(PORT, () => {
    console.log(`WhatsApp Service corriendo en http://localhost:${PORT}`);
    console.log(`Endpoints:`);
    console.log(`  GET  /status - Estado del servicio`);
    console.log(`  POST /send  - Enviar mensaje`);
});