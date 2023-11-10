export async function getRecentAsync(from = 0, to = 5)
{
    var resp = await window.fetch(`/api/directories/recent?from=${from}&to=${to}`);
    return await resp.json();
}

export async function getOverviewAsync(directoryId = "0") 
{
    var resp = await window.fetch(`/api/directories/${directoryId}/overview`);
    return await resp.json();
}

export async function getDescendantsAsync(directoryId) 
{
    var resp = await window.fetch(`/api/directories/${directoryId}/descendants`);
    return await resp.json();
}