import {getAsync, putAsync, delAsync } from './api-helper.js';

export async function indexProjects()
{
    return await getAsync(`/api/projects/`);
}

export async function readProject(projectName)
{
    return await getAsync(`/api/projects/${projectName}/`);
}

export async function putProject(projectName, data) {
    return await putAsync(`/api/projects/${projectName}/`, data);
}

export async function deleteProject(projectName) {
    return await delAsync(`/api/projects/${projectName}/`);
}