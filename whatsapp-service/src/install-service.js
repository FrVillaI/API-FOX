const Service = require('node-windows').Service;

const svc = new Service({
    name: 'WhatsApp Facturacion',
    description: 'Servicio de envío de facturas por WhatsApp',
    script: require('path').join(__dirname, 'server.js'),
    nodeOptions: [],
    env: { name: 'NODE_ENV', value: 'production' }
});

svc.on('install', () => {
    svc.start();
    console.log('Servicio instalado e iniciado.');
});

svc.install();