/* Wykorzystywane zmienne */
/*
 * var path :string
 * var collection :item[]
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

const AttType = {
    Number: "Number",
    Text: "Text",
    Input: "Input",
    Readonly: "Readonly Input",
    Delete: "DeleteButton",
    Select: "Select"
}

const DefaultValue = {
    "Number": 0,
    "String": "",
    "Null": "",
    "Select": null
}

class ContentManager {
    #tableList
    #select
    #addBtn
    #attributes

    /**
     * Tworzy menadżer do obsługi elementów w widoku
     * @param {TableContentGenerator[]} tables Lista obiektów TableContentGenerator
     * @param {SelectController} select Obiekt SelectController
     * @param {AddPlayerController} addBtn Obiekt AddController
     * @param {any[]} attributes Lista atrybutów w obiekcie kolekcji
     */
    constructor(tables, select, addBtn, attributes) {
        this.#tableList = tables;
        for (var i = 0; i < this.#tableList.length; i++) {
            this.#tableList[i].Manager = this;
        }
        this.#select = select;

        this.#addBtn = addBtn;
        this.#addBtn.BindOnClick(this);

        this.#attributes = attributes;

        this.Update();
    }

    // Methods
    /**
     * Zwraca typ atrybutu
     * @param {string} name Nazwa atrybutu
     */
    GetType(name) {
        for (var i = 0; i < this.#attributes.length; i++) {
            if (this.#attributes[i].Name == name) return this.#attributes[i].Type;
        }
        return null;
    }
    /**
     * Tworzy element kolekcji z obiektu bazowego
     * @param {any} object Obiekt bazowy
     */
    CollectionItem(object) {
        var item = {};

        for (var i = 0; i < this.#attributes.length; i++) {
            if (object[this.#attributes[i].Name] != null) {
                item[this.#attributes[i].Name] = object[this.#attributes[i].Name];
            }
            else {
                item[this.#attributes[i].Name] = DefaultValue[this.#attributes[i].Type];
            }
        }

        return item;
    }
    /**
     * Odświeża wszytskie przypisane elementy na stronie
     */
    Update() {
        for (var i = 0; i < this.#tableList.length; i++) {
            this.#tableList[i].Update();
        }
        if (this.#select != null) this.#select.Update();
    }
    /**
     * Dodaje wybrany element do kolekcji
     */
    AddItem() {
        if (this.#select != null) {
            var selected = this.#select.GetSelected();

            var item = this.CollectionItem(selected);
            this.#select.Remove(selected[this.#select.Attribute.Id]);
        }
        else {
            var item = this.CollectionItem({});
        }

        if (collection == null) {
            collection = [];
        }
        collection.push(item);

        this.Update();
    }
    /**
     * Usuwa wybrany element z kolekcji
     * @param {number} index Indeks elementu
     */
    DeleteItem(index) {
        if (this.#select != null) {
            var selectItem = {};
            selectItem[this.#select.Attribute.Id] = collection[index][this.#select.Attribute.Id];
            selectItem[this.#select.Attribute.Text] = collection[index][this.#select.Attribute.Text];

            this.#select.Add(selectItem);
        }

        collection.splice(index, 1);

        this.Update();
    }
}

class SelectManager {
    #selectIds
    #textIds

    Children

    /**
     * Tworzy obiekt, który odświerza wybrane elementy zgodnie z zmianą wybranego pola w elementach źródłowych
     * @param {any} children Lista obiektów, które zależą od źródeł
     * @param {string[]} selectIds Lista id elementów Select będących źródłami
     * @param {string[]} textIds Lista id tekstowych elementów Html
     */
    constructor(children, selectIds, textIds) {
        this.Children = children;
        this.#selectIds = selectIds;
        this.#textIds = textIds;

        for (let i = 0; i < this.#selectIds.length; i++) {
            let selectElement = document.getElementById(this.#selectIds[i]);

            // On change bind
            selectElement.onchange = function () {
                var selected = selectElement.options[selectElement.options.selectedIndex];
                for (var j = 0; j < this.Children.length; j++) {
                    this.Children[j].OnSourceChange(i, selected);
                }

                document.getElementById(this.#textIds[i]).innerHTML = selected.label;
            }.bind(this);

            // Set
            document.getElementById(this.#textIds[i]).innerHTML = selectElement.options[selectElement.options.selectedIndex].label;
        }
    }
}

class TableContentGenerator {
    #tableId
    #collectionName
    #selectOptions
    #selectAttribute

    Attributes
    Manager

    /**
     * Tworzy obiekt generatora treści dla podanej tabeli
     * @param {string} name Nazwa kolekcji obiektów
     * @param {string} tableId Html id tabeli
     * @param {any[]} attributes Lista atrybutów w obiekcie kolekcji
     * @param {string[]} options Lista opcji dla pół wyboru
     * @param {any} selectAttribute Atrybut dla pól wyboru
     */
    constructor(name, tableId, attributes, options = [], selectAttribute = null) {
        this.#collectionName = name;
        this.#tableId = tableId;
        this.Attributes = attributes;
        this.#selectOptions = options;
        this.#selectAttribute = selectAttribute;
    }

    // Private Methods

    /**
     * Funkcja zwraca element Html <td> zawiarający <input> w postaci wartości string
     * @param {string} name Wartość dla atrybutów 'id' oraz 'name'
     * @param {string} value Wartość dla atrybutu 'value'
     * @param {string} type Typ wartości pola
     * @param {boolean} readonly Posiada atrybut tylko do odczytu
     */
    #Input(name, value = "", type, readonly = false) {
        var str = "<td><input " + " id=\"" + name + "\" name=\"" + name + "\" value=\"" + value + "\" ";
        str += "type=\"text\" class=\"form-control text-box\" autocomplete=\"off\"";
        str += "onchange=\"UpdateCollection('" + name + "', '" + type + "')\"";

        if (readonly == true) {
            str += "readonly=\"readonly\"";
        }

        return str + "></td>";
    }
    /**
     * Funkcja zwraca element Html <td> z wartością onclick="DeleteItem" w postaci wartości string
     * @param {number} index Indeks elementu w tabeli
     */
    #DeleteButton(index) {
        return "<td class=\"text-danger btn\" id=\"" + "deleteBtn_"+ index + "\">Usuń</td>";
    }
    /**
     * Funkcja zwraca element Html <td> zawiarający pole wyboru <select>
     * @param {string} name Wartość dla atrybutów 'id' oraz 'name'
     * @param {number} value Indeks wybranej opcji
     */
    #Select(name, value, type) {
        var str = "<td><select class=\"list-box form-control\" id=\"" + name + "\" name=\"" + name + "\"";
        str += "onchange=\"UpdateCollection('" + name + "', '" + type + "')\" >";

        var selected = "";
        for (var i = 0; i < this.#selectOptions.length; i++) {
            if (i + 1 == value) selected = "selected=\"selected\"";
            else selected = "";
            if (this.#selectOptions[i] != null) {
                var text = this.#selectOptions[i][this.#selectAttribute.Text];
            }
            else var text = "Brak";
            str += "<option value=\"" + (i + 1).toString() + "\"" + selected + ">" + text + "</option>";
        }

        return str += "</select></td>";
    }
    /**
     * Funkcja zarwaca element <td> dla pola o określonym typie
     * @param {number} index Index elementu
     * @param {string} type Typ pola
     * @param {any} value Wartość lub lista wartości atrybutu
     */
    #Attribute(index, attName, type) {
        var value = collection[index][attName];
        var name = this.#collectionName + "[" + index + "]." + attName;

        switch (type) {
            case AttType.Number: return "<td>" + (index + 1).toString() + ".</td>";
            case AttType.Text: return "<td>" + value + "</td>";
            case AttType.Input: return this.#Input(name, value, this.Manager.GetType(attName));
            case AttType.Readonly: return this.#Input(name, value, null, true);
            case AttType.Select: return this.#Select(name, value, this.Manager.GetType(attName));
            case AttType.Delete: return this.#DeleteButton(index);
        }
    }

    /**
     * Ustawia funkcję onclick dla przycisków usunięcia elementu
     * @param {number[]} btnIndexList Lista indeksów przycisków
     */
    #BindOnClick(btnIndexList) {
        for (let i = 0; i < btnIndexList.length; i++) {
            let idx = btnIndexList[i];
            document.getElementById("deleteBtn_" + idx.toString()).onclick = () => this.Manager.DeleteItem(idx);
        }
    }

    // Public Methods
    /**
     * Zmienia wartość opcji przypisanej do zmienionego źródła
     * @param {number} index Indeks zmienionego źródła
     * @param {any} selected Wybrany element
     */
    OnSourceChange(index, selected) {
        var item = {};
        item[this.#selectAttribute.Id] = selected.value;
        item[this.#selectAttribute.Text] = selected.label;
        this.#selectOptions[index] = item;

        this.Update();
    }
    /**
     * Odświerza zawartość tabeli
     */
    Update() {
        if (collection == null || collection.length == 0) {
            document.getElementById(this.#tableId).innerHTML = "<tr><td colspan=\"" + this.Attributes.length + "\">Pusta lista</td></tr>";
            return;
        }

        var html = "";
        var deletedIdx = [];

        for (var i = 0; i < collection.length; i++) {
            // For each element
            html += "<tr>";
            this.Attributes.forEach(function (attribute) {
                // For each attribute
                html += this.#Attribute(i, attribute.Name, attribute.Type)
                if (attribute.Type == AttType.Delete) deletedIdx.push(i);
            }.bind(this));
            html += "</tr>";
        }

        document.getElementById(this.#tableId).innerHTML = html;

        this.#BindOnClick(deletedIdx);
    }
}

class SelectController {
    #selectId
    #options
    #nullValue

    Attribute
    Manager

    /**
     * Tworzy obiekt obsługujący Html element select
     * @param {string} selectId Html id elementy select
     * @param {any} options Lista opcji
     * @param {any} pair Lista atrybutów w obiekcie options
     * @param {string} nullValue Domyśla wartość pusta
     */
    constructor(selectId, options, attribute, nullValue, selected) {
        this.#selectId = selectId;
        this.#options = options;
        this.Attribute = attribute;
        this.#nullValue = nullValue;

        this.Update(selected);
    }

    /**
     * Zmienia wartość opcji przypisanej do zmienionego źródła 
     * @param {number} index Indeks zmienionego źródła
     * @param {any} selected Wybrany element
     */
    OnSourceChange(index, selected) {
        var item = {};
        item[this.Attribute.Id] = selected.value;
        item[this.Attribute.Text] = selected.label;
        this.#options[index] = item;

        this.Update();
    }
    /**
     * Odświerza listę opcji
     * @param {number} selected Wybrana opcja
     */
    Update(selected = null) {
        var content = "<option value=\"null\">" + this.#nullValue + "</option>";
        for (var i = 0; i < this.#options.length; i++) {
            var option = this.#options[i];
            if (option != null) {
                content += "<option value=\"" + option[this.Attribute.Id] + "\"";
                if (selected == option[this.Attribute.Id]) {
                    content += " selected";
                }
                content += ">" + option[this.Attribute.Text] + "</option>";
            }
        }

        document.getElementById(this.#selectId).innerHTML = content;
    }
    /**
     * Dodaje nowy element do listy opcji
     * @param {any} item Nowy element
     */
    Add(item) {
        this.#options.push(item);
    }
    /**
     * Z listy opcji usuwa obiekt
     * @param {number} Id Id usuwanego elementu
     */
    Remove(Id) {
        var key = this.#options.findIndex(item => item[this.Attribute.Id] == Id);
        this.#options.splice(key, 1);
    }
    /**
     * Zwraca wybrany element
     */
    GetSelected() {
        var select = document.getElementById(this.#selectId).options;

        return this.#options[select.selectedIndex-1];
    }
}

class AddPlayerController {
    #addBtnId

    /**
     * Tworzy kontroller przycisku dodania
     * @param {string} buttonId
     */
    constructor(buttonId) {
        this.#addBtnId = buttonId;
    }

    /**
     * Ustawia atrybut onclick przyciskowi
     * @param {any} manager
     */
    BindOnClick(manager) {
        document.getElementById(this.#addBtnId).onclick = manager.AddItem.bind(manager);
    }
}

/**
 * Uaktualnia element w kolekcji
 * @param {string} id
 * @param {string} type
 */
function UpdateCollection(id, type) {
    var value = document.getElementById(id).value;

    switch (type) {
        case "Number": {
            var onlyNumbers = /^\d+$/.test(value);
            if (!onlyNumbers) {
                document.getElementById(id).value = DefaultValue["Number"];
                value = DefaultValue["Number"];
            }
            break;
        }
        case "Null": {
            var onlyNumbers = /^\d+$/.test(value);
            if (value != "" && !onlyNumbers) {
                document.getElementById(id).value = DefaultValue["Null"];
                value = DefaultValue["Null"];
            }
            break;
        }
        default: break;
    }

    var parts = id.split(/[\[.\]]/);
    collection[parts[1]][parts[3]] = value;
}

