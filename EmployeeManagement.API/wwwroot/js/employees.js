/**
 * employees.js — Employees CRUD + Search
 */

let allDepts = [];
let editingId = null;

// ─── Toast ────────────────────────────────────────────────
function showToast(message, type = 'success') {
    const container = document.getElementById('toastContainer');
    const toast = document.createElement('div');
    toast.className = `toast toast-${type}`;
    toast.innerHTML = `
        <span class="toast-icon">${type === 'success' ? '✓' : '✕'}</span>
        <span>${message}</span>`;
    container.appendChild(toast);
    setTimeout(() => {
        toast.style.opacity = '0';
        toast.style.transform = 'translateX(20px)';
        toast.style.transition = 'all .3s';
        setTimeout(() => toast.remove(), 300);
    }, 3000);
}

// ─── Avatar Color Helper ──────────────────────────────────
function getAvatarClass(name) {
    const classes = ['avatar-lime', 'avatar-yellow', 'avatar-pink', 'avatar-cyan', 'avatar-purple'];
    let hash = 0;
    for (let c of name) hash += c.charCodeAt(0);
    return classes[hash % classes.length];
}

function getInitials(name) {
    return name.split(' ').slice(0, 2).map(w => w[0]).join('').toUpperCase();
}

// ─── Format Date ─────────────────────────────────────────
function formatDate(dateStr) {
    return new Date(dateStr).toLocaleDateString('en-US', { year: 'numeric', month: 'short', day: 'numeric' });
}

// ─── Escape HTML ─────────────────────────────────────────
function escapeHtml(str) {
    const div = document.createElement('div');
    div.appendChild(document.createTextNode(String(str || '')));
    return div.innerHTML;
}

// ─── Load Departments for dropdowns ──────────────────────
async function loadDepartments() {
    const res = await apiFetch('/departments');
    if (res && res.ok) {
        allDepts = res.data.data || [];
        populateDeptDropdowns();
    }
}

function populateDeptDropdowns() {
    const options = `<option value="">All Departments</option>` +
        allDepts.map(d => `<option value="${d.id}">${escapeHtml(d.name)}</option>`).join('');

    document.getElementById('filterDept').innerHTML = options;

    const modalOptions = `<option value="">Select Department</option>` +
        allDepts.map(d => `<option value="${d.id}">${escapeHtml(d.name)}</option>`).join('');

    document.getElementById('empDepartmentId').innerHTML = modalOptions;
    document.getElementById('editDepartmentId').innerHTML = modalOptions;
}

// ─── Render Table ───────────────────────────────────────
function renderEmployees(employees) {
    const tbody = document.getElementById('employeesBody');

    if (!employees.length) {
        tbody.innerHTML = `
            <tr><td colspan="7">
                <div class="empty-state">
                    <div class="empty-state-icon">👥</div>
                    <h3>No Employees Found</h3>
                    <p>Try adjusting your search or add a new employee</p>
                </div>
            </td></tr>`;
        return;
    }

    tbody.innerHTML = employees.map(emp => {
        const initials = getInitials(emp.fullName);
        const avatarClass = getAvatarClass(emp.fullName);
        const statusBadge = emp.isActive
            ? `<span class="badge badge-active">● Active</span>`
            : `<span class="badge badge-inactive">● Inactive</span>`;
        const deptBadge = emp.departmentName
            ? `<span class="badge badge-yellow">${escapeHtml(emp.departmentName)}</span>`
            : `<span class="badge badge-pink">Unassigned</span>`;

        return `
        <tr>
            <td data-label="Name">
                <div class="td-name">
                    <div class="td-avatar ${avatarClass}">${initials}</div>
                    <div>
                        <div style="font-weight:600">${escapeHtml(emp.fullName)}</div>
                        <div style="font-size:12px;color:var(--text-muted)">${escapeHtml(emp.email)}</div>
                    </div>
                </div>
            </td>
            <td data-label="Job Title" style="color:var(--text-muted)">${escapeHtml(emp.jobTitle)}</td>
            <td data-label="Department">${deptBadge}</td>
            <td data-label="Mobile" style="font-size:13px">${escapeHtml(emp.mobileNumber)}</td>
            <td data-label="Hire Date" style="color:var(--text-muted);font-size:13px">${formatDate(emp.hireDate)}</td>
            <td data-label="Status">${statusBadge}</td>
            <td data-label="Actions">
                <div style="display:flex;gap:6px">
                    <button class="btn btn-outline-secondary btn-sm" onclick="openEditModal(${emp.id})">✏️ Edit</button>
                    <button class="btn btn-danger btn-sm" onclick="openDeleteModal(${emp.id}, '${escapeHtml(emp.fullName)}')">🗑</button>
                </div>
            </td>
        </tr>`;
    }).join('');
}

// ─── Load Employees ───────────────────────────────────────
async function loadEmployees(search = '', departmentId = '') {
    const tbody = document.getElementById('employeesBody');
    tbody.innerHTML = `<tr><td colspan="7"><div class="spinner-overlay"><div class="spinner"></div><p>Loading employees…</p></div></td></tr>`;

    let url = '/employees';
    const params = new URLSearchParams();
    if (search) params.set('search', search);
    if (departmentId) params.set('departmentId', departmentId);
    if ([...params].length) url += '?' + params.toString();

    const res = await apiFetch(url);
    if (!res) return;

    if (res.ok) {
        const employees = res.data.data || [];
        document.getElementById('statTotal').textContent = employees.length;
        renderEmployees(employees);
    } else {
        showToast(res.data.message || 'Failed to load employees', 'error');
    }
}

