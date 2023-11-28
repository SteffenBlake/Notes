import { indexProjects } from '../api/projects-api.js';
import { enableDataTags } from '../utilities.js';

document.addEventListener("DOMContentLoaded", onLoaded);

async function onLoaded() {
    enableDataTags();

    var indexPromise = indexProjects();

    const container = document.getElementById(`projects-container`);
    const projectBoxTemplate = document.getElementById(`project-box`);

    var projects = await indexPromise;
    projects.data.data.forEach(project => {
        var projectBox = projectBoxTemplate.content.cloneNode(true);
        projectBox.querySelector(`h1`).innerHTML = project.name;
        projectBox.querySelector(`a`).href = `/${project.name}`;
        container.appendChild(projectBox);
    });

    var addNew = projectBoxTemplate.content.cloneNode(true);

    addNew.querySelector(`h1`).innerHTML = `New Project`;

    var addNewAnchor = addNew.querySelector(`a`);
    addNewAnchor.href = `#project-name-input`;
    addNewAnchor.dataset.target = `new-project-modal`;
    addNewAnchor.dataset.toggle = `show`;

    container.appendChild(addNew);
}