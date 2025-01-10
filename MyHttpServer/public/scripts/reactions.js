let userReaction = null; // хранит текущую реакцию пользователя: 'like', 'dislike' или null

function handleVote(type) {
    const postId = new URLSearchParams(window.location.search).get('id');
    const likesCountElement = document.getElementById("likes-count");
    const dislikesCountElement = document.getElementById("dislikes-count");

    // Получаем текущее количество лайков и дизлайков
    let likesCount = parseInt(likesCountElement.innerText);
    let dislikesCount = parseInt(dislikesCountElement.innerText);
    let id = parseInt(postId);

    // Обновление данных в зависимости от типа голосования
    if (userReaction === type) {
        console.log('Пользователь уже проголосовал этой реакцией, ничего не делаем');
        return; // Если голосование уже было сделано, ничего не меняем
    }

    if (type === 'like') {
        if (userReaction === 'dislike') {
            // Если у пользователя уже есть дизлайк, уменьшаем его и увеличиваем лайк
            dislikesCount--;
        }
        likesCount++; // Добавляем к лайкам
    } else if (type === 'dislike') {
        if (userReaction === 'like') {
            // Если у пользователя уже есть лайк, уменьшаем его и увеличиваем дизлайк
            likesCount--;
        }
        dislikesCount++; // Добавляем к дизлайкам
    }

    // Обновляем текущую реакцию пользователя
    userReaction = type;

    // Форма данных для отправки на сервер
    const reactionData = {
        id: id,
        likescount: likesCount,
        dislikescount: dislikesCount
    };

    fetch('card/reaction', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(reactionData)
    })
        .then(response => response.json())
        .then(data => {
            if (data.success) {
                // Обновляем количество лайков и дизлайков
                likesCountElement.innerText = data.likescount;
                dislikesCountElement.innerText = data.dislikescount;

                // Делаем неактивным блок для лайков или дизлайков в зависимости от типа
                if (type === 'like') {
                    document.querySelector('.main-marks-positive').style.pointerEvents = 'none';
                    document.querySelector('.main-marks-negative').style.pointerEvents = 'auto';
                } else if (type === 'dislike') {
                    document.querySelector('.main-marks-negative').style.pointerEvents = 'none';
                    document.querySelector('.main-marks-positive').style.pointerEvents = 'auto';
                }
            } else {
                alert('Ошибка:'+ data.message);
            }
        })
        .catch(error => {
            alert('Ошибка:', error);
        });
}