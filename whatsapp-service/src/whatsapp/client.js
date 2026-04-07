'use strict';

const { Client, LocalAuth } = require('whatsapp-web.js');
const CONFIG = require('../config');

const client = new Client({
    authStrategy: new LocalAuth({
        clientId: 'facturacion-bot',
        dataPath: CONFIG.SESSION_DIR,
    }),
    puppeteer: {
        headless: true,
        args: [
            '--no-sandbox',
            '--disable-setuid-sandbox',
            '--disable-dev-shm-usage',
            '--disable-gpu',
            '--no-first-run',
            '--no-zygote',
            '--single-process',
        ],
    },
});

module.exports = client;