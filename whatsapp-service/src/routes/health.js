'use strict';

const express = require('express');
const router = express.Router();
const { state } = require('../state/state');

router.get('/health', (req, res) => {
    res.json({
        service: {
            status: state.status,
            readySince: state.readySince,
            reconnectAttempts: state.reconnectAttempts,
            lastUpdatedAt: state.lastUpdatedAt,
        },
        lastDisconnectReason: state.lastDisconnectReason,
        lastError: state.lastError,
        process: {
            uptimeSeconds: Math.floor(process.uptime()),
            memoryMB: Math.round(process.memoryUsage().rss / 1024 / 1024),
            pid: process.pid,
            nodeVersion: process.version,
        },
    });
});

module.exports = router;