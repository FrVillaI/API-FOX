'use strict';

const rateLimit = require('express-rate-limit');

const sendLimiter = rateLimit({
    windowMs: 60 * 1000,
    max: 30,
    standardHeaders: true,
    legacyHeaders: false,
    message: { success: false, message: 'Demasiadas solicitudes, espera un momento' },
});

const logsLimiter = rateLimit({
    windowMs: 60 * 1000,
    max: 10,
    message: { success: false, message: 'Demasiadas solicitudes de logs' },
});

module.exports = { sendLimiter, logsLimiter };