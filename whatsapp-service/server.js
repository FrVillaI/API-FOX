'use strict';

const app = require('./src/app');
const client = require('./src/whatsapp/client');
const { clearReconnectTimer } = require('./src/whatsapp/events');
const logger = require('./src/logger/logger');
const CONFIG = require('./src/config');

const server = app.listen(CONFIG.PORT, () => {
    logger.info('Servicio iniciado', { port: CONFIG.PORT, env: CONFIG.NODE_ENV, pid: process.pid });
});

logger.info('Inicializando cliente WhatsApp...');
client.initialize().catch(err => {
    const { scheduleReconnect } = require('./src/whatsapp/events');
    logger.error('Error en inicialización inicial', { error: err.message, stack: err.stack });
    scheduleReconnect();
});

/* =========================
   APAGADO GRACEFUL
========================= */
async function shutdown(signal) {
    logger.info(`Señal ${signal} recibida. Cerrando...`);
    clearReconnectTimer();

    server.close(() => logger.info('Servidor HTTP cerrado'));

    try {
        await client.destroy();
        logger.info('Cliente WhatsApp cerrado');
    } catch (err) {
        logger.error('Error cerrando cliente WhatsApp', { error: err.message });
    }

    process.exit(0);
}

process.on('SIGTERM', () => shutdown('SIGTERM'));
process.on('SIGINT', () => shutdown('SIGINT'));

process.on('uncaughtException', (err) => {
    logger.error('Excepción no capturada', { error: err.message, stack: err.stack });
});

process.on('unhandledRejection', (reason) => {
    logger.error('Promise rechazada sin manejar', { reason: String(reason) });
});