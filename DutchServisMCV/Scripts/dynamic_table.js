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

    updateData();
}
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

    updateData();
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

    updateData();
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

    updateData();
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
function updateData() {
    var innerHtmlCode = "";
    for (var i = 0; i < dataset.length; i++) {
        innerHtmlCode += "<tr class=\"table-style\">";

        innerHtmlCode += "<td class=\"cell-1\">";
        innerHtmlCode += (i + 1) + ".";
        innerHtmlCode += "</td>";
 
        innerHtmlCode += "<td class=\"cell-1 disp-md\">";
        
        innerHtmlCode += "<img class=\"img-responsive img-rounded\" src=\"" + dataset[i].Img + "\"/>";
        innerHtmlCode += "</td>";

        innerHtmlCode += "<td class=\"cell-2\">";
        innerHtmlCode += "<a class=\"boldlink\" href=\"Players/Info?nickname=" + dataset[i].Nickname + "\">";
        innerHtmlCode += dataset[i].Nickname;
        innerHtmlCode += "</a>";
        innerHtmlCode += "</td>";

        innerHtmlCode += "<td class=\"cell-2 disp-lg\">";
        innerHtmlCode += dataset[i].Clan;
        innerHtmlCode += "</td>";

        innerHtmlCode += "<td class=\"cell-1\">";
        innerHtmlCode += dataset[i].Ranking.toFixed(1);
        innerHtmlCode += "</td>";

        innerHtmlCode += "<td class=\"cell-1\">";
        innerHtmlCode += dataset[i].Rating.toFixed(1);
        innerHtmlCode += "</td>";

        innerHtmlCode += "</tr>";
    }
    document.getElementById("table_body").innerHTML = innerHtmlCode;
}
function openFilterMenu() {
    var menu = document.getElementById("filter_menu");

    if (menu.style.display === "") {
        menu.style.display = "block";
    }
    else menu.style.display = "";
}