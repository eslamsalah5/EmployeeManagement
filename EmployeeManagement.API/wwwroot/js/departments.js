/**
 * departments.js — Departments CRUD
 */

let allDepartments = [];

// ─── Toast Notifications ─────────────────────────────────
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <span class="toast-icon">${type === 'success' ? '✓' : '✕'}</span>
        <span>${message}</span>`;
    container.appendChild(toast);
    setTimeout(() => { toast.style.opacity = '0'; toast.style.transform = 'translateX(20px)'; toast.style.transition = 'all .3s'; setTimeout(() => toast.remove(), 300); }, 3000);
}

// ─── Render Departments Table ─────────────────────────────
function renderDepartments(departments) {
    const tbody = document.getElementById('departmentsBody');

    if (!departments.length) {
        tbody.innerHTML = `
            <tr><td colspan="4">
                <div class="empty-state">
                    <div class="empty-state-icon">🏢</div>
                    <h3>No Departments Found</h3>
                    <p>Create your first department to get started</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = departments.map((dept, i) => {
        const colors = ['avatar-lime', 'avatar-yellow', 'avatar-pink', 'avatar-cyan', 'avatar-purple'];
        const colorClass = colors[i % colors.length];
        const initials = dept.name.substring(0, 2).toUpperCase();
        const date = new Date(dept.createdAt).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });

        return `
        <tr>
            <td data-label="Department">
                <div class="td-name">
                    <div class="td-avatar ${colorClass}">${initials}</div>
                    <span style="font-weight:600">${escapeHtml(dept.name)}</span>
                </div>
            </td>
            <td data-label="ID"><span class="badge badge-lime"># ${dept.id}</span></td>
            <td data-label="Created At" style="color:var(--text-muted)">${date}</td>
            <td data-label="Actions">
                <button class="btn btn-danger btn-sm" onclick="openDeleteModal(${dept.id}, '${escapeHtml(dept.name)}')">
                    🗑 Delete
                </button>
            </td>
        </tr>`;
    }).join('');
}

// ─── Load Departments ─────────────────────────────────────
async function loadDepartments() {
    const tbody = document.getElementById('departmentsBody');
    tbody.innerHTML = `<tr><td colspan="4"><div class="spinner-overlay"><div class="spinner"></div><p>Loading departments…</p></div></td></tr>`;

    const res = await apiFetch('/departments');
    if (!res) return;

    if (res.ok) {
        allDepartments = res.data.data || [];
        document.getElementById('statTotal').textContent = allDepartments.length;

        // Fill "Latest Added" stat
        const latestEl = document.getElementById('statLatest');
        if (latestEl) {
            if (allDepartments.length) {
                const latest = allDepartments[allDepartments.length - 1].name;
                latestEl.textContent = latest.length > 10 ? latest.slice(0, 10) + '…' : latest;
                latestEl.style.fontSize = '18px';
            } else {
                latestEl.textContent = '—';
            }
        }

        renderDepartments(allDepartments);
    } else {
        showToast(res.data.message || 'Failed to load departments', 'error');
    }
}

// ─── Search ───────────────────────────────────────────────
function setupSearch() {
    document.getElementById('searchInput').addEventListener('input', function () {
        const q = this.value.toLowerCase();
        const filtered = allDepartments.filter(d => d.name.toLowerCase().includes(q));
        renderDepartments(filtered);
    });
}

// ─── Create Modal ─────────────────────────────────────────
function openCreateModal() {
    document.getElementById('deptNameInput').value = '';
    document.getElementById('createAlert').classList.remove('show');
    document.getElementById('createModal').classList.add('show');
    document.getElementById('deptNameInput').focus();
}

function closeCreateModal() {
    document.getElementById('createModal').classList.remove('show');
}

async function submitCreate() {
    const name = document.getElementById('deptNameInput').value.trim();
    const alert = document.getElementById('createAlert');
    alert.classList.remove('show');

    if (!name) {
        alert.textContent = 'Department name is required';
        alert.classList.add('show');
        return;
    }

    const btn = document.getElementById('createSubmitBtn');
    btn.disabled = true;
    btn.textContent = 'Creating…';

    const res = await apiFetch('/departments', {
        method: 'POST',
        body: JSON.stringify({ name })
    });

    btn.disabled = false;
    btn.textContent = 'Create Department';

    if (res && res.ok) {
        closeCreateModal();
        showToast(`Department "${name}" created successfully!`);
        loadDepartments();
    } else {
        const errors = res?.data?.errors?.join(', ') || res?.data?.message || 'Failed to create department';
        alert.textContent = errors;
        alert.classList.add('show');
    }
}

// ─── Delete Modal ─────────────────────────────────────────
let deleteTargetId = null;

function openDeleteModal(id, name) {
    deleteTargetId = id;
    document.getElementById('deleteDeptName').textContent = name;
    document.getElementById('deleteModal').classList.add('show');
}

function closeDeleteModal() {
    document.getElementById('deleteModal').classList.remove('show');
    deleteTargetId = null;
}

async function confirmDelete() {
    if (!deleteTargetId) return;

    const btn = document.getElementById('deleteConfirmBtn');
    btn.disabled = true;
    btn.textContent = 'Deleting…';

    const res = await apiFetch(`/departments/${deleteTargetId}`, { method: 'DELETE' });

    btn.disabled = false;
    btn.textContent = 'Delete';

    if (res && res.ok) {
        closeDeleteModal();
        showToast('Department deleted successfully');
        loadDepartments();
    } else {
        closeDeleteModal();
        showToast(res?.data?.message || 'Failed to delete department', 'error');
    }
}

// ─── Helpers ──────────────────────────────────────────────
function escapeHtml(str) {
    const div = document.createElement('div');
    div.appendChild(document.createTextNode(str));
    return div.innerHTML;
}

// ─── Init ─────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', () => {
    if (!requireAuth()) return;
    loadDepartments();
    setupSearch();
});
