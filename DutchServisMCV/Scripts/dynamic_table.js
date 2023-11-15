var sortColumn = "null";
var sordOrder = "null";
var dataset = null;

function refresh() {
    dataset = applyFilters(model);

    switch (sortColumn) {
        case "Player": {
            dataset.sort(comparePlayer);
            break;
        }
        case "Ranking": {
            dataset.sort(compareRanking);
            break;
        }
        case "Rating": {
            dataset.sort(compareRating)
            break;
        }
    }

    if (sordOrder === "Desc") dataset.reverse();

    updateHtml();
}

// Sort and filter functions
function sortPlayer() {
    dataset = applyFilters(model);
    dataset.sort(comparePlayer);

    if (sortColumn === "Player") {
        if (sordOrder === "Asc") {
            sordOrder = "Desc";
            dataset.reverse();

            document.getElementById("player_sort").setAttribute("value", "Gracz \u2BAF");
        }
        else {
            sordOrder = "Asc";
            document.getElementById("player_sort").setAttribute("value", "Gracz \u2BAD");
        }
    }
    else {
        sortColumn = "Player";
        sordOrder = "Asc";
        document.getElementById("player_sort").setAttribute("value", "Gracz \u2BAD");

        document.getElementById("rank_sort1").setAttribute("value", "Ranking \u296E");
        document.getElementById("rank_sort2").setAttribute("value", "Rank. \u296E");
        document.getElementById("elo_sort1").setAttribute("value", "Rating \u296E");
        document.getElementById("elo_sort2").setAttribute("value", "Elo \u296E");
    }

    updateHtml();
}
function sortRanking() {
    dataset = applyFilters(model);
    dataset.sort(compareRanking);

    if (sortColumn === "Ranking") {
        if (sordOrder === "Asc") {
            sordOrder = "Desc";
            dataset.reverse();

            document.getElementById("rank_sort1").setAttribute("value", "Ranking \u2BAF");
            document.getElementById("rank_sort2").setAttribute("value", "Rank. \u2BAF");
        }
        else {
            sordOrder = "Asc";
            document.getElementById("rank_sort1").setAttribute("value", "Ranking \u2BAD");
            document.getElementById("rank_sort2").setAttribute("value", "Rank. \u2BAD");
        }
    }
    else {
        sortColumn = "Ranking";
        sordOrder = "Desc";
        dataset.reverse();
        document.getElementById("rank_sort1").setAttribute("value", "Ranking \u2BAF");
        document.getElementById("rank_sort2").setAttribute("value", "Rank. \u2BAF");

        document.getElementById("player_sort").setAttribute("value", "Gracz \u296E");
        document.getElementById("elo_sort1").setAttribute("value", "Rating \u296E");
        document.getElementById("elo_sort2").setAttribute("value", "Elo \u296E");
    }

    updateHtml();
}
function sortRating() {
    dataset = applyFilters(model);
    dataset.sort(compareRating);

    if (sortColumn === "Rating") {
        if (sordOrder === "Asc") {
            sordOrder = "Desc";
            dataset.reverse();

            document.getElementById("elo_sort1").setAttribute("value", "Rating \u2BAF");
            document.getElementById("elo_sort2").setAttribute("value", "Elo \u2BAF");
        }
        else {
            sordOrder = "Asc";
            document.getElementById("elo_sort1").setAttribute("value", "Rating \u2BAD");
            document.getElementById("elo_sort2").setAttribute("value", "Elo \u2BAD");
        }
    }
    else {
        sortColumn = "Rating";
        sordOrder = "Desc";
        dataset.reverse();
        document.getElementById("elo_sort1").setAttribute("value", "Rating \u2BAF");
        document.getElementById("elo_sort2").setAttribute("value", "Elo \u2BAF");

        document.getElementById("player_sort").setAttribute("value", "Gracz \u296E");
        document.getElementById("rank_sort1").setAttribute("value", "Ranking \u296E");
        document.getElementById("rank_sort2").setAttribute("value", "Rank. \u296E");
    }

    updateHtml();
}
function applyFilters(data) {
    var array = [];

    // active filter
    var onlyactive = document.getElementById("active_checkbox").checked;

    // text filter
    var filtertext = "";
    filtertext = document.getElementById("filter_filed").value.toLowerCase();

    for (var i = 0; i < data.length; i++) {
        if (!onlyactive && data[i].Active === false) continue;
        if (filtertext != "" && !data[i].Nickname.toLowerCase().includes(filtertext)) continue;

        array.push(data[i]);

        if (data[i].Img === null || !data[i].Img.includes(path.toString())) {
            array[array.length - 1].Img = getPath("playerdata/", data[i].Img, "avatar.png");
        }
    }

    return array;
}

// Comapre functions
function comparePlayer(a, b) {
    if (a.Nickname > b.Nickname) return 1;
    else if (a.Nickname === b.Nickname) return 0;
    else return -1;
}
function compareRanking(a, b) {
    if (a.Ranking > b.Ranking) return 1;
    else if (a.Ranking === b.Ranking) return 0;
    else return -1;
}
function compareRating(a, b) {
    if (a.Rating > b.Rating) return 1;
    else if (a.Rating === b.Rating) return 0;
    else return -1;
}

// Updateing Html
function addPlayerTr(nr, player) {
    var addedHtmlCode = "";

    addedHtmlCode += "<tr class=\"table-style\">";

    addedHtmlCode += "<td>" + nr + ".</td>";

    addedHtmlCode += "<td class=\"disp-md\">";
        addedHtmlCode += "<img class=\"img-responsive img-rounded\" src=\"" + player.Img + "\"/>";
    addedHtmlCode += "</td>";

    addedHtmlCode += "<td>";
    addedHtmlCode += "<a class=\"boldlink\" href=\"Players/Info/" + player.Nickname + "\">" + player.Nickname + "</a>";
    addedHtmlCode += "</td>";

    addedHtmlCode += "<td class=\"disp-lg\">" + player.Clan + "</td>";

    addedHtmlCode += "<td>" + player.Ranking.toFixed(1) + "</td>";

    addedHtmlCode += "<td>" + player.Rating.toFixed(1) + "</td>";

    addedHtmlCode += "</tr>";

    return addedHtmlCode;
}
function addErrorTr(nr) {
    var addedHtmlCode = "";

    addedHtmlCode += "<tr class=\"table-style\">";

    addedHtmlCode += "<td class=\"cell-1\">";
    addedHtmlCode += nr + ".";
    addedHtmlCode += "</td>";

    addedHtmlCode += "<td colspan=\"5\" class=\"text-danger\">";
    addedHtmlCode += "Nie powiodło się załadowanie gracza";
    addedHtmlCode += "</td>";

    addedHtmlCode += "</tr>";

    return addedHtmlCode;
}
function updateHtml() {
    var innerHtmlCode = "";
    for (var i = 0; i < dataset.length; i++) {
        try {
            innerHtmlCode += addPlayerTr(i+1, dataset[i]);
        }
        catch {
            innerHtmlCode += addErrorTr(i+1);
        }
    }
    document.getElementById("table_body").innerHTML = innerHtmlCode;
}

// Filter menu
function openFilterMenu() {
    var menu = document.getElementById("filter_menu");

    if (menu.style.display === "") {
        menu.style.display = "block";
    }
    else menu.style.display = "";
}