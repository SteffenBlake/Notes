import * as bootstrap from 'bootstrap';
fixBootstrapModalFocusTraps();

import { initAsync as themeSwitcherInitAsync } from './theme-switcher.js';
import { initAsync as dataTriggersInitAsync } from './data-triggers.js';

(() => {
    addEventListener("DOMContentLoaded", initAsync);
})()

async function initAsync() {
    await Promise.all([
        themeSwitcherInitAsync(),
        dataTriggersInitAsync()
    ]);
}

function fixBootstrapModalFocusTraps() {
    bootstrap.Modal.prototype._initializeFocusTrap = function () { return { activate: function () { }, deactivate: function () { } } };
}