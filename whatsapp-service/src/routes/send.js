'use strict';

const express = require('express');
const router = express.Router();
const client = require('../whatsapp/client');
const { state } = require('../state/state');
const logger = require('../logger/logger');
const { sendLimiter } = require('../middleware/rateLimit');
const { validatePhone, normalizePhone } = require('../utils/phone');
const CONFIG = require('../config');

router.post('/send', sendLimiter, async (req, res) => {
    const { phone, message } = req.body;

    if (!phone || !message) {
        return res.status(400).json({
            success: false,
            message: 'Los campos phone y message son requeridos',
        });
    }

    if (typeof message !== 'string' || message.trim().length === 0) {
        return res.status(400).json({ success: false, message: 'El mensaje no puede estar vacío' });
    }

    if (!validatePhone(phone)) {
        return res.status(400).json({
            success: false,
            message: 'Formato de teléfono inválido (ej: 593987654321)',
        });
    }

    if (state.status !== 'ready') {
        return res.status(503).json({
            success: false,
            message: `WhatsApp no disponible (estado: ${state.status})`,
            hint: state.status === 'qr' ? 'Escanea el QR en GET /qr' : 'Revisa GET /health para más detalles',
        });
    }

    const chatId = normalizePhone(phone);

    try {
        await client.sendMessage(chatId, message.trim());
        logger.info('Mensaje enviado', { to: chatId, length: message.length });

        res.json({ success: true, message: 'Mensaje enviado correctamente', to: chatId });
    } catch (error) {
        logger.error('Error enviando mensaje', { to: chatId, error: error.message, stack: error.stack });

        res.status(500).json({
            success: false,
            message: 'Error al enviar el mensaje',
            detail: CONFIG.NODE_ENV !== 'production' ? error.message : undefined,
        });
    }
});

module.exports = router;