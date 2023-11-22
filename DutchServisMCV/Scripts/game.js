// Deck class
class Deck {
    // private variebles
    #deck;
    // public variebles
    size = 52;

    // constructor
    /**
     * Tworzy zakryty stos kart
     */
    constructor() {
        this.deck = [];
        var colors = ["pik", "kier", "karo", "trefl"];

        for (var c = 0; c < 4; c++) {
            this.deck.push({ card: { color: colors[c], value: "A" }, ondeck: true });
            for (var i = 2; i < 11; i++) {
                this.deck.push({card: { color: colors[c], value: String(i) }, ondeck: true });
            }
            this.deck.push({ card: { color: colors[c], value: "J" }, ondeck: true });
            this.deck.push({ card: { color: colors[c], value: "Q" }, ondeck: true });
            this.deck.push({ card: { color: colors[c], value: "K" }, ondeck: true });
        }
    }

    // public functions
    /**
     * Przywraca wszystkie karty na stercie
     */
    Shuffle() {
        this.size = 52;
        for (var i = 0; i < 52; i++) {
            this.deck[i].ondeck = true;
        }
    }
    /**
     * Funkcja zwraca kartę i usuwa ją ze sterty
     */
    Draw() {
        do {
            var idx = Math.floor(Math.random() * 52);
        }
        while (this.deck[idx].ondeck === false)

        this.deck[idx].ondeck = false;
        this.size--;
        return this.deck[idx].card;
    }
}

// Variebles
var deck;
var bot_cards = [];
var player_cards = [];
var stack = [];
var mode;
var num;

// Mode
const Mode = {
    Lookup: "Lookup",
    Wait: "Wait",
    Draw: "Draw",
    Throw: "Throw",
    Peek: "Peek"
}

// Helper functions

/**
 * Returns css class for color
 * @param {string} color Card color
 */
function ColorClass(color) {
    if (color === "kier" || color === "karo") return "c-red";
    else return "c-black";
}
/**
 * Zwraca index w tablicy dla podanego id
 * @param {string} id
 */
function GetIndex(id) {
    return Number(id.split('_')[2]);
}

// Visual functions

/**
 * Ustawia wybranemu przyciskowi puste miejsce
 * @param {string} id Html id przycisku
 * @param {boolean} active Indykacja czy aktywny
 * @param {boolean} showEmpty Wyświetlanie pola, jeśli puste
 */
function SetEmpty(id, active, showEmpty) {
    if (!showEmpty) {
        document.getElementById("col_" + id).classList.toggle("disp-none", true);
    }

    var card = document.getElementById(id);
    if (active) {
        card.classList.toggle("active", true);
        card.onclick = Click;
    }
    else {
        card.classList.toggle("active", false);
        card.onclick = () => Click(card.id);
    }

    var img = "<img src=\"" + path + "images/cards/card_empty.png\")\" class=\"img-responsive\" />";
    var text = "<div class=\"text-empty\">Puste</div>";
    card.innerHTML = img + text;
}
/**
 * Ustawia wybranemu przyciskowi reverse karty
 * @param {string} id Html id przycisku
 * @param {boolean} active Indykacja czy aktywny
 * @param {boolean} showEmpty Wyświetlanie pola, jeśli puste
 */
function SetCardReverse(id, active, showEmpty) {
    if (!showEmpty) {
        document.getElementById("col_" + id).classList.toggle("disp-none", false);
    }

    var card = document.getElementById(id);
    if (active) {
        card.classList.toggle("active", true);
        card.onclick = () => Click(card.id);
    }
    else {
        card.classList.toggle("active", false);
        card.onclick = null;
    }

    card.innerHTML = "<img src=\"" + path + "images/cards/card_reverse.png" + "\")\" class=\"img-responsive\" />";
}
/**
 * Ustawia wybranemu przyciskowi avers karty
 * @param {string} id Html id przycisku
 * @param {string} color Kolor karty
 * @param {string} value Wartość karty
 * @param {boolean} active Indykacja czy aktywny
 * @param {boolean} showEmpty Wyświetlanie pola, jeśli puste
 */
function SetCard(id, color, value, active, showEmpty) {
    if (!showEmpty) {
        document.getElementById("col_" + id).classList.toggle("disp-none", false);
    }

    var card = document.getElementById(id);
    if (active) {
        card.classList.toggle("active", true);
        card.onclick = () => Click(card.id);
    }
    else {
        card.classList.toggle("active", false);
        card.onclick = null;
    }

    var img = "<img src=\"" + path + "images/cards/card_" + color + ".png\")\" class=\"img-responsive\" />";
    var text = "<div class=\"text-avers " + ColorClass(color) + "\">" + value + "</div>";
    card.innerHTML = img + text;
}

/**
 * Odświeża widok przycisku, jeśli ten spełnia warunek.
 * Warunkiem może być:
 * 'all' - odświeża każdy przycisk
 * 'empty' - odświerza tylko puste pola
 * 'reverse' - odświeża tylko zakryte karty
 * 'avers' - odświeża tylko odkryte karty
 * @param {string} id Html id przycisku
 * @param {string} card Warunek, aby przycisk był odświeżony
 */
