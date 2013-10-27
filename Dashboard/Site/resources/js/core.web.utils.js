

function getGraphicsExtent(graphics) {
    var geometry, extent, ext;

    dojo.forEach(graphics, function (graphic, i) {
        geometry = graphic.geometry;

        if (geometry instanceof esri.geometry.Point) {
            ext = new esri.geometry.Extent(geometry.x - 1, geometry.y - 1, geometry.x + 1, geometry.y + 1, geometry.spatialReference);
        }
        else if (geometry instanceof esri.geometry.Extent) {
            ext = geometry;
        }
        else {
            ext = geometry.getExtent();
        }

        if (extent) {
            extent = extent.union(ext);
        }
        else {
            extent = new esri.geometry.Extent(ext);
        }
    });
    return extent;
}

function clickMap(evt) {

    //Geocode Reverso
    if (evt.ctrlKey) {
        standby.show();
        var geom = esri.geometry.webMercatorToGeographic(evt.mapPoint);

        dijit.byId('txtLongitude').set('value', geom.x);
        dijit.byId('txtLatitude').set('value', geom.y);

        dijit.byId('optCoordenadas').set('checked', true);
        mudarLabelEndereco(undefined);

        doGeocode();
        standby.hide();
    } //Geocode Reverso
    else {
        if (glGeocode.graphics.length > 0) {
            var ponto = glGeocode.graphics[0].geometry;
            map.infoWindow.show(ponto, esri.dijit.InfoWindow.ANCHOR_UPPERRIGHT);
        }
    }
}

function getLayerById(name) {


}

function removeTabChilds(tabName) {

    ///Removing chart
    charts = [];

    ///Child
    var children = dijit.byId(tabName).getChildren();

    ///For each child
    for (var childIndex = 0; childIndex < children.length; childIndex++) {
        dijit.byId(tabName).removeChild(children[childIndex]);
        children[childIndex].destroy();
    }
}

///The normalize characteres
var normalizeCharacteres = [{ char: ' ', url: '-20' }, { char: '&', url: '-26' },
                            { char: 'À', url: '-C0' }, { char: 'Á', url: '-C1' }, { char: 'Â', url: '-C2' }, { char: 'Ã', url: '-C3' },
                            { char: 'Ä', url: '-C4' }, { char: 'Å', url: '-C5' }, { char: 'Ç', url: '-C7' }, { char: 'È', url: '-C8' },
                            { char: 'É', url: '-C9' }, { char: 'Ê', url: '-CA' }, { char: 'Ë', url: '-CB' }, { char: 'Ì', url: '-CC' },
                            { char: 'Í', url: '-CD' }, { char: 'Î', url: '-CE' }, { char: 'Ï', url: '-CF' }, { char: 'Ð', url: '-D0' },
                            { char: 'Ñ', url: '-D1' }, { char: 'Ò', url: '-D2' }, { char: 'Ó', url: '-D3' }, { char: 'Ô', url: '-D4' },
                            { char: 'Õ', url: '-D5' }, { char: 'Ö', url: '-D6' }, { char: '×', url: '-D7' }, { char: 'Ø', url: '-D8' },
                            { char: 'Ù', url: '-D9' }, { char: 'Ú', url: '-DA' }, { char: 'Û', url: '-DB' }, { char: 'Ü', url: '-DC' },
                            { char: 'Ý', url: '-DD' }, { char: 'à', url: '-E0' }, { char: 'á', url: '-E1' }, { char: 'â', url: '-E2' },
                            { char: 'ã', url: '-E3' }, { char: 'ä', url: '-E4' }, { char: 'å', url: '-E5' }, { char: 'æ', url: '-E6' },
                            { char: 'ç', url: '-E7' }, { char: 'è', url: '-E8' }, { char: 'é', url: '-E9' }, { char: 'ê', url: '-EA' },
                            { char: 'ë', url: '-EB' }, { char: 'ì', url: '-EC' }, { char: 'í', url: '-ED' }, { char: 'î', url: '-EE' },
                            { char: 'ï', url: '-EF' }, { char: 'ð', url: '-F0' }, { char: 'ñ', url: '-F1' }, { char: 'ò', url: '-F2' },
                            { char: 'ó', url: '-F3' }, { char: 'ô', url: '-F4' }, { char: 'õ', url: '-F5' }, { char: 'ö', url: '-F6' },
                            { char: '÷', url: '-F7' }, { char: 'ø', url: '-F8' }, { char: 'ù', url: '-F9' }, { char: 'ú', url: '-FA' },
                            { char: 'û', url: '-FB' }, { char: 'ü', url: '-FC' }, { char: 'ý', url: '-FD' }, { char: 'ÿ', url: '-FF'}];


