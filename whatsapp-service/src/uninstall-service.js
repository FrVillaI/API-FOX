const Service = require('node-windows').Service;

const svc = new Service({
    name: 'WhatsApp Facturacion',
    script: require('path').join(__dirname, 'server.js'), 
});

svc.on('uninstall', () => console.log('Servicio desinstalado.'));
svc.uninstall();