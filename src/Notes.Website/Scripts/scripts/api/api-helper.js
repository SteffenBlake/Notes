export function getAsync(url, data) {
    return jsonFetchAsync(url, "GET", data);
} 

export function postAsync(url, data) {
    return jsonFetchAsync(url, "POST", data);
} 

export function putAsync(url, data) {
    return jsonFetchAsync(url, "PUT", data);
} 

export function delAsync(url, data) {
    return jsonFetchAsync(url, "DELETE", data);
} 

async function jsonFetchAsync(url, method, data) {
    var options = {
        method, 
        mode: "same-origin", 
        credentials: "same-origin",
        redirect: "follow"
    };

    if (data) {
        options.headers = {
            "Content-Type": "application/json",
        };
        options.body = JSON.stringify(data);
    }

    const response = await fetch(url, options);
    var responseObj = await response.json();

    if (!responseObj.errors) {
        throw new Error(responseObj);
    }

    return responseObj;
}
