// Получаем элементы модальных окон
const authModal = document.getElementById("authModal");
const cabinetModal = document.getElementById("cabinetModal");
const openModalBtn = document.getElementById("openModal");
const closeAuthModalBtn = document.getElementById("closeModal");
const closeCabinetModalBtn = document.getElementById("closeCabinetModal");

// Открытие нужного модального окна
openModalBtn.onclick = function () {
    if (openModalBtn.innerText === "ВОЙТИ") {
        authModal.style.display = "block";
    } else if (openModalBtn.innerText === "КАБИНЕТ") {
        cabinetModal.style.display = "block";
    }
};

// Закрытие модальных окон
closeAuthModalBtn.onclick = function () {
    authModal.style.display = "none";
};

closeCabinetModalBtn.onclick = function () {
    cabinetModal.style.display = "none";
};


// Закрытие при клике вне модального окна
window.onclick = function (event) {
    if (event.target === authModal) {
        authModal.style.display = "none";
    } else if (event.target === cabinetModal) {
        cabinetModal.style.display = "none";
    }
};