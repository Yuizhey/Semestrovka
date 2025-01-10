document.getElementById('addProducerForm').addEventListener('submit', submitProducerForm);
function checkForm() {
    const name = document.getElementById('name').value.trim();
    const directedFilmsCount = document.getElementById('directedFilmsCount').value.trim();
    const birthYear = document.getElementById('birthYear').value.trim();
    const birthCountry = document.getElementById('birthCountry').value.trim();

    const submitButton = document.getElementById('submitButton');
    submitButton.disabled = !(name && directedFilmsCount && birthYear && birthCountry);
}
async function submitProducerForm(event) {
    event.preventDefault();
    const form = document.getElementById('addProducerForm');
    const formData = new FormData(form);

    const data = Object.fromEntries(formData.entries());
    data.directedfilmscount = +data.directedfilmscount;
    data.birthyear = +data.birthyear;

    try {
        const response = await fetch('/admin/producer/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (response.ok) {
            const result = await response.json();

            const tbody = document.getElementById('producersTableBody');
            const newRow = `
                        <tr>
                            <td>${result.id}</td>
                            <td>${data.name}</td>
                            <td>${data.directedfilmscount}</td>
                            <td>${data.birthyear}</td>
                            <td>${data.birthcountry}</td>
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

document.getElementById('deleteProducerForm').addEventListener('submit', deleteProducer);
function checkDeleteProducerForm() {
    const deleteId = document.getElementById('deleteId').value.trim();
    const deleteButton = document.getElementById('deleteButton');
    deleteButton.disabled = !deleteId;
}
async function deleteProducer(event) {
    event.preventDefault();

    let deleteId = document.getElementById('deleteId').value.trim();
    deleteId = +deleteId;

    try {
        const data = { id: deleteId };

        const response = await fetch('/admin/producer/delete', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {

                const tbody = document.getElementById('producersTableBody');
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
                document.getElementById('deleteProducerForm').reset();
                checkDeleteProducerForm();
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


document.getElementById('updateProducerForm').addEventListener('submit', updateProducer);
function checkUpdateProducerForm() {
    const updateId = document.getElementById('updateId').value.trim();
    const updateName = document.getElementById('updateName').value.trim();
    const updateDirectedFilmsCount = document.getElementById('updateDirectedFilmsCount').value.trim();
    const updateBirthYear = document.getElementById('updateBirthYear').value.trim();
    const updateBirthCountry = document.getElementById('updateBirthCountry').value.trim();

    const updateButton = document.getElementById('updateButton');
    updateButton.disabled = !(updateId && updateName && updateDirectedFilmsCount && updateBirthYear && updateBirthCountry);
}
async function updateProducer(event) {
    event.preventDefault();

    const form = document.getElementById('updateProducerForm');
    const formData = new FormData(form);

    const data = Object.fromEntries(formData.entries());
    data.id = Number(data.id);
    data.directedfilmscount = Number(data.directedfilmscount);
    data.birthyear = Number(data.birthyear);

    try {
        const response = await fetch('/admin/producer/update', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {
                const tbody = document.getElementById('producersTableBody');
                const rows = tbody.getElementsByTagName('tr');

                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');
                    if (cells.length > 0 && cells[0].textContent === String(data.id)) {
                        cells[1].textContent = result.name;
                        cells[2].textContent = result.directedFilmsCount;
                        cells[3].textContent = result.birthYear;
                        cells[4].textContent = result.birthCountry;
                        break;
                    }
                }

                form.reset();
                checkUpdateProducerForm();
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
