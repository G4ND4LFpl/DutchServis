class Card {
    Color
    Value
    Visible
}

// Variebles
var state;

// Helper functions

/**
 * Returns css class for color id
 * @param {int} color Card color id
 */
function ColorClass(color) {
    if (color == 0 || color == 1) return "c-red";
    else return "c-black";
}
/**
 * Returns color name for color id
 * @param {int} color Card color id
 */
function GetColor(color) {
    if (color == 0) return "heart";
    if (color == 1) return "diamonds";
    if (color == 2) return "spades";
    if (color == 3) return "clubs";
}
/**
 * Zwraca index w tablicy dla podanego id
 * @param {string} id
 */
function GetIndex(id) {
    return Number(id.split('_')[2]);
}
/**
 * Ustawia offset 2 kolumny w wierszu
 * Wiersz przyjmuje wartości: 'bot' lub 'player'
 * @param {string} row Wiersz
 * @param {boolean} toggle Offset kolumny
 */
function ToggleOffSet(row, toggle) {
    document.getElementById("col_" + row + "_card_0").classList.toggle("col-lg-offset-2", toggle);
}

// Functions
/**
 * Ustawia obiektowi klasę "active" oraz zdarzenia onclick
 * @param {any} button Obiekt Html
 * @param {boolean} active Aktywność
 */
function SetActive(button, active) {
    if (active) {
        button.classList.toggle("active", true);
        button.onclick = () => Action(button.id);
    }
    else {
        button.classList.toggle("active", false);
        button.onclick = null;
    }
}

/**
 * Ustawia wybranemu przyciskowi puste miejsce
 * @param {string} id Html id przycisku
 * @param {boolean} active Aktywność
 * @param {boolean} showEmpty Wyświetlanie pola, jeśli puste
 */
function SetEmpty(id, active, showEmpty) {
    if (!showEmpty) {
        document.getElementById("col_" + id).classList.toggle("disp-none", true);
    }

    var button = document.getElementById(id);
    SetActive(button, active);

    var img = "<img src=\"" + path + "images/cards/card_empty.png\")\" class=\"img-responsive\" />";
    var text = "<div class=\"text-empty\">Puste</div>";
    button.innerHTML = img + text;
}
/**
 * Ustawia wybranemu przyciskowi reverse karty
 * @param {string} id Html id przycisku
 * @param {boolean} active Aktywność
 * @param {boolean} showEmpty Wyświetlanie pola, jeśli puste
 */
function SetCardReverse(id, active, showEmpty) {
    if (!showEmpty) {
        document.getElementById("col_" + id).classList.toggle("disp-none", false);
    }

    var button = document.getElementById(id);
    SetActive(button, active);

    button.innerHTML = "<img src=\"" + path + "images/cards/card_reverse.png" + "\")\" class=\"img-responsive\" />";
}
/**
 * Ustawia wybranemu przyciskowi avers karty
 * @param {string} id Html id przycisku
 * @param {Card} card Obiekt karty
 * @param {boolean} active Aktywność
 * @param {boolean} showEmpty Wyświetlanie pola, jeśli puste
 */
function SetCard(id, card, active, showEmpty) {
    if (!showEmpty) {
        document.getElementById("col_" + id).classList.toggle("disp-none", false);
    }

    var button = document.getElementById(id);
    SetActive(button, active);

    var img = "<img src=\"" + path + "images/cards/card_" + GetColor(card.Color) + ".png\")\" class=\"img-responsive\" />";
    var text = "<div class=\"text-avers " + ColorClass(card.Color) + "\">" + card.Value + "</div>";
    button.innerHTML = img + text;
}

/**
 * Odświeża widok przycisku, jeśli ten spełnia warunek.
 * Warunkiem może być:
 * 'all' - odświeża każdy przycisk
 * 'empty' - odświerza tylko puste pola
 * 'reverse' - odświeża tylko zakryte karty
 * 'avers' - odświeża tylko odkryte karty
 * @param {string} id Html id przycisku
 * @param {string} cards Warunek, aby przycisk był odświeżony
 */
function RefreshCard(id, cards="all") {
    idx = GetIndex(id);

    if (idx == 4) {
        if (id.includes("bot")) {
            ToggleOffSet("bot", state[id].Card == null);
        }
        else ToggleOffSet("player", state[id].Card == null);
    }

    if (state[id].Card == null && (cards == "all" || cards == "empty")) {
        SetEmpty(id, state[id].Active, idx < 4);
    }
    else if (state[id].Card.Visible == false && (cards == "all" || cards == "reverse")) {
        SetCardReverse(id, state[id].Active, idx < 4);
    }
    else if (cards == "all" || cards == "avers") {
        SetCard(id, state[id].Card, state[id].Active, idx < 4);
    }
}
/**
 * Odświeża widok kart na planszy w określonym wierszu i spełniających warunek
 * Wierszem może być: 'both', 'bot' lub 'player'
 * Warunkiem może być: 'all', 'empty', 'reverse' lub 'avers'
 * @param {string} row Wiersz kart (Wartość domyślna 'both')
 * @param {string} cards Warunek, aby przycisk był odświeżony (Wartość domyślna 'all')
 */
