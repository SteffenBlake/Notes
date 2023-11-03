import { indexProjects, putProject } from '../api/projects-api.js';

document.addEventListener("DOMContentLoaded", onLoaded);

async function onLoaded() {
    var indexPromise = indexProjects();

    const container = document.getElementById(`projects-container`);
    const projectBoxTemplate = document.getElementById(`project-box`);

    var projects = await indexPromise;
    projects.data.forEach(project => {
        var projectBox = projectBoxTemplate.content.cloneNode(true);
        projectBox.querySelector(`h1`).innerHTML = project.name;
        projectBox.querySelector(`a`).href = `/${project.name}`;
        container.appendChild(projectBox);
    });

    var addNew = projectBoxTemplate.content.cloneNode(true);
    addNew.querySelector(`h1`).innerHTML = `New Project`;
    addNew.querySelector(`a`).href = `#project-name-input`;
    container.appendChild(addNew);

    document.getElementById(`new-project-form`).addEventListener(`submit`, onNewProject);
}

async function onNewProject(e) {

    return false;
}