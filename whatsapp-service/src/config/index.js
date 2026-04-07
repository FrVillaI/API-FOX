'use strict';

require('dotenv').config();

const CONFIG = {
    PORT: parseInt(process.env.PORT) || 3000,
    API_KEY: process.env.API_KEY || 'no-usado-por-ahora',
    NODE_ENV: process.env.NODE_ENV || 'development',
    MAX_RECONNECT_ATTEMPTS: parseInt(process.env.MAX_RECONNECT_ATTEMPTS) || 5,
    RECONNECT_DELAY_MS: parseInt(process.env.RECONNECT_DELAY_MS) || 5000,
    LOG_LEVEL: process.env.LOG_LEVEL || 'info',
    LOG_DIR: process.env.LOG_DIR || './logs',
    SESSION_DIR: process.env.SESSION_DIR || './sessions',
};

module.exports = CONFIG;