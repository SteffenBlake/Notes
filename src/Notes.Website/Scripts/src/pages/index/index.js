import { NotesEditor } from "../../ck-editor/notes-editor.js";

import "../../styles/main.scss";

async function init() {
    var editor = await NotesEditor.create(document.querySelector('#app'));

    document.getElementById("skip-btn").addEventListener("click", () => editor.editing.view.focus());
}

(async () => init())();