function Refresh(row = "both", cards = "all") {
    for (var i = 0; i < 8; i++) {
        // Bot cards
        if (row != "player") {
            RefreshCard("bot_card_" + i, cards);
        }

        // Player cards
        if (row != "bot") {
            RefreshCard("player_card_" + i, cards);
        }
    }
}
/**
 * Ustawia wierzchnią kartę odkrytemu stosowi kart
 * @param {Card} card Obiekt karty
 * @param {boolean} active Aktywność
 */
function RefreshStack(card, active) {
    if (card != null) {
        SetCard("stack", card, active, true);
    }
    else {
        SetEmpty("stack", active, true);
    }
}

/**
 * Zmienia aktywność deku
 * @param {boolean} toggle Aktywność
 */
function ToggleDeck(toggle) {
    document.getElementById("deck").classList.toggle("active", toggle);
    if (toggle) {
        document.getElementById("deck").onclick = () => Action("deck");
    }
    else {
        document.getElementById("deck").onclick = null;
    }
}
/**
 * Zmienia aktywność stosu
 * @param {boolean} toggle Aktywność
 */
function ToggleStack(toggle) {
    document.getElementById("stack").classList.toggle("active", toggle);
    if (toggle) {
        document.getElementById("stack").onclick = () => Action("stack");
    }
    else {
        document.getElementById("stack").onclick = null;
    }
}

/**
 * Ustawia przycisk na dole strony
 * @param {string} text Tekst
 * @param {boolean} active Aktywność
 */
function RefreshButton(text, visible) {
    var btn = document.getElementById("button");
    btn.innerHTML = text;

    if (visible) {
        btn.classList.remove(["disp-none"]);
    }
    else {
        btn.classList.add(["disp-none"]);
    }
}
/**
 * Ustawia informacje o rundzie
 * @param {string} number Runda
 * @param {boolean} visible Widoczność
 */
function RefreshRound(number, visible) {
    document.getElementById("round").innerHTML = number;

    if (visible) {
        document.getElementById("round-text").classList.remove(["disp-none"]);
        document.getElementById("round").classList.remove(["disp-none"]);
    }
    else {
        document.getElementById("round-text").classList.add(["disp-none"]);
        document.getElementById("round").classList.add(["disp-none"]);
    }
}
/**
 * Ustawia przycisk dutch
 * @param {boolean} active Aktywny
 */
function RefreshDutchButton(active) {
    if (active) {
        document.getElementById("dutch").classList.remove(["disp-none"]);
    }
    else {
        document.getElementById("dutch").classList.add(["disp-none"]);
    }
}
/**
 * Zwraca odmieniony wyraz "punkty"
 * @param {number} number liczba
 */
function GetDeclination(number) {
    if (number == 1) return "punkt";
    if (number <= 4) return "punkty";
    if (number < 20) return "punktów";
    if (number % 10 >= 2 && number % 10 <= 4) return "punkty";
    return "punktów";
}
/**
 * Ustawia podsumowanie punktacji
 * @param {number} bot_sum Suma punktów bota
 * @param {number} player_sum Suma punktów gracza
 */
function ChangeVisibilitySummary(bot_sum, player_sum) {
    if (bot_sum == -1 && player_sum == -1) {
        document.getElementById("bot_cards_summary").innerHTML = "";
        document.getElementById("player_cards_summary").innerHTML = "";
        return;
    }

    document.getElementById("bot_cards_summary").innerHTML = "Komputerowy przeciwnik ma " + String(bot_sum) + " " + GetDeclination(bot_sum) + ".";
    document.getElementById("player_cards_summary").innerHTML = "Ty masz " + String(player_sum) + " " + GetDeclination(player_sum) + ".";
}

// Action
/**
 * Funkcja wysyła zapytanie do serwera i po uzyskaniu odpowiedzi zmiania stan gry.  
 * @param {string} id Id przycisku wywołującego
 */
function Action(id) {
    $.ajax({
        url: "/Game/Action",
        data: {"id": id},
        type: "POST",
        dataType: "json",
        success: function (data) {
            state = data.State;

            if (data.RefreshAllCards) {
                Refresh(data.Args[0], data.Args[1]);
            }
            else if (data.RefreshSingleCard) {
                for (var i = 0; i < data.Args.length; i++) {
                    RefreshCard(data.Args[i]);
                }
            }

            if (data.Refresh["deck"] == true) {
                ToggleDeck(state["deck"].Active);
            }
            if (data.Refresh["stack"] == true) {
                RefreshStack(state["stack"].Card, state["stack"].Active);
            }
            if (data.Refresh["button"] == true) {
                RefreshButton(state["button"].Card.Value, state["button"].Active);
            }
            if (data.Refresh["round"] == true) {
                RefreshRound(state["round"].Card.Value, state["round"].Active);
            }
            if (data.Refresh["dutch"] == true) {
                RefreshDutchButton(state["dutch"].Active);
            }
            if (data.Refresh["summary"] == true) {
                ChangeVisibilitySummary(state["bot_summary"].Card.Value, state["player_summary"].Card.Value);
            }
        },
        error: function () {
            alert("Wystąpił błąd podczas przetwarzania twojego ruchu.");
        }
    });
}