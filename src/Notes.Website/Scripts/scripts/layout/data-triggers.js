export async function initAsync() 
{
    await Promise.all([
        bindReplaceTriggersAsync()
    ]);
}

async function bindReplaceTriggersAsync() {
    var triggerElems = document.querySelectorAll(`[data-trigger="replace"]`);
    var promises = [...triggerElems].map(bindReplaceTriggerAsync);
    await Promise.all(promises);
}

async function bindReplaceTriggerAsync(triggerElem) {
    var selectorSelector = triggerElem.dataset.selector;
    var placeholderSelector = triggerElem.dataset.placeholder;
    var targetSelector = triggerElem.dataset.target;

    var selectors = triggerElem.querySelectorAll(selectorSelector);
    var placeholders = triggerElem.querySelectorAll(placeholderSelector);
    var targets = triggerElem.querySelectorAll(targetSelector);

    for (const selector of selectors) {
        const selectorVal = selector.dataset.value;
        const selectorContents = selector.innerHTML;
        selector.addEventListener('click', () => {

            // Clear the "selected" styling of the selected button
            var selectedElems = triggerElem.querySelectorAll(`.text-bg-secondary`);
            for (const selectedElem of selectedElems) {
                selectedElem.classList.remove(`text-bg-secondary`);
            }
            // Then apply it to the newly selected one
            selector.classList.add(`text-bg-secondary`);

            // Swap out the "parent" button's contents with the selected contents
            for (const placeholder of placeholders) {
                placeholder.innerHTML = selectorContents;
            }

            // Update the value of the (typically hidden) target
            for (var target of targets) {
                target.value = selectorVal;
            }
        });
    }
}