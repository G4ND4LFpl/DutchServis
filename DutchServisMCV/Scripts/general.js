function getPath(img) {
    if (img === null || img === "" || path === null) {
        return path + "/images/avatar.png";
    }
    else {
        return path + "/images/playerdata/" + img;
    }
}
function setPath(id, img) {
    document.getElementById(id).setAttribute("src", getPath(img));
}