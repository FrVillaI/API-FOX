'use strict';

const express = require('express');
const app = express();

// Body parser
app.use(express.json());

// Registrar eventos de WhatsApp (qr, ready, disconnected, etc.)
require('./whatsapp/events').registerEvents();

// Rutas
app.use(require('./routes/status'));
app.use(require('./routes/health'));
app.use(require('./routes/qr'));
app.use(require('./routes/send'));
app.use(require('./routes/logs'));
app.use(require('./routes/restart'));

// Error handler global
app.use((err, req, res, _next) => {
    const logger = require('./logger/logger');
    logger.error('Error no manejado en request', {
        method: req.method,
        path: req.path,
        error: err.message,
        stack: err.stack,
    });
    res.status(500).json({ success: false, message: 'Error interno del servidor' });
});

module.exports = app;