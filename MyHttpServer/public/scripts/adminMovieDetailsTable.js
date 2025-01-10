// Обработчик формы добавления
document.getElementById('addMovieDetailsForm').addEventListener('submit', addMovieDetails);

function checkAddForm() {
    const fields = [
        'movieDescription',
        'country',
        'producerId',
        'engTitle',
        'movieId',
        'quality',
        'videoUrl'
    ];
    const isValid = fields.every(id => document.getElementById(id).value.trim() !== '');
    document.getElementById('addSubmitButton').disabled = !isValid;
}

async function addMovieDetails(event) {
    event.preventDefault();
    const form = document.getElementById('addMovieDetailsForm');
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());
    data.movieid = +data.movieid;
    data.producerid = +data.producerid;

    try {
        const response = await fetch('/admin/movie-details/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            const tbody = document.getElementById('movieDetailsTableBody');
            const newRow = `
                <tr>
                    <td>${result.id}</td>
                    <td>${data.moviedescription}</td>
                    <td>${data.country}</td>
                    <td>${data.producerid}</td>
                    <td>${data.engtitle}</td>
                    <td>${data.movieid}</td>
                    <td>${data.quality}</td>
                    <td>${data.videourl}</td>
                </tr>`;
            tbody.innerHTML += newRow;

            form.reset();
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка добавления:', err);
        alert('Произошла ошибка при добавлении записи.');
    }
}

// Обработчик формы удаления
document.getElementById('deleteMovieDetailsForm').addEventListener('submit', deleteMovieDetails);

function checkDeleteForm() {
    const deleteId = document.getElementById('deleteId').value.trim();
    document.getElementById('deleteButton').disabled = !deleteId;
}

async function deleteMovieDetails(event) {
    event.preventDefault();
    let deleteId = Number(document.getElementById('deleteId').value.trim());
    deleteId=+deleteId;

    try {
        const response = await fetch('/admin/movie-details/delete', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({ id: deleteId })
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {
                const tbody = document.getElementById('movieDetailsTableBody');
                const rows = tbody.getElementsByTagName('tr');

                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');
                    if (cells.length > 0 && cells[0].textContent === String(deleteId)) {
                        tbody.deleteRow(i);
                        break;
                    }
                }

                document.getElementById('deleteMovieDetailsForm').reset();
                checkDeleteForm();
            } else {
                alert('Ошибка: ' + result.message);
            }
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка удаления:', err);
        alert('Произошла ошибка при удалении записи.');
    }
}

// Обработчик формы обновления
document.getElementById('updateMovieDetailsForm').addEventListener('submit', updateMovieDetails);

function checkUpdateForm() {
    const fields = [
        'updateId',
        'updateMovieDescription',
        'updateCountry',
        'updateProducerId',
        'updateEngTitle',
        'updateMovieId',
        'updateQuality',
        'updateVideoUrl'
    ];
    const isValid = fields.every(id => document.getElementById(id).value.trim() !== '');
    document.getElementById('updateButton').disabled = !isValid;
}

async function updateMovieDetails(event) {
    event.preventDefault();
    const form = document.getElementById('updateMovieDetailsForm');
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());
    data.id = Number(data.id);
    data.movieid = +data.movieid;
    data.producerid = +data.producerid;

    try {
        const response = await fetch('/admin/movie-details/update', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {
                const tbody = document.getElementById('movieDetailsTableBody');
                const rows = tbody.getElementsByTagName('tr');

                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');
                    if (cells.length > 0 && cells[0].textContent === String(data.id)) {
                        cells[1].textContent = data.moviedescription;
                        cells[2].textContent = data.country;
                        cells[3].textContent = data.producerid;
                        cells[4].textContent = data.engtitle;
                        cells[5].textContent = data.movieid;
                        cells[6].textContent = data.quality;
                        cells[7].textContent = data.videourl;
                        break;
                    }
                }

                form.reset();
                checkUpdateForm();
            } else {
                alert('Ошибка: ' + result.message);
            }
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка обновления:', err);
        alert('Произошла ошибка при обновлении записи.');
    }
}