// Read a page's GET URL variables and return them as an associative array.
function getUrlVars() {
    var vars = [], hash;
    var hashes;

    if (window.location.href.indexOf('?') == -1)
        hashes = [];
    else
        hashes = desnormalizeUrl(window.location.href.slice(window.location.href.indexOf('?') + 1)).split('&');

    for (var i = 0; i < hashes.length; i++) {
        hash = hashes[i].split('=');
        vars.push(hash[0]);
        vars[hash[0]] = hash[1];
    }

    return vars;
}

function normalizeUrl(url) {

    var newUrl = url;

    ///For each char
    for (var charactereIndex = 0; charactereIndex < normalizeCharacteres.length; charactereIndex++)
        newUrl = newUrl.replace(new RegExp(normalizeCharacteres[charactereIndex].char, 'g'), normalizeCharacteres[charactereIndex].url)

    return newUrl;

}

function desnormalizeUrl(url) {

    var newUrl = url;

    ///For each char
    for (var charactereIndex = 0; charactereIndex < normalizeCharacteres.length; charactereIndex++)
        newUrl = newUrl.replace(new RegExp(normalizeCharacteres[charactereIndex].url, 'g'), normalizeCharacteres[charactereIndex].char)

    return newUrl;

}

function Set_Cookie(name, value, expires, path, domain, secure) {
    // set time, it's in milliseconds
    var today = new Date();
    today.setTime(today.getTime());

    if (expires) {
        expires = expires * 1000 * 60 * 60 * 24;
    }
    var expires_date = new Date(today.getTime() + (expires));

    document.cookie = name + "=" + escape(value) +
        ((expires) ? ";expires=" + expires_date.toGMTString() : "") +
        ((path) ? ";path=" + path : "") +
        ((domain) ? ";domain=" + domain : "") +
        ((secure) ? ";secure" : "");
}

function Get_Cookie(name) {
    var retorno = '';

    if (name != undefined) {
        try {
            var start = document.cookie.indexOf(name + "=");
            var len = start + name.length + 1;

            if ((!start) && (name != document.cookie.substring(0, name.length))) {
                return null;
            }

            if (start == -1)
                return null;

            var end = document.cookie.indexOf(";", len);

            if (end == -1)
                end = document.cookie.length;

            retorno = unescape(document.cookie.substring(len, end));
        } catch (e) {

        }

        return retorno;
    }
}

function showMessage(content, title) {

    var dialog = dijit.byId('dialog');
    dialog.set('title', title);
    dialog.set('content', content);

    dialog.show();

    dijit.byId('btnGeocode').setLabel("Buscar", 500);
}

function isMobile() {
    if (navigator.userAgent.match(/iPad/i) != null)
        return 'iPad';
    else if (navigator.userAgent.match(/iPhone/i) != null)
        return 'iPhone';
    else if (navigator.userAgent.match(/iPod/i) != null)
        return 'iPod';
    else
        return '';
}

var userAccessLogin;

function ShowForm(title, content) {

    ///Creates the dialog
    var dialog = new dijit.Dialog({
        title: title,
        content: content,
        onShow: function () { }
    });

    ///Shows the dialog
    dialog.show();

    return dialog;
}

function setDijitVisibility(controlId, visible) {
    dojo.style(dijit.byId(controlId).domNode, {
        visibility: (!visible ? 'hidden' : 'visible'),
        display: (!visible ? 'none' : 'block')
    });

    dijitRefresh('mainWindow');
}

function setDijitEnable(controlId, enabled) {

    dijit.byId(controlId).set('disabled', !enabled);
}

function dijitRefresh(controlId) {

    dijit.byId(controlId).resize();

    ///Resizes the map
    if (controlId == 'mainWindow')
        map.resize();
}

function removeItemFromArray(originalArray, itemToRemove) {

    var j = 0;

    while (j < originalArray.length) {

        if (originalArray[j] == itemToRemove) {
            originalArray.splice(j, 1);
        } else { j++; }
    }
}

function ArrayRemoveItem(originalArray, itemToRemove) {

    var j = 0;

    while (j < originalArray.length) {

        if (originalArray[j] == itemToRemove) {
            originalArray.splice(j, 1);
        } else { j++; }
    }
}

function ArrayAddItem(originalArray, itemToAdd) {

    var j = 0;

    var exists = false;

    while (j < originalArray.length) {

        if (originalArray[j] == itemToAdd) {

            exists = true;
            break;
        }
        else { j++; }
    }

    if (!exists)
        originalArray.push(itemToAdd);
}

