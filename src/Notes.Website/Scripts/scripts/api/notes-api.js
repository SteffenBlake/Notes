export async function indexNotes(projectName)
{
    var resp = await window.fetch(`/api/projects/${projectName}/notes`);
    return await resp.json();
}

export async function readNote(projectName, notePath) {
    var resp = await window.fetch(`/api/projects/${projectName}/notes/${notePath}`);
    return await resp.json();
}

export async function putNote(projectName, notePath, data) {
    var resp = await window.fetch(`/api/projects/${projectName}/notes/${notePath}`, {
        method: "PUT",
        body: JSON.stringify(data)
    });
    if (!resp.ok) {
        return;
    }
    return await resp.json();
}

export async function deleteNote(projectName, notePath) {
    await window.fetch(`/api/projects/${projectName}/notes/${notePath}`, {
        method: "DELETE",
    });
}