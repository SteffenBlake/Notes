import { ShowCreateNote } from './create-new-modal.js';

export async function initAsync() 
{
    bindCreateButton();
}

function bindCreateButton() {
    var createButton = document.getElementById(`create-new-btn`);
    createButton.addEventListener('click', onCreateButton);
}

function onCreateButton() {
    ShowCreateNote({
        path: window.location.pathname
    });
}