function RefreshCard(id, card="all") {
    idx = GetIndex(id);

    // debug
    document.getElementById("debug").innerHTML = mode;

    if (id.includes("bot")) {
        // Bot card
        if (idx == 4) {
            ToggleOffSet("bot", bot_cards[idx].card == null);
        }

        if (bot_cards[idx].card == null && (card == "all" || card == "empty")) {
            SetEmpty(id, false, idx < 4);
        }
        else if (bot_cards[idx].visible == false && (card == "all" || card == "reverse")) {
            SetCardReverse(id, false, idx < 4);
        }
        else if (card == "all" || card == "avers") {
            SetCard(id, bot_cards[idx].card.color, bot_cards[idx].card.value, false, idx < 4);
        }
    }
    else {
        // Player card
        if (idx == 4) {
            ToggleOffSet("player", player_cards[idx].card == null);
        }

        if (player_cards[idx].card == null) {
            if (card == "all" || card == "empty") {
                SetEmpty(id, false, idx < 4);
            }
        }
        else if (player_cards[idx].visible == false) {
            if (card == "all" || card == "reverse") {
                SetCardReverse(id, mode == Mode.Lookup || mode == Mode.Throw, idx < 4);
            }
        }
        else if (player_cards[idx].visible == true) {
            if (card == "all" || card == "avers") {
                SetCard(id, player_cards[idx].card.color, player_cards[idx].card.value, false, idx < 4);
            }
        }
    }
}
/**
 * Odświeża widok kart na planszy w określonym wierszu i spełniających warunek
 * Wierszem może być: 'both', 'bot' lub 'player'
 * Warunkiem może być: 'all', 'empty', 'reverse' lub 'avers'
 * @param {string} row Wiersz kart (Wartość domyślna 'both')
 * @param {string} card Warunek, aby przycisk był odświeżony (Wartość domyślna 'all')
 */
function Refresh(row = "both", card = "all") {
    for (var i = 0; i < 8; i++) {
        // Bot cards
        if (row != "player") {
            RefreshCard("bot_card_" + i, card);
        }

        // Player cards
        if (row != "bot") {
            RefreshCard("player_card_" + i, card);
        }
    }
}
/**
 * Odświeża wierzch odkrytego stosu
 */
function RefreshStack() {
    if (stack.length != 0) {
        var card = stack[stack.length - 1];
        SetCard("stack", card.color, card.value, mode == mode.Draw, true);
    }
    else {
        SetEmpty("stack", false, true);
    }
}

/**
 * Zmienia aktywność deku
 * @param {boolean} toggle Aktywność
 */
function ToggleDeck(toggle) {
    document.getElementById("deck").classList.toggle("active", toggle);
    if (toggle) {
        document.getElementById("deck").onclick = () => Draw("deck");
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
        document.getElementById("stack").onclick = () => Draw("stack");
    }
    else {
        document.getElementById("stack").onclick = null;
    }
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

// Startup function

/**
 * Rozkłada karty dla graczy i rozpoczyna fazę podglądania na start
 */
function Deal() {
    deck = new Deck();

    bot_cards = [];
    player_cards = [];
    mode = Mode.Lookup;
    num = 2;

    for (var i = 0; i < 4; i++) {
        bot_cards.push({ card: deck.Draw(), visible: false });
        player_cards.push({ card: deck.Draw(), visible: false });
    }
    for (var i = 0; i < 4; i++) {
        bot_cards.push({ card: null, visible: false });
        player_cards.push({ card: null, visible: false });
    }

    ToggleDeck(false);
    Refresh();
}
/**
 * Ukrywa karty i rozpoczyna rozgrywkę
 */
function Start() {
    document.getElementById("start_button").classList.add(["disp-none"]);
    for (var i = 0; i < 4; i++) {
        player_cards[i].visible = false;
    }
    Refresh("player");
    mode = Mode.Draw;
    ToggleDeck(true);

    // debug
    document.getElementById("debug").innerHTML = mode;
}

// Main Logic Functions

/**
 * Realizuje podejrzenie jednej z swoich kart na początku gry
 * @param {string} id Html id karty
 */
function Lookup(id) {
    player_cards[GetIndex(id)].visible = true;
    num--;

    if (num == 0) {
        mode = Mode.Wait;
        document.getElementById("start_button").classList.remove(["disp-none"]);
        Refresh("player");
    }
    else RefreshCard(id);
}
/**
 * Realizuje dobranie karty przez gracza
 * @param {string} from Źródło karty 'deck' lub 'stack'
 */
function Draw(from) {
    if (from == "deck") {
        var card = deck.Draw();
    }
    else {
        var card = stack.pop();
        RefreshStack();
    }

    for (var i = 0; i < 8; i++) {
        if (player_cards[i].card == null) {
            player_cards[i] = { card: card, visible: true }
            ToggleDeck(false);
            mode = Mode.Throw;
            Refresh("player");
            return;
        }
    }

    throw new DOMException("Dobieranie karty nie powiodło się! Limit kart w ręce (8) osiągnięty.");
}
/**
 * Realizuje wyrzucenie karty z turze
 * @param {string} id Html id karty
 */
function Throw(id) {
    var idx = GetIndex(id);

    stack.push(player_cards[idx].card);
    player_cards[idx].card = null;
    player_cards[idx].visible = false;

    mode = Mode.Draw;

    RefreshStack();
    ToggleStack();
    ToggleDeck();
    RefreshCard(id); //player
}

/**
 * Realizuje reakcję na kliknięcie karty
 * @param {string} id Html id przycisku wywołującego
 */
function Click(id) {
    switch (mode) {
        case Mode.Lookup: {
            Lookup(id);
            break;
        }
        case Mode.Throw: {
            Throw(id);
            break;
        }
    }
}