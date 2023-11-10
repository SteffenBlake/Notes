import * as bootstrap from 'bootstrap';

import { init as themeSwitcherInit } from './theme-switcher.js';

(() => {
    addEventListener("DOMContentLoaded", init);
})()

function init() {
    themeSwitcherInit();
}