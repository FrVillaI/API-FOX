'use strict';

const express = require('express');
const router = express.Router();
const { state } = require('../state/state');

router.get('/qr', (req, res) => {
    if (!state.qr) {
        return res.status(404).json({
            success: false,
            message: state.status === 'ready'
                ? 'Ya autenticado, no hay QR disponible'
                : 'QR aún no generado, espera unos segundos',
        });
    }

    res.json({
        qr: state.qr,
        generatedAt: state.qrGeneratedAt,
    });
});

router.get('/qr-page', (req, res) => {
    res.send(`<!DOCTYPE html>
<html>
<head>
  <title>WhatsApp QR</title>
  <style>
    body { font-family: sans-serif; display: flex; flex-direction: column; align-items: center; padding: 40px; }
    img { border: 1px solid #ddd; border-radius: 8px; padding: 16px; }
    #status { margin-top: 16px; font-size: 14px; color: #666; }
  </style>
</head>
<body>
  <h2>Escanea con WhatsApp</h2>
  <img id="qr" style="width:280px; height:280px"/>
  <p id="status">Cargando QR...</p>
  <script>
    async function loadQR() {
      try {
        const res = await fetch('/qr');
        if (res.status === 404) {
          document.getElementById('status').textContent = 'Ya autenticado o QR no disponible';
          return;
        }
        const data = await res.json();
        document.getElementById('qr').src =
          'https://api.qrserver.com/v1/create-qr-code/?size=280x280&data=' + encodeURIComponent(data.qr);
        document.getElementById('status').textContent = 'QR generado a las ' + new Date(data.generatedAt).toLocaleTimeString();
        setTimeout(loadQR, 20000);
      } catch(e) {
        document.getElementById('status').textContent = 'Error: ' + e.message;
      }
    }
    loadQR();
  </script>
</body>
</html>`);
});

module.exports = router;