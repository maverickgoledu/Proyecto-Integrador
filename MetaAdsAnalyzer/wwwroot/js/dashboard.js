// Funcionalidades específicas para el Dashboard
document.addEventListener('DOMContentLoaded', function () {
    // Configurar colores para los gráficos
    const chartColors = {
        primary: '#4285F4',
        secondary: '#34A853',
        danger: '#EA4335',
        warning: '#FBBC05',
        info: '#46BDC6',
        primaryLight: 'rgba(66, 133, 244, 0.1)',
        secondaryLight: 'rgba(52, 168, 83, 0.1)',
        dangerLight: 'rgba(234, 67, 53, 0.1)',
        warningLight: 'rgba(251, 188, 5, 0.1)',
        infoLight: 'rgba(70, 189, 198, 0.1)'
    };

    // Función para obtener datos actualizados del dashboard
    function refreshDashboardData() {
        fetch('/Dashboard/GetDashboardData')
            .then(response => {
                if (!response.ok) {
                    throw new Error('Network response was not ok');
                }
                return response.json();
            })
            .then(data => {
                // Actualizar valores KPI
                document.getElementById('totalReach').textContent = formatNumber(data.totalReach);
                document.getElementById('totalImpressions').textContent = formatNumber(data.totalImpressions);
                document.getElementById('totalResults').textContent = formatNumber(data.totalResults);
                document.getElementById('totalSpend').textContent = formatCurrency(data.totalSpend);
                document.getElementById('avgCostPerResult').textContent = formatCurrency(data.averageCostPerResult);
                document.getElementById('conversionRate').textContent = formatPercent(data.conversionRate);

                // Actualizar gráficos
                updateCharts(data);
            })
            .catch(error => {
                console.error('Error al obtener datos del dashboard:', error);
            });
    }

    // Actualizar los gráficos con nuevos datos
    function updateCharts(data) {
        // Verificar si los gráficos existen
        if (window.budgetChart) {
            window.budgetChart.data.labels = Object.keys(data.campaignBudgetDistribution);
            window.budgetChart.data.datasets[0].data = Object.values(data.campaignBudgetDistribution);
            window.budgetChart.update();
        }

        if (window.spendChart) {
            window.spendChart.data.labels = Object.keys(data.campaignSpendDistribution);
            window.spendChart.data.datasets[0].data = Object.values(data.campaignSpendDistribution);
            window.spendChart.update();
        }

        if (window.resultsChart) {
            window.resultsChart.data.labels = Object.keys(data.campaignResultsDistribution);
            window.resultsChart.data.datasets[0].data = Object.values(data.campaignResultsDistribution);
            window.resultsChart.update();
        }
    }

    // Función para formatear números
    function formatNumber(value) {
        return new Intl.NumberFormat('es-ES').format(value);
    }

    // Función para formatear moneda
    function formatCurrency(value) {
        return new Intl.NumberFormat('es-ES', { style: 'currency', currency: 'USD' }).format(value);
    }

    // Función para formatear porcentajes
    function formatPercent(value) {
        return new Intl.NumberFormat('es-ES', { style: 'percent', minimumFractionDigits: 2 }).format(value / 100);
    }

    // Configurar actualización automática de datos cada 5 minutos
    const autoRefresh = false; // Cambiar a true para habilitar la actualización automática
    if (autoRefresh) {
        setInterval(refreshDashboardData, 300000); // 5 minutos
    }
});