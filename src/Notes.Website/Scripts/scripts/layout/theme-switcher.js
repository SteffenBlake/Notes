function getStoredTheme() {
    return localStorage.getItem('theme');
} 

function setStoredTheme(theme) {
    return localStorage.setItem('theme', theme);
} 

function getPreferredTheme() {
    var storedTheme = getStoredTheme();
    if (storedTheme) {
        return storedTheme;
    }

    return window.matchMedia('(prefers-color-scheme: dark)').matches ? 'dark' : 'light';
}

function setTheme(theme) {
    if (theme === 'auto' && window.matchMedia('(prefers-color-scheme: dark)').matches) {
        document.documentElement.setAttribute('data-bs-theme', 'dark')
    } else {
        document.documentElement.setAttribute('data-bs-theme', theme)
    }
}

function showActiveTheme(theme, focus = false) {
    const themeSwitcher = document.querySelector('#bd-theme')
    if (!themeSwitcher) {
        return
    }

    const themeSwitcherText = document.querySelector('#bd-theme-text')
    const activeThemeIcon = document.querySelector('.theme-icon-active use')
    const btnToActive = document.querySelector(`[data-bs-theme-value="${theme}"]`)
    const svgOfActiveBtn = btnToActive.querySelector('svg use').getAttribute('href')

    document.querySelectorAll('[data-bs-theme-value]').forEach(element => {
        element.classList.remove('active')
        element.setAttribute('aria-pressed', 'false')
    })

    btnToActive.classList.add('active')
    btnToActive.setAttribute('aria-pressed', 'true')
    activeThemeIcon.setAttribute('href', svgOfActiveBtn)
    const themeSwitcherLabel = `${themeSwitcherText.textContent} (${btnToActive.dataset.bsThemeValue})`
    themeSwitcher.setAttribute('aria-label', themeSwitcherLabel)

    if (focus) {
        themeSwitcher.focus()
    }
}

function onPrefersColorScheme() {
    const storedTheme = getStoredTheme()
    if (storedTheme !== 'light' && storedTheme !== 'dark') {
        setTheme(getPreferredTheme())
    }
}

function onThemeToggle(e) {
    const theme = e.srcElement.getAttribute('data-bs-theme-value');
    setStoredTheme(theme);
    setTheme(theme);
    showActiveTheme(theme, true);
}

export async function initAsync() {
    var preferred = getPreferredTheme();
    setTheme(preferred);

    window.matchMedia('(prefers-color-scheme: dark)').addEventListener('change', onPrefersColorScheme);

    showActiveTheme(getPreferredTheme());

    document.querySelectorAll('[data-bs-theme-value]')
        .forEach(toggle => toggle.addEventListener('click', onThemeToggle));
}