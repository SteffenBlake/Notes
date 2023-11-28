import { initAsync as editorInitAsync } from "../../shared/ckEditor.js";
import { putProject } from '../../api/projects-api.js';
import { putNote } from '../../api/notes-api.js';
import { serializeForm, segmentPath } from '../../utilities.js';
import { Modal, Tab } from 'bootstrap';

export async function initAsync() 
{
    await editorInitAsync(document.getElementById(`new-project-description`));
    bindCreateProjectForm();
    bindCreateNoteForm();
    createNoteBreadcrumbs(window.location.pathname);
}

function bindCreateProjectForm() {
    const form = document.getElementById(`create-project-form`);
    form.addEventListener(`submit`,onCreateProject);
}

async function onCreateProject(e) {
    e.preventDefault();
    var data = serializeForm(e.target);

    await putProject(data.projectName, data);

    window.location.href = `/${data.projectName}`;

    return false;
}

function bindCreateNoteForm() {
    const form = document.getElementById(`create-note-form`);
    if (!form) {
        return;
    }
    form.addEventListener(`submit`, onCreateNote);

    var noteNameInput = document.getElementById(`create-note-name-input`);
    noteNameInput.addEventListener('input', onNoteNameUpdate);
}

async function onCreateNote(e) {
    e.preventDefault();
    const data = serializeForm(e.target);
    const path = `${data.path}/${data.name}`;

    await putNote(path, data);

    window.location.href = path;
    
    return false;
}

function onNoteNameUpdate(e) {
    const breadcrumbContainer = document.getElementById('create-note-breadcrumbs');
    const lastCrumb = breadcrumbContainer.lastChild;
    
    let text = e.target.value;
    if (!text) {
        text = "...";
    }
    lastCrumb.innerHTML = text;
}

export function ShowCreateNote(opts) {
    createNoteBreadcrumbs(opts.path);
    new Modal(`#create-new-modal`).show();
    var tab = document.getElementById(`create-note-tab`);
    if (!tab) {
        return;
    }
    new Tab(tab).show();

    document.querySelector(`#create-note-form [name="path"]`).value = opts.path;
}

export function createNoteBreadcrumbs(path) {
    var breadcrumbContainer = document.getElementById('create-note-breadcrumbs');
    if (!breadcrumbContainer) {
        return;
    }
    var segments = segmentPath(path);
    segments.push("...");
    
    var newCrumbs = [];
    for (const segment of segments) {
        const crumb = document.createElement(`li`);
        crumb.classList.add('breadcrumb-item');
        crumb.classList.add('active');
        crumb.innerHTML = segment;
        newCrumbs.push(crumb);
    }

    breadcrumbContainer.replaceChildren(...newCrumbs);
}