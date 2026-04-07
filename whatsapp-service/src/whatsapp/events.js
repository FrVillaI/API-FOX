'use strict';

const client = require('./client');
const { updateState } = require('../state/state');
const logger = require('../logger/logger');
const CONFIG = require('../config');

let reconnectTimer = null;

function scheduleReconnect() {
    const { state } = require('../state/state');

    if (state.reconnectAttempts >= CONFIG.MAX_RECONNECT_ATTEMPTS) {
        logger.error(`Máximo de reconexiones alcanzado (${CONFIG.MAX_RECONNECT_ATTEMPTS}). Intervención manual requerida.`);
        updateState({ status: 'error', lastError: 'Max reconnect attempts reached' });
        return;
    }

    const delay = CONFIG.RECONNECT_DELAY_MS * Math.pow(2, state.reconnectAttempts);
    state.reconnectAttempts++;

    logger.info(`Reconexión #${state.reconnectAttempts} en ${delay / 1000}s...`);

    clearTimeout(reconnectTimer);
    reconnectTimer = setTimeout(async () => {
        try {
            await client.initialize();
        } catch (err) {
            logger.error('Error al reconectar', { error: err.message, stack: err.stack });
            scheduleReconnect();
        }
    }, delay);
}

function registerEvents() {
    client.on('qr', (qr) => {
        updateState({ status: 'qr', qr, qrGeneratedAt: new Date().toISOString() });
        logger.info('QR generado — escanear con WhatsApp');
    });

    client.on('authenticated', () => {
        logger.info('Sesión autenticada correctamente');
        updateState({ qr: null, qrGeneratedAt: null });
    });

    client.on('ready', () => {
        updateState({
            status: 'ready',
            qr: null,
            readySince: new Date().toISOString(),
            reconnectAttempts: 0,
            lastError: null,
        });
        logger.info('WhatsApp listo y conectado');
    });

    client.on('disconnected', (reason) => {
        updateState({ status: 'disconnected', lastDisconnectReason: reason });
        logger.warn(`WhatsApp desconectado: ${reason}`);
        scheduleReconnect();
    });

    client.on('auth_failure', (msg) => {
        updateState({ status: 'error', lastError: `Auth failure: ${msg}` });
        logger.error(`Fallo de autenticación: ${msg}`);
    });

    client.on('message', (msg) => {
        logger.debug(`Mensaje recibido de ${msg.from}: tipo=${msg.type}`);
    });
}

module.exports = { registerEvents, scheduleReconnect, getReconnectTimer: () => reconnectTimer, clearReconnectTimer: () => clearTimeout(reconnectTimer) };