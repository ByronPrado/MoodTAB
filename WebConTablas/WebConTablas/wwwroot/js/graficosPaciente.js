// wwwroot/js/graficosPaciente.js

const graficosPaciente = {
    instancias: {},

    crearYGuardar: function (nombre, fnCrear, ...args) {
        if (this.instancias[nombre]) {
            this.instancias[nombre].destroy();
        }
        this.instancias[nombre] = fnCrear(...args);
    },

    exportarPDF: async function () {
        if (!window.jspdf) {
            alert("La librería jsPDF no está cargada.");
            return;
        }
        const { jsPDF } = window.jspdf;
        const pdf = new jsPDF('p', 'pt', 'a4');
        let y = 20;

        for (const id in this.instancias) {
            const chart = this.instancias[id];
            if (!chart) continue;
            const canvas = chart.canvas;

            const imgData = canvas.toDataURL('image/png');
            const imgProps = pdf.getImageProperties(imgData);
            const pdfWidth = pdf.internal.pageSize.getWidth() - 40;
            const pdfHeight = (imgProps.height * pdfWidth) / imgProps.width;

            if (y + pdfHeight > pdf.internal.pageSize.getHeight()) {
                pdf.addPage();
                y = 20;
            }

            pdf.text(id.toUpperCase(), 20, y - 5);
            pdf.addImage(imgData, 'PNG', 20, y, pdfWidth, pdfHeight);
            y += pdfHeight + 40;
        }

        pdf.save("reporte_paciente.pdf");
    }
};

// Función para crear gráfico de barras de Pasos
function crearGraficoPasos(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Pasos realizados',
                data: data,
                backgroundColor: 'rgba(54, 162, 235, 0.6)'
            }]
        },
        options: {
            animation: false,
            scales: {
                y: { beginAtZero: true, title: { display: true, text: 'Cantidad de Pasos' } }
            },
            plugins: { legend: { display: false } }
        }
    });
}

// Función para crear gráfico de línea de Horas de Celular
function crearGraficoCelular(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    return new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Horas de uso',
                data: data,
                fill: false,
                borderColor: 'rgb(255, 99, 132)',
                tension: 0.1
            }]
        },
        options: {
            animation: false,
            scales: {
                y: { beginAtZero: true, title: { display: true, text: 'Horas de Uso' } }
            },
            plugins: { legend: { display: false } }
        }
    });
}

// Función para crear gráfico de línea de Horas en Redes Sociales
function crearGraficoRedes(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    return new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: [{
                label: 'Horas en redes',
                data: data,
                fill: false,
                borderColor: 'rgb(75, 192, 192)',
                tension: 0.1
            }]
        },
        options: {
            animation: false,
            scales: {
                y: { beginAtZero: true, title: { display: true, text: 'Horas de Uso' } }
            },
            plugins: { legend: { display: false } }
        }
    });
}

// Función para crear gráfico doughnut de distribución de estados
function crearGraficoEstados(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    return new Chart(ctx, {
        type: 'doughnut',
        data: {
            labels: labels,
            datasets: [{
                label: 'Distribución de Estados',
                data: data,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.7)', // Inhibido
                    'rgba(75, 192, 192, 0.7)', // Exaltado
                    'rgba(201, 203, 207, 0.7)' // Otro
                ]
            }]
        },
        options: {
            animation: false,
            responsive: true,
            plugins: {
                legend: { position: 'top' },
                tooltip: {
                    callbacks: {
                        label: context => `${context.label}: ${context.parsed}%`
                    }
                }
            }
        }
    });
}

// Función para crear gráfico de barras de Promedio de Pasos por Estado
function crearGraficoPromedioPasos(canvasId, labels, data) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    return new Chart(ctx, {
        type: 'bar',
        data: {
            labels: labels,
            datasets: [{
                label: 'Promedio Pasos',
                data: data,
                backgroundColor: [
                    'rgba(255, 99, 132, 0.7)', // Inhibido
                    'rgba(75, 192, 192, 0.7)'  // Exaltado
                ]
            }]
        },
        options: {
            animation: false,
            responsive: true,
            scales: {
                y: {
                    beginAtZero: true,
                    title: { display: true, text: 'Pasos promedio' }
                }
            },
            plugins: {
                legend: { display: false },
                tooltip: {
                    callbacks: {
                        label: ctx => `${ctx.dataset.label}: ${ctx.parsed.y.toFixed(0)} pasos`
                    }
                }
            }
        }
    });
}

// Función para crear gráfico de línea de tendencia de emociones
function crearGraficoTendenciaEmociones(canvasId, labels, emocionesKeys, emocionesData) {
    const ctx = document.getElementById(canvasId).getContext('2d');
    const colores = [
        'rgba(255, 99, 132, 0.7)',
        'rgba(54, 162, 235, 0.7)',
        'rgba(255, 206, 86, 0.7)',
        'rgba(75, 192, 192, 0.7)',
        'rgba(153, 102, 255, 0.7)',
        'rgba(255, 159, 64, 0.7)',
        'rgba(199, 199, 199, 0.7)'
    ];

    const datasets = emocionesKeys.map((emocion, idx) => ({
        label: emocion,
        data: emocionesData[emocion],
        borderColor: colores[idx % colores.length],
        fill: false,
        tension: 0.2
    }));

    return new Chart(ctx, {
        type: 'line',
        data: {
            labels: labels,
            datasets: datasets
        },
        options: {
            animation: false,
            responsive: true,
            scales: {
                y: { beginAtZero: true, title: { display: true, text: 'Intensidad' } }
            },
            plugins: {
                legend: { position: 'top' }
            }
        }
    });
}
