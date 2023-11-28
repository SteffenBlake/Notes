import { initAsync as CreateNewModalInitAsync } from './create-new-modal.js';
import { initAsync as initNavbarAsync } from './navbar.js';
import { initAsync as navigationSidebarInitAsync } from './navigation-sidebar.js';

export default async function() {
    if (!document.getElementById(`navbar`)){
        return;
    }

    await Promise.all([
        CreateNewModalInitAsync(),
        initNavbarAsync(),
        navigationSidebarInitAsync(),
    ]);
}