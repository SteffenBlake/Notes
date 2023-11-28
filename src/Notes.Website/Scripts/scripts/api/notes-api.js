import { segmentPath } from "../utilities.js";
import {getAsync, putAsync, delAsync } from './api-helper.js';

function parsePath(path) {
    const segments = segmentPath(path);
    const [projectName, ...rest] = segments;
    const notePath = rest.join('/');
    return { projectName, notePath };
}

export async function indexNotes(projectName)
{
    return await getAsync(`/api/projects/${projectName}/notes`);
}

export async function readNote(path) {
    const { projectName, notePath } = parsePath(path);

    return await getAsync(`/api/projects/${projectName}/notes/${notePath}`);
}

export async function putNote(path, data) {
    const { projectName, notePath } = parsePath(path);

    return await putAsync(`/api/projects/${projectName}/notes/${notePath}`, data);
}

export async function deleteNote(path) {
    const { projectName, notePath } = parsePath(path);
    return await delAsync(`/api/projects/${projectName}/notes/${notePath}`);
}