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

function GetInput(name, value, readonly) {
    var str = "<input type=\"text\" class=\"form-control text-box\" id=\"" + name + "\" name=\"" + name + "\" value=\"" + value + "\" autocomplete=\"off\"";

    if (readonly == true) {
        str += "readonly=\"readonly\"";
    }

    return str + ">";
}
function Table1(index, id1, id2) {
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

    html += "<td class=\"text-danger btn\" onclick=\"DeleteItem('" + index + "', '" + id1 + "', '" + id2 + "')\">";
    html += "Usuń"
    html += "</td>";

    html += "</tr>";
    return html;
}
function Table2(index) {
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
function UpdateTables(id1, id2) {
    var table1 = "";
    var table2 = "";
    if (list != null && list.length != 0) {
        for (var i = 0; i < list.length; i++) {
            table1 += Table1(i, id1, id2);
            table2 += Table2(i);
        }
    }
    else {
        table1 += "<tr><td>Pusta lista</td></tr>";
        table2 += "<tr><td>Pusta lista</td></tr>";
    }
    // set
    document.getElementById(id1).innerHTML = table1;
    document.getElementById(id2).innerHTML = table2;
}
function AddItem(select_id, table_id_1, table_id_2) {
    var options = document.getElementById(select_id).options;
    var selected = options[options.selectedIndex];

    if (selected.value != "null") {
        var item = {
            Id: selected.value,
            Nickname: selected.label,
            Place: "",
            RankingGet: 0,
            Price: 0.0
        }
        if (list == null) {
            list = [];
        }
        list.push(item)

        UpdateTables(table_id_1, table_id_2);
    }
}
function DeleteItem(key, table_id_1, table_id_2) {
    list.splice(key, 1);

    UpdateTables(table_id_1, table_id_2);
}
