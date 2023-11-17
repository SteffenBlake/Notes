import { initAsync as editorInitAsync } from "../../shared/ckEditor.js";

export async function initAsync() 
{
    await editorInitAsync(document.getElementById(`new-project-description`));
}
