'use strict';

const winston = require('winston');
const DailyRotateFile = require('winston-daily-rotate-file');
const fs = require('fs');
const path = require('path');
const CONFIG = require('../config');

if (!fs.existsSync(CONFIG.LOG_DIR)) {
    fs.mkdirSync(CONFIG.LOG_DIR, { recursive: true });
}

const logger = winston.createLogger({
    level: CONFIG.LOG_LEVEL,
    format: winston.format.combine(
        winston.format.timestamp({ format: 'YYYY-MM-DD HH:mm:ss' }),
        winston.format.errors({ stack: true }),
        winston.format.json()
    ),
    transports: [
        new DailyRotateFile({
            filename: path.join(CONFIG.LOG_DIR, 'error-%DATE%.log'),
            datePattern: 'YYYY-MM-DD',
            level: 'error',
            maxSize: '20m',
            maxFiles: '30d',
            zippedArchive: true,
        }),
        new DailyRotateFile({
            filename: path.join(CONFIG.LOG_DIR, 'combined-%DATE%.log'),
            datePattern: 'YYYY-MM-DD',
            maxSize: '20m',
            maxFiles: '30d',
            zippedArchive: true,
        }),
    ],
});

if (CONFIG.NODE_ENV !== 'production') {
    logger.add(new winston.transports.Console({
        format: winston.format.combine(
            winston.format.colorize(),
            winston.format.simple()
        )
    }));
}

module.exports = logger;