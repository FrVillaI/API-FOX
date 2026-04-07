'use strict';

const express = require('express');
const router = express.Router();
const { state } = require('../state/state');

router.get('/ping', (req, res) => {
    res.json({ ok: true, ts: new Date().toISOString() });
});

router.get('/status', (req, res) => {
    const uptime = state.readySince
        ? Math.floor((Date.now() - new Date(state.readySince).getTime()) / 1000)
        : null;

    res.json({
        status: state.status,
        uptime: uptime !== null ? `${uptime}s` : null,
        readySince: state.readySince,
    });
});

module.exports = router;