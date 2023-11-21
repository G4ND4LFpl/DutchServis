/* Wykorzystywane zmienne */
/*
 * var path :string
 * var list :item[]
 * 
 * item = {
 * Id :string
 * Nickname :string
 * Place :string
 * RankingGet :number
 * Price :number
 * }
 * 
 * item = {
 * Id :string
 * Nickname :string
 * Points :string
 * Won :number
 * Loose :number
 * Draw :number
 * Price :number
 * }
 * 
 * var players :player[]
 * 
 * player = {
 * Id :string
 * Nickname :string
 * }
 */

/* Funkcje Ścieżki */

/**
 * Funkcja zwraca prawidłową ścieżkę do pliku
 * Dla pustego parametru 'img' zwraca ścieżkę do obrazka placeholder
 * @param {string} catalog Katalog
 * @param {string} img Nazwa pliku z rozszerzeniem
 * @param {string} placeholder Domyślny obrazek
 */
function GetPath(catalog, img, placeholder) {
    if (img === null || img === "" || path === null) {
        return path + "/images/" + placeholder;
    }
    else {
        return path + "/images/" + catalog + img;
    }
}
/**
 * Ustawia obraz z pliku 'img' odpowiedniemu elementowi Html
 * @param {string} id Id elementu
 * @param {string} catalog Katalog
 * @param {string} img Nazwa pliku z rozszerzeniem
 * @param {string} placeholder Domyślny obrazek
 */
function SetPath(id, catalog, img, placeholder) {
    document.getElementById(id).setAttribute("src", GetPath(catalog, img, placeholder));
}

/* Funkcje Tabeli */

/**
 * Funkcja zwraca element Html <input type="text"> w postaci wartości string
 * @param {string} name Wartość dla atrybutów 'id' oraz 'name' 
 * @param {string} value Wartość dla atrybutu 'value'
 * @param {boolean} readonly Posiada atrybut tylko do odczytu
 */
function GetInput(name, value, readonly) {
    if (value == null) value = "";
    var str = "<input type=\"text\" class=\"form-control text-box\" id=\"" + name + "\" name=\"" + name + "\" value=\"" + value + "\" autocomplete=\"off\"";

    if (readonly == true) {
        str += "readonly=\"readonly\"";
    }

    return str + ">";
}
/**
 * Funckja zwraca pojedyńczy wiersz tabeli PlayerSet dla danych z zmiennej list[index]
 * @param {number} index Indeks danych w tabeli 'list'
 * @param {string[]} ids Id powiązanych elementów Html
 * @param {string} type Typ, o przyjowanych wartościach: 'tournament', 'league'
 */
function TablePlayerSet(index, ids, type) {
    var html = "";
    html += "<tr>";

    html += "<td>" + (index + 1).toString() + ".</td>";

    var name = "Players[" + index + "].Id"

    html += "<td>";
    html += GetInput(name, list[index].Id, true);
    html += "</td>";

    name = "Players[" + index + "].Nickname"

    html += "<td>";
    html += GetInput(name, list[index].Nickname, true);
    html += "</td>";

    html += "<td class=\"text-danger btn\" onclick=\"DeleteItem('" + index + "', '" + ids[0] + "', '" + ids[1] + "', '" + ids[2] + "', '" + type + "')\">";
    html += "Usuń"
    html += "</td>";

    html += "</tr>";
    return html;
}
/**
 * Funckja zwraca pojedyńczy wiersz tabeli Results dla widoku Tournaments dla danych z zmiennej list[index]
 * @param {number} index Indeks danych w zmiennej 'list'
 */
function TableTournamentResults(index) {
    var html = "";
    html += "<tr>";

    html += "<td>" + list[index].Nickname + "</td>";

    var name = "Players[" + index + "].Place"

    html += "<td>";
    html += GetInput(name, list[index].Place, false);
    html += "</td>";

    name = "Players[" + index + "].RankingGet";

    html += "<td>";
    html += GetInput(name, list[index].RankingGet, false);
    html += "</td>";

    name = "Players[" + index + "].Price";

    html += "<td>";
    html += GetInput(name, list[index].Price, false);
    html += "</td>";

    html += "</tr>";
    return html;
}
/**
 * Funckja zwraca pojedyńczy wiersz tabeli Results dla widoku Leauge dla danych z zmiennej list[index]
 * @param {number} index Indeks danych w zmiennej 'list'
 */
