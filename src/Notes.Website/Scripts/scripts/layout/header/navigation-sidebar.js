import {getRecentAsync, getOverviewAsync, getDescendantsAsync} from '../../api/directories-api.js'

export async function initAsync() 
{
    await Promise.all([
        loadRecentAsync(),
        loadOverviewAsync()
    ]);
    
}

async function loadRecentAsync() {
    const recentReadModelTask = getRecentAsync();
    var recents = [];
    const recentTemplate = document.getElementById(`navigation-sidebar-recent-template`);
    const recentReadModel = await recentReadModelTask;

    for (const recentModel of recentReadModel.data.recent) {
        recents.push(buildRecentElement(recentModel, recentTemplate));
    }

    const container = document.getElementById(`navigation-recent`);
    container.replaceChildren(...recents);
}

function buildRecentElement(recentModel, recentTemplate) {
    var recentElem = recentTemplate.content.cloneNode(true);
    recentElem.querySelector(`span`).innerHTML = recentModel.name;
    recentElem.querySelector(`a`).href = recentModel.route;
    recentElem.querySelector(`use`)
        .setAttributeNS('http://www.w3.org/1999/xlink', 'xlink:href', `#${recentModel.icon}`);
    return recentElem;
}

async function loadOverviewAsync() {
    const overviewTask = getOverviewAsync();
    var directory = [];
    const dirTemplate = document.getElementById(`navigation-sidebar-directory-template`);
    const overviewModel = await overviewTask;

}


function onClick(e) {
    var parentLi = e.target.closest('li');
    if (parentLi.classList.contains('expand')) {
        parentLi.classList.remove('expand');
    } else {
        parentLi.classList.add('expand');
    }
}