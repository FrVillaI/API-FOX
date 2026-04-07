'use strict';

const express = require('express');
const router = express.Router();
const fs = require('fs');
const path = require('path');
const { logsLimiter } = require('../middleware/rateLimit');
const logger = require('../logger/logger');
const CONFIG = require('../config');

router.get('/logs', logsLimiter, (req, res) => {
    const level = req.query.level === 'error' ? 'error' : 'combined';
    const lines = Math.min(parseInt(req.query.lines) || 100, 500);
    const today = new Date().toISOString().slice(0, 10);
    const logFile = path.join(CONFIG.LOG_DIR, `${level}-${today}.log`);

    try {
        if (!fs.existsSync(logFile)) {
            return res.json({ lines: [], message: 'No hay logs para hoy todavía' });
        }

        const content = fs.readFileSync(logFile, 'utf-8');
        const allLines = content.trim().split('\n').filter(Boolean);
        const tail = allLines.slice(-lines).map(l => {
            try { return JSON.parse(l); } catch { return l; }
        });

        res.json({ date: today, level, count: tail.length, lines: tail });
    } catch (err) {
        logger.error('Error leyendo logs', { error: err.message });
        res.status(500).json({ success: false, message: 'No se pudo leer el archivo de logs' });
    }
});

module.exports = router;