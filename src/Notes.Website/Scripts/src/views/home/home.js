import "../../styles/main.scss";
import "./ck-editor-overrides.scss";

async function init() {
    var editor = await window.ClassicEditor.create(
        document.querySelector('#app'), 
        {
            toolbar: []
        }
    );

    document.getElementById("skip-btn").addEventListener("click", () => editor.editing.view.focus());
}

(async () => init())();