// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

function set_some(set, predicate) {
    for (const item of set)
        if (predicate(item))
            return true;

    return false;
}

function set_every(set, predicate) {
    for (const item of set)
        if (!predicate(item))
            return false;

    return true;
}

function stringToColor(str) {
    // Simple hash function
    let hash = 0;
    for (let i = 0; i < str.length; i++) {
        hash = str.charCodeAt(i) + ((hash << 5) - hash);
    }

    // Convert hash to a hexadecimal color
    let color = '#';
    for (let j = 0; j < 3; j++) {
        let value = (hash >> (j * 8)) & 0xFF;
        color += ('00' + value.toString(16)).substr(-2); // Ensure two digits
    }

    return color;
}