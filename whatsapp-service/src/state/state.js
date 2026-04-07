'use strict';

const state = {
    status: 'initializing', // initializing | qr | ready | disconnected | error
    qr: null,
    qrGeneratedAt: null,
    readySince: null,
    reconnectAttempts: 0,
    lastError: null,
    lastDisconnectReason: null,
    lastUpdatedAt: new Date().toISOString(),
};

function updateState(patch) {
    Object.assign(state, patch, { lastUpdatedAt: new Date().toISOString() });
}

module.exports = { state, updateState };