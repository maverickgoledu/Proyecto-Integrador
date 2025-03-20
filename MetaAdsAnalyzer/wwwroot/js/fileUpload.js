// Funcionalidades para la carga de archivos
document.addEventListener('DOMContentLoaded', function () {
    // Seleccionar elementos relevantes
    const fileInput = document.querySelector('input[type="file"]');
    const form = document.querySelector('form[enctype="multipart/form-data"]');
    const submitBtn = form ? form.querySelector('button[type="submit"]') : null;
    const progressContainer = document.getElementById('uploadProgressContainer');
    const progressBar = document.getElementById('uploadProgressBar');
    const progressText = document.getElementById('uploadProgressText');

    // Verificar si están disponibles los elementos necesarios
    if (!fileInput || !form) return;

    // Añadir validación al cambiar el archivo
    fileInput.addEventListener('change', function (e) {
        validateFile(this);
    });

    // Validar archivo seleccionado
    function validateFile(input) {
        if (input.files && input.files[0]) {
            const file = input.files[0];
            const fileName = file.name;
            const fileSize = file.size;
            const fileType = file.type;
            const fileExtension = fileName.split('.').pop().toLowerCase();

            // Verificar extensión permitida
            const allowedExtensions = ['csv', 'xlsx', 'xls'];
            if (!allowedExtensions.includes(fileExtension)) {
                showError('Tipo de archivo no permitido. Por favor seleccione un archivo CSV o Excel.');
                input.value = '';
                return false;
            }

            // Verificar tamaño máximo (10 MB)
            const maxSize = 10 * 1024 * 1024; // 10 MB en bytes
            if (fileSize > maxSize) {
                showError('El archivo es demasiado grande. El tamaño máximo es 10 MB.');
                input.value = '';
                return false;
            }

            // Mostrar información del archivo
            showFileInfo(fileName, fileSize);
            enableSubmitButton();
            return true;
        } else {
            disableSubmitButton();
            return false;
        }
    }

    // Mostrar mensaje de error
    function showError(message) {
        // Crear o actualizar elemento de alerta
        let alertEl = document.getElementById('fileUploadAlert');

        if (!alertEl) {
            alertEl = document.createElement('div');
            alertEl.id = 'fileUploadAlert';
            alertEl.className = 'alert alert-danger mt-3';
            alertEl.role = 'alert';
            fileInput.parentNode.appendChild(alertEl);
        }

        alertEl.textContent = message;
    }

    // Mostrar información del archivo
    function showFileInfo(name, size) {
        let sizeString = '';

        if (size < 1024) {
            sizeString = size + ' bytes';
        } else if (size < 1024 * 1024) {
            sizeString = (size / 1024).toFixed(2) + ' KB';
        } else {
            sizeString = (size / (1024 * 1024)).toFixed(2) + ' MB';
        }

        // Crear o actualizar elemento de info
        let infoEl = document.getElementById('fileUploadInfo');

        if (!infoEl) {
            infoEl = document.createElement('div');
            infoEl.id = 'fileUploadInfo';
            infoEl.className = 'alert alert-info mt-3';
            fileInput.parentNode.appendChild(infoEl);
        }

        infoEl.textContent = 'Archivo seleccionado: ' + name + ' (' + sizeString + ')';

        // Ocultar mensaje de error si existe
        const alertEl = document.getElementById('fileUploadAlert');
        if (alertEl) {
            alertEl.style.display = 'none';
        }
    }

    // Habilitar botón de enviar
    function enableSubmitButton() {
        if (submitBtn) {
            submitBtn.disabled = false;
        }
    }

    // Deshabilitar botón de enviar
    function disableSubmitButton() {
        if (submitBtn) {
            submitBtn.disabled = true;
        }
    }

    // Configurar envío de formulario con barra de progreso
    if (form) {
        form.addEventListener('submit', function (e) {
            if (!validateFile(fileInput)) {
                e.preventDefault();
                return false;
            }

            // Si no hay contenedor de progreso, simplemente dejar que el formulario se envíe normalmente
            if (!progressContainer) return true;

            // Evitar envío normal y usar AJAX con barra de progreso
            e.preventDefault();

            // Mostrar barra de progreso
            progressContainer.style.display = 'block';
            progressBar.style.width = '0%';
            progressBar.setAttribute('aria-valuenow', 0);
            progressText.textContent = '0%';

            // Crear FormData y enviar con XMLHttpRequest para poder mostrar progreso
            const formData = new FormData(form);
            const xhr = new XMLHttpRequest();

            xhr.open('POST', form.action, true);

            // Actualizar barra de progreso
            xhr.upload.onprogress = function (e) {
                if (e.lengthComputable) {
                    const percentComplete = (e.loaded / e.total) * 100;
                    const percentValue = Math.round(percentComplete) + '%';

                    progressBar.style.width = percentValue;
                    progressBar.setAttribute('aria-valuenow', Math.round(percentComplete));
                    progressText.textContent = percentValue;
                }
            };

            // Manejar respuesta
            xhr.onload = function () {
                if (xhr.status === 200) {
                    // Redirigir a la misma página o actualizar contenido
                    window.location.href = window.location.href;
                } else {
                    showError('Error al procesar el archivo: ' + xhr.statusText);
                    progressContainer.style.display = 'none';
                }
            };

            xhr.onerror = function () {
                showError('Error en la conexión al intentar subir el archivo.');
                progressContainer.style.display = 'none';
            };

            // Enviar datos
            xhr.send(formData);
            return false;
        });
    }

    // Inicializar en estado deshabilitado si no hay archivo seleccionado
    if (submitBtn) {
        submitBtn.disabled = !fileInput.files.length;
    }
});