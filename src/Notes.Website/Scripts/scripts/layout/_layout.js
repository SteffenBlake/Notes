import * as bootstrap from 'bootstrap';
fixBootstrapModalFocusTraps();

import { initAsync as themeSwitcherInitAsync } from './theme-switcher.js';
import { initAsync as dataTriggersInitAsync } from './data-triggers.js';
import initAutoTemplates from './auto-templates.js';
import initAutoInputPatterns from './auto-input-patterns.js';
import initHeaderAsync from './header/_header.js';

(() => {
    addEventListener("DOMContentLoaded", initAsync);
})()

async function initAsync() {
    await Promise.all([
        themeSwitcherInitAsync(),
        dataTriggersInitAsync(),
        initHeaderAsync()
    ]);

    initAutoTemplates();
    initAutoInputPatterns();
}

function fixBootstrapModalFocusTraps() {
    bootstrap.Modal.prototype._initializeFocusTrap = function () { return { activate: function () { }, deactivate: function () { } } };
}