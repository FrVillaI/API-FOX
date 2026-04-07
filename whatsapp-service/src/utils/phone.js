'use strict';

function validatePhone(phone) {
    const cleaned = phone.replace(/\D/g, '');
    return cleaned.length >= 10 && cleaned.length <= 15;
}

function normalizePhone(phone) {
    if (phone.includes('@c.us')) return phone;
    const cleaned = phone.replace(/\D/g, '');
    return `${cleaned}@c.us`;
}

module.exports = { validatePhone, normalizePhone };