///Gets a specific graphic of a graphics layer
function setSymbol(graphicsLayer, symbols, attribute) {

    ///for each graphic
    $(graphicsLayer.graphics).each(function (graphicsIndex, graphic) {

        if (graphic.symbol != null && graphic.symbol.type == 'textsymbol') return;

        ///Creating the symbol
        var graphicSymbol = graphic.symbol == null ? new esri.symbol.SimpleMarkerSymbol() : graphic.symbol; ;

        ///Verify the length
        if (symbols.length == 1) {

            ///Setting the symbol properties
            //graphicSymbol.setColor(symbols[0].color == undefined ? graphic.symbol.color : new dojo.Color(symbols[0].color));
            graphicSymbol.setSize(symbols[0].size == undefined ? graphic.symbol.size : symbols[0].size);
            //graphicSymbol.setStyle(symbols[0].style == undefined ? graphic.symbol.style : symbols[0].style);

            ///Setting the symbol
            graphic.setSymbol(graphicSymbol);
        }
        else {
            ///for each symbol
            $(symbols).each(function (symboIndex, symbol) {

                ///Verify if attribute is undefined
                if (attribute != undefined) {
                    if ($(graphic.attributes.data).attr(attribute) == symbol.value) {

                        ///Setting the symbol properties
                        graphicSymbol.setColor(symbol.color == undefined ? graphic.symbol.color : new dojo.Color(symbol.color));
                        graphicSymbol.setSize(symbol.size == undefined ? graphic.symbol.size : symbol.size);
                        graphicSymbol.setStyle(symbol.style == undefined ? graphic.symbol.style : symbol.style);

                        ///Setting the symbol
                        graphic.setSymbol(graphicSymbol);
                        return false;
                    }
                }
                else {
                    ///Setting the symbol properties
                    graphicSymbol.setColor(new dojo.Color(symbol.color == undefined ? graphic.symbol.color : symbol.color));
                    graphicSymbol.setSize(symbol.size == undefined ? graphic.symbol.size : symbol.size);
                    graphicSymbol.setStyle(symbol.style == undefined ? graphic.symbol.style : symbol.style);

                    ///Setting the symbol
                    graphic.setSymbol(graphicSymbol);
                }
            });
        }
    });
}

///Gets a specific row of a specific table
function setZebra(tableId, oddClass, evenClass) {

    ///For each row
    $(document.getElementById(tableId).rows).each(function (rowIndex, row) {

        ///For each row
        $(row).attr('class', ((rowIndex % 2) == 0 ? evenClass : oddClass));
    });
}

///Gets a specific graphic of a graphics layer
function getGraphics(graphicsLayer, attribute, value) {

    ///The return graphic
    var returnGraphics = [];

    ///for each graphic
    $(graphicsLayer.graphics).each(function (graphicsIndex, graphic) {

        if (graphic.symbol != null && graphic.symbol.type == 'textsymbol') return;

        ///Getting the data object
        var dataObject = null;

        ///Verifying the attributes
        if (graphic.attributes.data != undefined)
            dataObject = graphic.attributes.data;
        else
            dataObject = graphic.attributes;

        ///Verifying the attribute
        if ($(dataObject).attr(attribute) == value) {
            returnGraphics.push(graphic);
        }
    });

    ///Returning the graphic
    return returnGraphics;
}

///Gets a specific row of a specific table
function getRows(tableId, column, value) {

    ///The column index and the return row
    var columnIndex;
    var returnRows = [];

    ///For each row
    $(document.getElementById(tableId).rows).each(function (rowIndex, row) {

        ///If the row index is 0, its the header
        if (rowIndex == 0) {

            ///For each row
            $(row.cells).each(function (cellIndex, cell) {

                ///If the inner text is equals the column name
                if (cell.innerText == column) {
                    columnIndex = cellIndex;
                    return false;
                }
            });
        }
        else {
            if (row.cells[columnIndex].innerText == value) {
                returnRows.push(row);
            }
        }

    });

    ///Returning the row
    return returnRows;
}

///Get tabs
function getTabs(tabContainerId, tabAttribute, value) {

    ///Getting the tab
    var tabContainer = dijit.byId(tabContainerId);

    ///Child
    var childrens = tabContainer.getChildren();

    ///The tab controller variable
    var tabs = [];

    ///For each child
    for (var childrenIndex = 0; childrenIndex < childrens.length; childrenIndex++) {

        ///Getting the children
        var children = childrens[childrenIndex];

        ///Setting the selected
        if (children[tabAttribute] == value)
            tabs.push(children);
    }

    ///Returning the tabs
    return tabs;
}

///Remove childs
function removeChilds(obj) {

    if (obj.hasChildNodes()) {
        while (obj.childNodes.length >= 1) {
            obj.removeChild(obj.firstChild);
        }
    }
}

function formatNumberMask(src, mask) {

    var i = src.get('displayedValue').length;
    var out = mask.substring(i, i + 1);

    if (out != '#')
        src.set('displayedValue', src.get('displayedValue') + out);
}