function TableLeaugeResults(index) {
    var html = "";
    html += "<tr>";

    html += "<td>" + list[index].Nickname + "</td>";

    name = "Players[" + index + "].Points";

    html += "<td>";
    html += GetInput(name, list[index].Points, true);
    html += "</td>";

    name = "Players[" + index + "].Price";

    html += "<td>";
    html += GetInput(name, list[index].Price, false);
    html += "</td>";

    html += "</tr>";
    return html;
}
/**
 * Funkcja uzupełnia tabele PlayerSet i Results dla widoku Tournament danymi z zmiennej 'list'
 * @param {string[]} ids Html id tablic PlayerSet i Results
 */
function UpdateTournamentTables(ids) {
    var table1 = "";
    var table2 = "";
    if (list != null && list.length != 0) {
        for (var i = 0; i < list.length; i++) {
            table1 += TablePlayerSet(i, ids, 'tournament');
            table2 += TableTournamentResults(i);
        }
    }
    else {
        table1 += "<tr><td>Pusta lista</td></tr>";
        table2 += "<tr><td>Pusta lista</td></tr>";
    }
    // set
    document.getElementById(ids[0]).innerHTML = table1;
    document.getElementById(ids[1]).innerHTML = table2;
}
/**
 * Funkcja uzupełnia tabele PlayerSet i Results dla widoku League danymi z zmiennej 'list'
 * @param {string[]} ids Html id tablic PlayerSet i Results
 */
function UpdateLeagueTables(ids) {
    var table1 = "";
    var table2 = "";
    if (list != null && list.length != 0) {
        for (var i = 0; i < list.length; i++) {
            table1 += TablePlayerSet(i, ids, 'league');
            table2 += TableLeaugeResults(i);
        }
    }
    else {
        table1 += "<tr><td>Pusta lista</td></tr>";
        table2 += "<tr><td>Pusta lista</td></tr>";
    }
    // set
    document.getElementById(ids[0]).innerHTML = table1;
    document.getElementById(ids[1]).innerHTML = table2;
}
/**
 * Ustawia opcja elemenowi select
 * @param {string} id Html id
 */
function UpdateSelect(id) {
    var html = "<option value=\"null\" selected=\"selected\">Brak</option>";
    for (var i = 0; i < players.length; i++) {
        html += "<option value=\"" + players[i].Id + "\">" + players[i].Nickname + "</option>";
    }

    document.getElementById(id).innerHTML = html;
}
/**
 * Zwraca obiekt item o żądanym typie
 * @param {string} type Typ, o przyjowanych wartościach: 'tournament', 'league'
 * @param {number} id Id obiektu
 * @param {string} nick Pole Nickname obiektu
 */
function GetItem(type, id, nick) {
    if (type == 'tournament') {
        return {
            Id: id,
            Nickname: nick,
            Place: "",
            RankingGet: 0,
            Price: 0.0
        }
    }
    else {
        return {
            Id: id,
            Nickname: nick,
            Points: 0,
            Price: 0.0
        }
    }
}
/**
 * Dodaje wybrany element do zmiennej 'list' i aktualizuje tabele
 * @param {string} select_id Id elementu Html <select>
 * @param {string[]} table_id_1 Html id tabel do aktualizacji
 * @param {string} type Typ, o przyjowanych wartościach: 'tournament', 'league'
 */
function AddItem(select_id, ids, type) {
    var options = document.getElementById(select_id).options;
    var selected = options[options.selectedIndex];

    if (selected.value == "null") {
        return;
    }

    // Add item
    var item = GetItem(type, selected.value, selected.label)
    if (list == null) {
        list = [];
    }
    list.push(item)

    // Remove player
    var key = players.findIndex(p => p.Id == selected.value)
    players.splice(key, 1);

    // Update
    if (type == 'tournament') {
        UpdateTournamentTables(ids);
    }
    else {
        UpdateLeagueTables(ids);
    }
    UpdateSelect(select_id);
}
/**
 * Usuwa element z zmiennej 'list'
 * @param {number} key Index elementu
 * @param {string} id_1 Html id pierwszej tabeli
 * @param {string} id_2 Html id drugiej tabeli
 * @param {string} select_id Html id elementu select
 * @param {string} type Typ, o przyjowanych wartościach: 'tournament', 'league'
 */
function DeleteItem(key, id_1, id_2, select_id, type) {
    //Add player
    var p = GetItem(type, list[key].Id, list[key].Nickname);
    players.push(p);

    // Remove item
    list.splice(key, 1);

    // Update
    UpdateSelect(select_id);
    if (type == 'tournament') {
        UpdateTournamentTables([id_1, id_2, select_id]);
    }
    else {
        UpdateLeagueTables([id_1, id_2, select_id]);
    }
}
