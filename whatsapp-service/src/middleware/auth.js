'use strict';

const CONFIG = require('../config');
const logger = require('../logger/logger');

// Middleware desactivado — el servicio corre en localhost.
// Para activarlo: reemplaza next() por la validación de API Key
// y agrega 'authenticate' como segundo argumento en cada ruta de app.js.
function authenticate(req, res, next) {
    // const key = req.headers['x-api-key'];
    // if (!key || key !== CONFIG.API_KEY) {
    //     logger.warn(`Acceso no autorizado desde ${req.ip} a ${req.path}`);
    //     return res.status(401).json({ success: false, message: 'No autorizado' });
    // }
    next();
}

module.exports = { authenticate };