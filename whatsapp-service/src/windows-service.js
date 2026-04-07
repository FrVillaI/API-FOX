'use strict';

// Uso:
//   node windows-service.js install    (ejecutar como Administrador)
//   node windows-service.js uninstall  (ejecutar como Administrador)

const Service = require('node-windows').Service;
const path = require('path');

const svc = new Service({
    name: 'WhatsApp Facturacion',
    description: 'Servicio de envío de facturas por WhatsApp — Nexora',
    script: path.join(__dirname, 'server.js'),
    env: { name: 'NODE_ENV', value: 'production' },
});

const action = process.argv[2];

if (action === 'install') {
    svc.on('install', () => {
        svc.start();
        console.log('Servicio instalado e iniciado correctamente.');
        console.log('Puedes verlo en: services.msc > "WhatsApp Facturacion"');
    });
    svc.install();

} else if (action === 'uninstall') {
    svc.on('uninstall', () => {
        console.log('Servicio desinstalado correctamente.');
    });
    svc.uninstall();

} else {
    console.log('Uso: node windows-service.js [install|uninstall]');
    process.exit(1);
}