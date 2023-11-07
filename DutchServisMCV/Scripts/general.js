function getPath(catalog, img, placeholder) {
    if (img === null || img === "" || path === null) {
        return path + "/images/" + placeholder;
    }
    else {
        return path + "/images/" + catalog + img;
    }
}
function setPath(id, catalog, img, placeholder) {
    document.getElementById(id).setAttribute("src", getPath(catalog, img, placeholder));
}