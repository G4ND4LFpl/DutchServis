// Deck class
class Deck {
    // private variebles
    #deck;
    // public variebles
    size = 52;

    // constructor
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
    Shuffle() {
        this.size = 52;
        for (var i = 0; i < 52; i++) {
            this.deck[i].ondeck = true;
        }
    }
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

// variebles
var deck;
var bot_cards = [];
var player_cards = [];
var stacktop = null;
var mode;
var num;

// Tryb
/* lookup */

// Visual functions
function colorClass(color) {
    if (color === "kier" || color === "karo") return "c-red";
    else return "c-black";
}
function getCardButton(id, color, colorImg, value, active) {
    if (active) {
        var hover = "active";
        var onclick = "onclick=\"clickCard('" + id + "')\"";
    }
    else {
        var hover = "";
        var onclick = "";
    }

    var html = ""; 

    html += "<button class=\"center-block card " + hover + "\">";

    html += "<div class=\"row\">";
    html += "<img class=\"card-color f-left\" src=\"" + path + "images/" + colorImg + "\"/>";
    html += "</div>";

    html += "<div class=\"row\">";
    html += "<div class=\"card-awers " + color + "\">" + value + "</div>";
    html += "</div>";

    html += "<div class=\"row\">";
    html += "<img class=\"card-color f-right\" src=\"" + path + "/images/" + colorImg + "\"/>";
    html += "</div>";

    html += "</button>";

    return html;
}
function getEmptyButton(id, active) {
    if (active) {
        var hover = "active";
        var onclick = "onclick=\"clickCard('" + id + "')\"";
    }
    else {
        var hover = "";
        var onclick = "";
    }

    var html = "";

    html += "<button class=\"center-block card " + hover + "\">";

    html += "<div class=\"row\">";
    html += "<div class=\"card-empty\">brak</div>";
    html += "</div>";

    html += "</button>";

    return html;
}
function getReverseButton(id, active) {
    if (active) {
        var hover = "active";
        var onclick = "onclick=\"clickCard('" + id + "')\"";
    }
    else {
        var hover = "";
        var onclick = "";
    }

    var html = "";

    html += "<button class=\"center-block card " + hover + " backgroud-img\"" + onclick + "></button>";

    return html;
}

function refresh() {
    document.getElementById("deck").classList.toggle("active", false);

    for (var i = 0; i < 4; i++) {
        // bot
        if (bot_cards[i].card === null) {
            document.getElementById("bot_card_" + i).innerHTML = getEmptyButton(i, false);
        }
        else if (bot_cards[i].visible === false) {
            document.getElementById("bot_card_" + i).innerHTML = getReverseButton(i, false);
        }
        else {
            document.getElementById("bot_card_" + i).innerHTML = getCardButton(
                i,
                colorClass(bot_cards[i].card.color),
                bot_cards[i].card.color + ".png",
                bot_cards[i].card.value,
                false
            );
        }

        // player
        if (bot_cards[i].card === null) {
            document.getElementById("player_card_" + i).innerHTML = getEmptyButton(i + 10, false);
        }
        else if (player_cards[i].visible === false) {
            document.getElementById("player_card_" + i).innerHTML = getReverseButton(i + 10, mode == "lookup");
        }
        else {
            document.getElementById("player_card_" + i).innerHTML = getCardButton(
                i + 10,
                colorClass(player_cards[i].card.color),
                player_cards[i].card.color + ".png",
                player_cards[i].card.value,
                false
            );
        }
    }
}
function refreshCard(id) {
    if (id < 10) {
        // bot
        if (bot_cards[id].card === null) {
            document.getElementById("bot_card_" + id).innerHTML = getEmptyButton(id, false);
        }
        else if (bot_cards[id].visible === false) {
            document.getElementById("bot_card_" + id).innerHTML = getReverseButton(id, false);
        }
        else {
            document.getElementById("bot_card_" + id).innerHTML = getCardButton(
                id,
                colorClass(bot_cards[id].card.color),
                bot_cards[id].card.color + ".png",
                bot_cards[id].card.value,
                false
            );
        }
    }
    else {
        id = id - 10;
        // player
        if (bot_cards[id].card === null) {
            document.getElementById("player_card_" + id).innerHTML = getEmptyButton(id + 10, false);
        }
        else if (player_cards[id].visible === false) {
            document.getElementById("player_card_" + id).innerHTML = getReverseButton(id + 10, false);
        }
        else {
            document.getElementById("player_card_" + id).innerHTML = getCardButton(
                id + 10,
                colorClass(player_cards[id].card.color),
                player_cards[id].card.color + ".png",
                player_cards[id].card.value,
                false
            );
        }
    }
}

function start() {
    deck = new Deck();

    bot_cards = [];
    player_cards = [];
    mode = "lookup";
    num = 2;

    for (var i = 0; i < 4; i++) {
        bot_cards.push({ card: deck.Draw(), visible: false});
        player_cards.push({ card: deck.Draw(), visible: false });
    }
    refresh();
}

function clickCard(id) {
    switch (mode) {
        case "lookup": {
            player_cards[id - 10].visible = true;
            refreshCard(id);
            num --;
            break;
        }
    }
}