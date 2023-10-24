import { NotesEditor } from "./notes-editor.js";

import "./notes.scss";

NotesEditor
    .create(document.querySelector('#app'))
    .then(editor => {
        console.log('Editor was initialized', editor);
    })
    .catch(error => {
        console.error(error.stack);
    });

document.querySelectorAll('header>nav a').forEach(elem => {
    elem.addEventListener('focus', showMenu);
    elem.addEventListener('blur', hideMenu);
});

function showMenu()
{
    document.querySelector('header').classList.add('show');
}
function hideMenu() {
    document.querySelector('header').classList.remove('show');
}

