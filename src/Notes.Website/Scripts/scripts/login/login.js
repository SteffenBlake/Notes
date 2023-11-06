async function init() {
    const urlParams = new URLSearchParams(window.location.search);
    const returnUrl = urlParams.get('returnUrl');

    document.getElementById('return-url-input').value = returnUrl;
}

(async () => init())();