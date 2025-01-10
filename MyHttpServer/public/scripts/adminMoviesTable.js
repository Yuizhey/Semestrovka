document.getElementById('addMovieForm').addEventListener('submit', submitAddMovieForm);

function checkAddForm() {
    const ruTitle = document.getElementById('ruTitle').value.trim();
    const releaseYear = document.getElementById('releaseYear').value.trim();
    const imageSource = document.getElementById('imageSource').value.trim();
    const status = document.getElementById('status').value.trim();

    const submitButton = document.getElementById('addSubmitButton');
    submitButton.disabled = !(ruTitle && releaseYear && imageSource && status);
}

async function submitAddMovieForm(event) {
    event.preventDefault();
    const form = document.getElementById('addMovieForm');
    const formData = new FormData(form);

    const data = Object.fromEntries(formData.entries());
    data.releaseyear = +data.releaseyear;

    try {
        const response = await fetch('/admin/movies/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            const tbody = document.getElementById('moviesTableBody');
            const newRow = `
                <tr>
                    <td>${result.id}</td>
                    <td>${data.rutitle}</td>
                    <td>${data.releaseyear}</td>
                    <td>${data.imagesource}</td>
                    <td>${data.status}</td>
                </tr>`;
            tbody.innerHTML += newRow;

            form.reset();
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка отправки:', err);
        alert('Произошла ошибка при добавлении записи.');
    }
}

document.getElementById('deleteMovieForm').addEventListener('submit', deleteMovie);

function checkDeleteMovieForm() {
    const deleteId = document.getElementById('deleteId').value.trim();
    const deleteButton = document.getElementById('deleteButton');
    deleteButton.disabled = !deleteId;
}

async function deleteMovie(event) {
    event.preventDefault();

    let deleteId = document.getElementById('deleteId').value.trim();
    deleteId = +deleteId;

    try {
        // Формируем JSON-объект для отправки
        const data = { id: deleteId };

        const response = await fetch('/admin/movies/delete', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data) // Преобразуем объект в строку JSON
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {

                const tbody = document.getElementById('moviesTableBody');
                const rows = tbody.getElementsByTagName('tr');

                // Ищем строку с нужным ID, проверяя только первую ячейку
                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');

                    // Проверяем только первую ячейку на совпадение с ID
                    if (cells.length > 0 && cells[0].textContent.trim() === deleteId.toString()) {
                        tbody.deleteRow(i);
                        break; // Прерываем цикл после удаления строки
                    }
                }

                // Сброс формы и кнопки
                document.getElementById('deleteMovieForm').reset();
                checkDeleteMovieForm();
            } else {
                alert('Ошибка: ' + result.message);
            }
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка отправки:', err);
        alert('Произошла ошибка при удалении записи.');
    }
}

document.getElementById('updateMovieForm').addEventListener('submit', updateMovie);

function checkUpdateMovieForm() {
    const updateId = document.getElementById('updateId').value.trim();
    const updateRuTitle = document.getElementById('updateRuTitle').value.trim();
    const updateReleaseYear = document.getElementById('updateReleaseYear').value.trim();
    const updateImageSource = document.getElementById('updateImageSource').value.trim();
    const updateStatus = document.getElementById('updateStatus').value.trim();

    const updateButton = document.getElementById('updateButton');
    updateButton.disabled = !(updateId && updateRuTitle && updateReleaseYear && updateImageSource && updateStatus);
}

async function updateMovie(event) {
    event.preventDefault();

    const form = document.getElementById('updateMovieForm');
    const formData = new FormData(form);

    const data = Object.fromEntries(formData.entries());
    data.id = Number(data.id);
    data.releaseyear = +data.releaseyear;

    try {
        const response = await fetch('/admin/movies/update', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {
                const tbody = document.getElementById('moviesTableBody');
                const rows = tbody.getElementsByTagName('tr');

                // Обновляем строку с указанным ID
                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');
                    if (cells.length > 0 && cells[0].textContent === String(data.id)) {
                        cells[1].textContent = result.ruTitle;
                        cells[2].textContent = result.releaseYear;
                        cells[3].textContent = result.imageSource;
                        cells[4].textContent = result.status;
                        break;
                    }
                }

                form.reset();
                checkUpdateMovieForm();
            } else {
                alert('Ошибка: ' + result.message);
            }
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка отправки:', err);
        alert('Произошла ошибка при обновлении записи.');
    }
}

