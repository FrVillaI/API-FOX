module.exports = {
    apps: [
        {
            name: 'whatsapp-facturacion',
            script: './server.js',
            instances: 1,
            autorestart: true,
            watch: false,
            max_memory_restart: '500M',
            env: { NODE_ENV: 'development' },
            env_production: { NODE_ENV: 'production' },
            out_file: './logs/pm2-out.log',
            error_file: './logs/pm2-error.log',
            log_date_format: 'YYYY-MM-DD HH:mm:ss',
            merge_logs: true,
            restart_delay: 5000,
            max_restarts: 10,
            min_uptime: '30s',
            kill_timeout: 10000,
            listen_timeout: 15000,
        },
    ],
};