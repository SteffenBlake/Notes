export async function indexProjects()
{
    var resp = await window.fetch(`/api/projects/`);
    return await resp.json();
}

export async function readProject(projectName)
{
    var resp = await window.fetch(`/api/projects/${projectName}/`);
    return await resp.json();
}

export async function putProject(projectName, data) {
    var resp = await window.fetch(`/api/projects/${projectName}/`, {
        method: "PUT",
        body: JSON.stringify(data)
    });
    if (!resp.ok) {
        return;
    }
    return await resp.json();
}

export async function deleteProject(projectName) {
    await window.fetch(`/api/projects/${projectName}/`, {
        method: "DELETE",
    });
}