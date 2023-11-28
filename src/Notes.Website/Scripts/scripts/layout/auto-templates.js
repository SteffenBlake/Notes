export default function() {
    var templates = document.querySelectorAll('template');
    for (const template of templates) {
        var id = template.id;
        if (!id) {
            continue;
        }
        window.customElements.define(id, 
            class extends HTMLElement {
                constructor() {
                    super();
                    this
                        .attachShadow({ mode: "open" })
                        .appendChild(template.content.cloneNode(true));
                }
            }
        )
    }
}