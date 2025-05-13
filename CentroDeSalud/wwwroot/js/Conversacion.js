document.addEventListener('input', function (event) {
    if (event.target.classList.contains('auto-grow')) {
        event.target.style.height = 'auto';
        event.target.style.height = (event.target.scrollHeight) + 'px';
    }
});