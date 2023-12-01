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

const AttType = {
    Number: "Number",
    Text: "Text",
    Input: "Input",
    Readonly: "Readonly Input",
    Delete: "DeleteButton"
}

class ContentManager {
    #tableList
    #select
    #addBtn

    /**
     * Tworzy menadżer do obsługi elementów w widoku
     * @param {TableContentGenerator[]} tables Lista obiektów TableContentGenerator
     * @param {SelectController} select Obiekt SelectController
     * @param {AddPlayerController} addBtn Obiekt AddController
     */
    constructor(tables, select, addBtn) {
        this.#tableList = tables;
        for (var i = 0; i < this.#tableList.length; i++) {
            this.#tableList[i].Manager = this;
        }

        this.#select = select;

        this.#addBtn = addBtn;
        this.#addBtn.BindOnClick(this);
    }

    // Methods

    Update() {
        for (var i = 0; i < this.#tableList.length; i++) {
            this.#tableList[i].Update();
        }
        this.#select.Update();
    }

    AddItem() {
        var selected = this.#select.GetSelected();

        var item = GetItem("tournament", selected[this.#select.Attribute.Id], selected[this.#select.Attribute.Text]);
        if (collection == null) {
            collection = [];
        }
        collection.push(item);

        this.#select.Remove(selected[this.#select.Attribute.Id]);

        this.Update();
    }

    DeleteItem(index) {
        var selectItem = { Id: collection[index].Id, Nickname: collection[index].Nickname };
        this.#select.Add(selectItem);

        collection.splice(index, 1);

        this.Update();
    }
}

class TableContentGenerator {
    #tableId
    #collectionName

    Attributes
    Manager

    /**
     * Tworzy obiekt generatora treści dla podanej tabeli
     * @param {string} name Nazwa kolekcji obiektów
     * @param {string} tableId Html id tabeli
     * @param {any[]} attributes Lista atrybutów w obiekcie kolekcji
     */
    constructor(name, tableId, attributes) {
        this.#collectionName = name;
        this.#tableId = tableId;
        this.Attributes = attributes;

        this.Update();
    }

    // Private Methods

    /**
     * Funkcja zwraca element Html <td> zawiarający <input> w postaci wartości string
     * @param {string} name Wartość dla atrybutów 'id' oraz 'name'
     * @param {string} value Wartość dla atrybutu 'value'
     * @param {boolean} readonly Posiada atrybut tylko do odczytu
     */
    #Input(name, value="", readonly=false) {
        var str = "<td><input type=\"text\" class=\"form-control text-box\" id=\"" + name + "\" name=\"" + name + "\" value=\"" + value + "\" autocomplete=\"off\"";

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
     * Funkcja zarwaca element <td> dla pola o określonym typie
     * @param {number} index Index elementu
     * @param {string} type Typ pola
     * @param {any} value Wartość lub lista wartości atrybutu
     */
    #Attribute(index, attName, type) {
        var value = collection[index][attName];

        switch (type) {
            case AttType.Number: return "<td>" + (index + 1).toString() + ".</td>";
            case AttType.Text: return "<td>" + value + "</td>";
            case AttType.Input: return this.#Input(this.#collectionName + "[" + index + "]." + attName, value);
            case AttType.Readonly: return this.#Input(this.#collectionName + "[" + index + "]." + attName, value, true);
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
     * Odświerza zawartość tabeli
     */
    Update() {
        if (collection == null) {
            document.getElementById(this.#tableId).innerHTML = "<tr><td>Pusta lista</td></tr>";
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
    constructor(selectId, options, attribute, nullValue) {
        this.#selectId = selectId;
        this.#options = options;
        this.Attribute = attribute;
        this.#nullValue = nullValue;
    }

    Update() {
        var content = "<option value=\"null\" selected=\"selected\">" + this.#nullValue + "</option>";
        for (var i = 0; i < this.#options.length; i++) {
            content += "<option value=\"" + this.#options[i][this.Attribute.Id] + "\">" + this.#options[i][this.Attribute.Text] + "</option>";
        }

        document.getElementById(this.#selectId).innerHTML = content;
    }
    /**
     * Dodaje obiekt do listy opcji
     * @param {any} item Obiekt
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

