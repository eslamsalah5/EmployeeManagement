/**
 * auth.js — JWT login, storage, and route guard
 */

const API_BASE = '/api';
const TOKEN_KEY = 'emp_mgmt_token';

// ── Storage ──────────────────────────────────────────────
function saveToken(token) {
    localStorage.setItem(TOKEN_KEY, token);
}

function getToken() {
    return localStorage.getItem(TOKEN_KEY);
}

function removeToken() {
    localStorage.removeItem(TOKEN_KEY);
}

// ── Auth Headers ─────────────────────────────────────────
function authHeaders() {
    return {
        'Content-Type': 'application/json',
        'Authorization': `Bearer ${getToken()}`
    };
}

// ── Route Guard ───────────────────────────────────────────
function requireAuth() {
    const token = getToken();
    if (!token) {
        window.location.href = '/index.html';
        return false;
    }
    return true;
}

function requireGuest() {
    if (getToken()) {
        window.location.href = '/employees.html';
    }
}

// ── Login ─────────────────────────────────────────────────
async function login(email, password) {
    const response = await fetch(`${API_BASE}/auth/login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ email, password })
    });

    const result = await response.json();

    if (!response.ok || !result.success) {
        throw new Error(result.message || 'Login failed');
    }

    saveToken(result.data.token);
    return result.data;
}

// ── Logout ────────────────────────────────────────────────
function logout() {
    removeToken();
    window.location.href = '/index.html';
}

// ── Generic Fetch Wrapper ────────────────────────────────
async function apiFetch(url, options = {}) {
    const response = await fetch(`${API_BASE}${url}`, {
        ...options,
        headers: authHeaders()
    });

    if (response.status === 401) {
        logout();
        return;
    }

    const result = await response.json();
    return { ok: response.ok, data: result };
}
