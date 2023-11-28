export default function() {
    var patternInputs= document.querySelectorAll('input[pattern]');
    for (const input of patternInputs) {
        input.addEventListener('keypress', onKeypress);
        input.addEventListener('paste', onPaste);
        input.addEventListener('input', onInput);
    }
}

function onKeypress(e) {
    handleTextEvent(e, e.key);
}

function onPaste(e) {
    if (e.clipboardData.items[0].kind != "string") {
        e.preventDefault();
        return;
    }
    
    handleTextEvent(e, e.clipboardData.getData('text/plain'));
}

function handleTextEvent(e, text) {
    const re = new RegExp(e.target.pattern);
    var before = e.target.value.substring(0, e.target.selectionStart);
    var after = e.target.value.substring(e.target.selectionEnd);
    var result = before + text + after;
    if (!result.match(re))
    {
        // Only actually block if this specifically isnt clearing the field
        if (result) {
            e.preventDefault();
        }
        invalid(e.target);
        window.setTimeout(() => valid(e.target), 500);
    } else {
        valid(e.target);
    }
}

function onInput(e){
    const re = new RegExp(e.target.pattern);
    if (!e.target.value.match(re))
    {
        invalid(e.target);
    }
}

function invalid(input) {
    input.classList.remove('is-valid');
    input.classList.add('is-invalid');
}

function valid(input) {
    input.classList.remove('is-invalid');
    input.classList.add('is-valid');
}