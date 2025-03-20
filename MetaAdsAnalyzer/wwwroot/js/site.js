// Script principal para la aplicación MetaAdsAnalyzer

// Función para toggle del sidebar en dispositivos móviles
function toggleSidebar() {
    const sidebar = document.querySelector('.sidebar');
    const mainContent = document.querySelector('.main-content');

    if (sidebar) {
        sidebar.classList.toggle('show');
    }

    if (mainContent) {
        mainContent.classList.toggle('shifted');
    }
}

// Configuración de tooltips para Bootstrap
document.addEventListener('DOMContentLoaded', function () {
    // Inicializar tooltips
    var tooltipTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="tooltip"]'));
    var tooltipList = tooltipTriggerList.map(function (tooltipTriggerEl) {
        return new bootstrap.Tooltip(tooltipTriggerEl);
    });

    // Cerrar alertas automáticamente después de 5 segundos
    const alerts = document.querySelectorAll('.alert-dismissible:not(.alert-danger)');
    alerts.forEach(alert => {
        setTimeout(() => {
            const closeButton = alert.querySelector('.btn-close');
            if (closeButton) {
                closeButton.click();
            }
        }, 5000);
    });

    // Inicializar dropdowns
    var dropdownTriggerList = [].slice.call(document.querySelectorAll('[data-bs-toggle="dropdown"]'));
    var dropdownList = dropdownTriggerList.map(function (dropdownTriggerEl) {
        return new bootstrap.Dropdown(dropdownTriggerEl);
    });

    // Añadir clase 'active' al enlace de navegación actual
    const currentPath = window.location.pathname;
    const navLinks = document.querySelectorAll('.navbar-nav .nav-link, .sidebar-menu .nav-link');

    navLinks.forEach(link => {
        const href = link.getAttribute('href');
        if (href && currentPath.startsWith(href) && href !== '/') {
            link.classList.add('active');
        } else if (href === '/' && currentPath === '/') {
            link.classList.add('active');
        }
    });
});

// Función para confirmar acciones destructivas
function confirmAction(message, form) {
    if (confirm(message)) {
        form.submit();
    }
    return false;
}

// Función para formatear números
function formatNumber(number, decimals = 0) {
    return new Intl.NumberFormat('es-ES', {
        minimumFractionDigits: decimals,
        maximumFractionDigits: decimals
    }).format(number);
}

// Función para formatear moneda
function formatCurrency(number) {
    return new Intl.NumberFormat('es-ES', {
        style: 'currency',
        currency: 'USD',
        minimumFractionDigits: 2,
        maximumFractionDigits: 2
    }).format(number);
}

// Función para formatear fechas
function formatDate(date) {
    if (!date) return '';
    const d = new Date(date);
    return d.toLocaleDateString('es-ES');
}

// Función para validar formularios
function validateForm(formId) {
    const form = document.getElementById(formId);
    if (!form) return true;

    if (!form.checkValidity()) {
        form.classList.add('was-validated');
        return false;
    }

    return true;
}

// Función para mostrar un mensaje de carga
function showLoading(message = 'Cargando...') {
    const loadingDiv = document.createElement('div');
    loadingDiv.className = 'loading-overlay';
    loadingDiv.innerHTML = `
        <div class="spinner-border text-primary" role="status">
            <span class="visually-hidden">Cargando...</span>
        </div>
        <div class="mt-2">${message}</div>
    `;

    document.body.appendChild(loadingDiv);
    document.body.classList.add('loading');
}

// Función para ocultar el mensaje de carga
function hideLoading() {
    const loadingDiv = document.querySelector('.loading-overlay');
    if (loadingDiv) {
        loadingDiv.remove();
        document.body.classList.remove('loading');
    }
}