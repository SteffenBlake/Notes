// https://gomakethings.com/how-to-serialize-form-data-with-vanilla-js/
export function stringifyForm(formId) {
    var form = document.getElementById(formId);
	var formData = new FormData(form);

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

	return JSON.stringify(obj);
}