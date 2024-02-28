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