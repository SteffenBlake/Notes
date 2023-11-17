import { initAsync as navigationSidebarInitAsync } from './navigation-sidebar.js';
import { initAsync as CreateNewModalInitAsync } from './create-new-modal.js';

(() => {
    addEventListener("DOMContentLoaded", initAsync);
})()

async function initAsync() {

    await Promise.all([
        navigationSidebarInitAsync(),
        CreateNewModalInitAsync()
    ]);
}