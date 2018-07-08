var doc = $(document);
var win = $(window);

class Config { constructor() { throw new Error("Static class"); } }
Config.mainPopupId = "popup";
Config.alertPopupId = "alert-popup";