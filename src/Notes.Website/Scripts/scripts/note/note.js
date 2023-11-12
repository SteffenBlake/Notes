import { initAsync as editorInitAsync } from "../shared/ckEditor.js";

async function initAsync() {
    editorInitAsync(document.getElementById('note-contents'));
}

(async () => initAsync())();