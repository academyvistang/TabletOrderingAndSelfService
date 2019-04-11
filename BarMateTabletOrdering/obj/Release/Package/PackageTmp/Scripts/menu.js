// product constraction
function menu(id, name, description, price, image) {
    this.id = id;
    this.name = name;
    this.description = description;
    this.price = price;
    this.image = image;
}

// create new products from the product constructor
var menu1 = new menu(
    1,
    "Menu Classic",
    "Sandwich: Burger, Salade, Tomate, Cornichon + Frites + Boisson",
    8.9,
    "https://github.com/ibrahimouachem/Royal-Burger/blob/master/images/m1.png?raw=true"
);
var menu2 = new menu(
    2,
    "Menu Bacon",
    "Sandwich: Burger, Fromage, Bacon, Salade, Tomate + Frites + Boisson",
    9.5,
    "https://github.com/ibrahimouachem/Royal-Burger/blob/master/images/m2.png?raw=true"
);
var menu3 = new menu(
    3,
    "Menu Big",
    "Sandwich: Double Burger, Fromage, Cornichon, Salade + Frites + Boisson",
    10.9,
    "https://github.com/ibrahimouachem/Royal-Burger/blob/master/images/m3.png?raw=true"
);
var menu4 = new menu(
    4,
    "Menu Chicken",
    "Sandwich: Poulet Frit, Tomate, Salade, Mayonnaise + Frites + Boisson",
    9.9,
    "https://github.com/ibrahimouachem/Royal-Burger/blob/master/images/m4.png?raw=true"
);
var menu5 = new menu(
    5,
    "Menu Fish",
    "Sandwich: Poisson, Salade, Mayonnaise, Cornichon + Frites + Boisson",
    10.9,
    "https://github.com/ibrahimouachem/Royal-Burger/blob/master/images/m5.png?raw=true"
);
var menu6 = new menu(
    6,
    "Menu Double Steak",
    "Sandwich: Double Burger, Fromage, Bacon, Salade, Tomate + Frites + Boisson",
    11.9,
    "https://github.com/ibrahimouachem/Royal-Burger/blob/master/images/m6.png?raw=true"
);

// create an array to add products to
var menuArray = new Array();

// add products to array of products

menuArray.push(menu1, menu2, menu3, menu4, menu5, menu6);

// loop through products array
for (var i = 0; i < menuArray.length; i++) {
    // creation the element
    var newItem = document.createElement("div");
    var newDiv = document.createElement("div");
    var idH1 = document.createElement("h1");
    var nameH3 = document.createElement("h3");
    var descriptionH5 = document.createElement("h5");
    var priceH4 = document.createElement("h4");
    var image = document.createElement("img");
    var btn = document.createElement("button");
    var icon = document.createElement("glyphicon");

    // create texte for the element

    var tid = document.createTextNode(menuArray[i].id);
    var tName = document.createTextNode(menuArray[i].name);
    var tDescription = document.createTextNode(menuArray[i].description);
    var tPrice = document.createTextNode("$ " + menuArray[i].price);
    var tButton = document.createTextNode(" Commander");

    // update source attribute
    image.src = menuArray[i].image;

    image.className = "img-responsive";
    btn.className = "btn-order";
    newItem.className = "col-sm-6 col-md-4";
    newDiv.className = "menu menuName" + [i] + " thumbnail";
    priceH4.className = "price";
    descriptionH5.className = "caption > p";
    icon.className = "glyphicon glyphicon-shopping-cart";

    // add text to elements
    idH1.appendChild(tid);
    nameH3.appendChild(tName);
    descriptionH5.appendChild(tDescription);
    priceH4.appendChild(tPrice);
    btn.appendChild(icon);
    btn.appendChild(tButton);

    // add elements to new div
    newDiv.appendChild(nameH3);
    newDiv.appendChild(image);
    newDiv.appendChild(idH1);
    newDiv.appendChild(descriptionH5);
    newDiv.appendChild(priceH4);
    newDiv.appendChild(btn);

    // add new div to new item div
    newItem.appendChild(newDiv);

    // add new item to the element with id="shirts"
    document.getElementById("menu").appendChild(newItem);
}
