document.getElementById('userForm').addEventListener('submit', submitForm);
function checkForm() {
    const login = document.getElementById('login').value.trim();
    const password = document.getElementById('password').value.trim();
    const email = document.getElementById('email').value.trim();

    const submitButton = document.getElementById('submitButton');
    submitButton.disabled = !(login && password && email);
}
async function submitForm(event) {
    event.preventDefault();
    const form = document.getElementById('userForm');
    const formData = new FormData(form);

    const data = Object.fromEntries(formData.entries());

    try {
        const response = await fetch('/admin/users/add', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });
        if (response.ok) {
            const result = await response.json();

            const tbody = document.getElementById('usersTableBody');
            const newRow = `
                        <tr>
                            <td>${result.id}</td>
                            <td>${data.login}</td>
                            <td>${data.password}</td>
                            <td>${data.email}</td>
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

document.getElementById('deleteUserForm').addEventListener('submit', deleteUser);
function checkDeleteForm() {
    const deleteId = document.getElementById('deleteId').value.trim();
    const deleteButton = document.getElementById('deleteButton');
    deleteButton.disabled = !deleteId;
}
async function deleteUser(event) {
    event.preventDefault();

    const deleteId = document.getElementById('deleteId').value.trim();

    try {
        // Формируем JSON-объект для отправки
        const data = { id: deleteId };

        const response = await fetch('/admin/users/delete', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data) // Преобразуем объект в строку JSON
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {

                const tbody = document.getElementById('usersTableBody');
                const rows = tbody.getElementsByTagName('tr');

                // Удаляем строку из таблицы с указанным ID
                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');
                    if (cells.length > 0 && cells[0].textContent === deleteId) {
                        tbody.deleteRow(i);
                        break;
                    }
                }

                // Сброс формы и кнопки
                document.getElementById('deleteUserForm').reset();
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

document.getElementById('updateUserForm').addEventListener('submit', updateUser);
function checkUpdateForm() {
    const updateId = document.getElementById('updateId').value.trim();
    const updateLogin = document.getElementById('updateLogin').value.trim();
    const updatePassword = document.getElementById('updatePassword').value.trim();
    const updateEmail = document.getElementById('updateEmail').value.trim();

    const updateButton = document.getElementById('updateButton');
    updateButton.disabled = !(updateId && updateLogin && updatePassword && updateEmail);
}
async function updateUser(event) {
    event.preventDefault();

    const form = document.getElementById('updateUserForm');
    const formData = new FormData(form);

    const data = Object.fromEntries(formData.entries());
    data.id = Number(data.id);

    try {
        const response = await fetch('/admin/users/update', {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify(data)
        });

        if (response.ok) {
            const result = await response.json();

            if (result.success) {
                const tbody = document.getElementById('usersTableBody');
                const rows = tbody.getElementsByTagName('tr');

                // Обновляем строку с указанным ID
                for (let i = 0; i < rows.length; i++) {
                    const cells = rows[i].getElementsByTagName('td');
                    if (cells.length > 0 && cells[0].textContent === String(data.id)) {
                        cells[1].textContent = result.login;
                        cells[2].textContent = result.password;
                        cells[3].textContent = result.email;
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