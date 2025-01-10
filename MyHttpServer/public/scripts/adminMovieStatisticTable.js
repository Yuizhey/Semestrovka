// Обработчик добавления записи
document.getElementById('addMovieStatisticForm').addEventListener('submit', submitAddForm);

function checkAddForm() {
    const kpRating = document.getElementById('kpRating').value.trim();
    const imdbRating = document.getElementById('imdbRating').value.trim();
    const likesCount = document.getElementById('likesCount').value.trim();
    const dislikesCount = document.getElementById('dislikesCount').value.trim();
    const movieId = document.getElementById('movieId').value.trim();

    const addSubmitButton = document.getElementById('addSubmitButton');
    addSubmitButton.disabled = !(kpRating && imdbRating && likesCount && dislikesCount && movieId);
}

async function submitAddForm(event) {
    event.preventDefault();

    const form = document.getElementById('addMovieStatisticForm');
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());
    data.likes_count = +data.likes_count;
    data.dislikes_count = +data.dislikes_count;
    data.movie_id = +data.movie_id;
    data.kp_rating = parseFloat(data.kp_rating);
    data.imdb_rating = parseFloat(data.imdb_rating);

    try {
        const response = await fetch('/admin/movie-stats/add', {
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
                    <td>${data.kp_rating}</td>
                    <td>${data.imdb_rating}</td>
                    <td>${data.likes_count}</td>
                    <td>${data.dislikes_count}</td>
                    <td>${data.movie_id}</td>
                </tr>`;
            tbody.innerHTML += newRow;

            form.reset();
            checkAddForm();
        } else {
            const error = await response.json();
            alert('Ошибка: ' + error.message);
        }
    } catch (err) {
        console.error('Ошибка отправки:', err);
        alert('Произошла ошибка при добавлении записи.');
    }
}

// Обработчик удаления записи
document.getElementById('deleteMovieStatisticForm').addEventListener('submit', submitDeleteForm);

function checkDeleteForm() {
    const deleteId = document.getElementById('deleteId').value.trim();
    const deleteButton = document.getElementById('deleteButton');
    deleteButton.disabled = !deleteId;
}

async function submitDeleteForm(event) {
    event.preventDefault();

    let deleteId = document.getElementById('deleteId').value.trim();
    deleteId = +deleteId;

    try {
        const response = await fetch('/admin/movie-stats/delete', {
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
                    if (cells.length > 0 && cells[0].textContent.trim() === deleteId.toString()) {
                        tbody.deleteRow(i);
                        break;
                    }
                }

                document.getElementById('deleteMovieStatisticForm').reset();
                checkDeleteForm();
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

// Обработчик обновления записи
document.getElementById('updateMovieStatisticForm').addEventListener('submit', submitUpdateForm);

function checkUpdateForm() {
    const updateId = document.getElementById('updateId').value.trim();
    const updateKpRating = document.getElementById('updateKpRating').value.trim();
    const updateImdbRating = document.getElementById('updateImdbRating').value.trim();
    const updateLikesCount = document.getElementById('updateLikesCount').value.trim();
    const updateDislikesCount = document.getElementById('updateDislikesCount').value.trim();
    const updateMovieId = document.getElementById('updateMovieId').value.trim();

    const updateButton = document.getElementById('updateButton');
    updateButton.disabled = !(updateId && updateKpRating && updateImdbRating && updateLikesCount && updateDislikesCount && updateMovieId);
}

async function submitUpdateForm(event) {
    event.preventDefault();

    const form = document.getElementById('updateMovieStatisticForm');
    const formData = new FormData(form);
    const data = Object.fromEntries(formData.entries());
    data.id = Number(data.id);
    data.likes_count = +data.likes_count;
    data.dislikes_count = +data.dislikes_count;
    data.movie_id = +data.movie_id;
    data.kp_rating = parseFloat(data.kp_rating);
    data.imdb_rating = parseFloat(data.imdb_rating);

    try {
        const response = await fetch('/admin/movie-stats/update', {
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
                    if (cells.length > 0 && cells[0].textContent.trim() === String(data.id)) {
                        cells[1].textContent = data.kp_rating;
                        cells[2].textContent = data.imdb_rating;
                        cells[3].textContent = data.likes_count;
                        cells[4].textContent = data.dislikes_count;
                        cells[5].textContent = data.movie_id;
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
        console.error('Ошибка отправки:', err);
        alert('Произошла ошибка при обновлении записи.');
    }
}
