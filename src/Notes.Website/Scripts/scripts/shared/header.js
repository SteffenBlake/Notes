import { initAsync as navigationSidebarInitAsync } from './navigation-sidebar.js';

(() => {
    addEventListener("DOMContentLoaded", initAsync);
})()

async function initAsync() {

    await Promise.all([
        navigationSidebarInitAsync()
    ]);
}