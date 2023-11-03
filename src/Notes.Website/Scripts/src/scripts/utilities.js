export function enableDataTags() {
	document.querySelector(`body`).addEventListener(`click`, onClick);
}

function onClick(e) {
	if (e.target.dataset.target) {
		var dataTarget = document.getElementById(e.target.dataset.target);
		if (!dataTarget) {
			return;
		}
		if (e.target.dataset.toggle === `toggle`) {
			toggle(dataTarget);
			return;
		}
		if (e.target.dataset.toggle === `show`) {
			show(dataTarget);
			return;
		}
		if (e.target.dataset.toggle === `hide`) {
			hide(dataTarget);
			return;
		}
	}

	for (let modal of document.querySelectorAll('.modal')) {
		if (modal.contains(e.target)) {
			return;
		}
	}

	hideModals();
}

function toggle(element) {
	if (element.classList.contains(`show`)) {
		hide(element);
	} else {
		show(element);
	}
}

function show(element) {
	element.classList.add(`show`);
}

function hideModals() {
	for (let modal of document.querySelectorAll(`.modal`)) {
		hide(modal);
	}
}

function hide(element) {
	element.classList.remove(`show`);
}


// https://gomakethings.com/how-to-serialize-form-data-with-vanilla-js/
export function serializeForm(formElement) {
	var formData = new FormData(formElement);
	let obj = {};

	for (let [key, value] of formData) {
		if (obj[key] !== undefined) {
			if (!Array.isArray(obj[key])) {
				obj[key] = [obj[key]];
			}
			obj[key].push(value);
		} else {
			obj[key] = value;
		}
	}

	return obj;
}