'use strict';

const express = require('express');
const router = express.Router();
const client = require('../whatsapp/client');
const { updateState } = require('../state/state');
const { clearReconnectTimer } = require('../whatsapp/events');
const logger = require('../logger/logger');

router.post('/restart', async (req, res) => {
    logger.info('Reinicio manual solicitado');
    try {
        clearReconnectTimer();
        updateState({ status: 'initializing', reconnectAttempts: 0 });

        await client.destroy();
        await client.initialize();

        logger.info('Cliente reiniciado correctamente');
        res.json({ success: true, message: 'Reinicio iniciado' });
    } catch (err) {
        logger.error('Error en reinicio manual', { error: err.message, stack: err.stack });
        res.status(500).json({ success: false, message: err.message });
    }
});

module.exports = router;