// ─── Search & Filter ──────────────────────────────────────
function setupFilters() {
    let searchTimeout;
    document.getElementById('searchInput').addEventListener('input', function () {
        clearTimeout(searchTimeout);
        searchTimeout = setTimeout(() => applyFilters(), 400);
    });
    document.getElementById('filterDept').addEventListener('change', applyFilters);
}

function applyFilters() {
    const search = document.getElementById('searchInput').value.trim();
    const deptId = document.getElementById('filterDept').value;
    loadEmployees(search, deptId);
}

// ─── Create Modal ─────────────────────────────────────────
function openCreateModal() {
    editingId = null;
    document.getElementById('modalTitle').textContent = '+ Add New Employee';
    document.getElementById('createAlert').classList.remove('show');
    document.getElementById('empForm').reset();
    document.getElementById('empModal').classList.add('show');
    document.getElementById('empFullName').focus();
}

function closeCreateModal() {
    document.getElementById('empModal').classList.remove('show');
}

async function submitCreate() {
    const alert = document.getElementById('createAlert');
    alert.classList.remove('show');

    const payload = {
        fullName:     document.getElementById('empFullName').value.trim(),
        email:        document.getElementById('empEmail').value.trim(),
        mobileNumber: document.getElementById('empMobile').value.trim(),
        jobTitle:     document.getElementById('empJobTitle').value.trim(),
        hireDate:     document.getElementById('empHireDate').value,
        departmentId: parseInt(document.getElementById('empDepartmentId').value)
    };

    if (!payload.fullName || !payload.email || !payload.mobileNumber || !payload.jobTitle || !payload.hireDate || !payload.departmentId) {
        alert.textContent = 'Please fill all required fields';
        alert.classList.add('show');
        return;
    }

    const btn = document.getElementById('createSubmitBtn');
    btn.disabled = true;
    btn.textContent = 'Saving…';

    const res = await apiFetch('/employees', {
        method: 'POST',
        body: JSON.stringify(payload)
    });

    btn.disabled = false;
    btn.textContent = 'Add Employee';

    if (res && res.ok) {
        closeCreateModal();
        showToast(`Employee "${payload.fullName}" added successfully!`);
        applyFilters();
    } else {
        const errors = res?.data?.errors?.join('\n') || res?.data?.message || 'Failed to create employee';
        alert.textContent = errors;
        alert.classList.add('show');
    }
}

// ─── Edit Modal ───────────────────────────────────────────
async function openEditModal(id) {
    const res = await apiFetch(`/employees/${id}`);
    if (!res || !res.ok) { showToast('Failed to load employee data', 'error'); return; }

    const emp = res.data.data;
    editingId = id;

    document.getElementById('editAlert').classList.remove('show');
    document.getElementById('editFullName').value = emp.fullName;
    document.getElementById('editEmail').value = emp.email;
    document.getElementById('editMobile').value = emp.mobileNumber;
    document.getElementById('editJobTitle').value = emp.jobTitle;
    document.getElementById('editHireDate').value = emp.hireDate ? emp.hireDate.substring(0, 10) : '';
    document.getElementById('editDepartmentId').value = emp.departmentId;
    document.getElementById('editIsActive').checked = emp.isActive;

    document.getElementById('editModal').classList.add('show');
}

function closeEditModal() {
    document.getElementById('editModal').classList.remove('show');
    editingId = null;
}

async function submitEdit() {
    if (!editingId) return;
    const alert = document.getElementById('editAlert');
    alert.classList.remove('show');

    const payload = {
        fullName:     document.getElementById('editFullName').value.trim(),
        email:        document.getElementById('editEmail').value.trim(),
        mobileNumber: document.getElementById('editMobile').value.trim(),
        jobTitle:     document.getElementById('editJobTitle').value.trim(),
        hireDate:     document.getElementById('editHireDate').value,
        departmentId: parseInt(document.getElementById('editDepartmentId').value),
        isActive:     document.getElementById('editIsActive').checked
    };

    const btn = document.getElementById('editSubmitBtn');
    btn.disabled = true;
    btn.textContent = 'Saving…';

    const res = await apiFetch(`/employees/${editingId}`, {
        method: 'PUT',
        body: JSON.stringify(payload)
    });

    btn.disabled = false;
    btn.textContent = 'Save Changes';

    if (res && res.ok) {
        closeEditModal();
        showToast('Employee updated successfully!');
        applyFilters();
    } else {
        const errors = res?.data?.errors?.join('\n') || res?.data?.message || 'Failed to update employee';
        alert.textContent = errors;
        alert.classList.add('show');
    }
}

// ─── Delete Modal ─────────────────────────────────────────
let deleteTargetId = null;

function openDeleteModal(id, name) {
    deleteTargetId = id;
    document.getElementById('deleteEmpName').textContent = name;
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

    const res = await apiFetch(`/employees/${deleteTargetId}`, { method: 'DELETE' });

    btn.disabled = false;
    btn.textContent = 'Delete';

    if (res && res.ok) {
        closeDeleteModal();
        showToast('Employee deleted successfully');
        applyFilters();
    } else {
        closeDeleteModal();
        showToast(res?.data?.message || 'Failed to delete employee', 'error');
    }
}

// ─── Init ─────────────────────────────────────────────────
document.addEventListener('DOMContentLoaded', async () => {
    if (!requireAuth()) return;
    await loadDepartments();
    await loadEmployees();
    setupFilters();
});
