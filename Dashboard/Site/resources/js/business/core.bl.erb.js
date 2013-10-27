///core.bl.erb.js
function ERB() {

    this.ColorNormal = '#5eed2c';

    ///Components
    this.ComponentLayerName = 'erbLayer';

    this.ComponentSearchCombobox = 'searchCombobox';
    this.ComponentSearchButton = 'searchButton';

    ///Graphics
    this.GraphicSymbolSelectedSize = 30;
    this.GraphicSymbols = [{ size: 18}];

    ///The query attributes
    this.queryState = "";

    ///Services
    this.geometryService = "http://10.126.111.74/ArcGIS/rest/services/Geometry/GeometryServer";
    this.mapService = "http://10.126.111.213/ArcGIS/rest/services/SIG-DI/ERBs/MapServer/0";
    //this.mapService = "http://10.126.111.213/ArcGIS/rest/services/Desenvolvimento/ERBS_SIG_A/MapServer/0";

    this.Fields = {
    
        Detail: [{ name: 'STATUS_ERB', title: 'Status' },
                 { name: 'TECNOLOGIA', title: 'Tecnologia' },
                 { name: 'SIG_CCC', title: 'Sigla CCC' },
                 { name: 'SIG_BSC', title: 'Sigla BSC' },
                 { name: 'SIG_SITE', title: 'Sigla Site' },
                 { name: 'SIG_LOGICA', title: 'Sigla Lógica' },
                 { name: 'SIG_ERB', title: 'Sigla ERB' },
                 { name: 'POTENCIA', title: 'Potência' },
                 { name: 'BANDA', title: 'Freq'}]
    
    }

    ///Attributes
    this.Attributes = {
    
        Site : { name: 'SIG_SITE', title: 'Sigla Site' },
        State : { name: 'UF', title: 'F' },
        ERB : { name: 'SIG_LOGICA', title: 'Sigla Lógica' },
        ERBAux : { name: 'SIG_ERB', title: 'Sigla ERB' },
        Status : { name: 'STATUS_ERB', title: 'Status' },
        QoS : { name: 'QoS', title: '3G QoS' },
        Symbol : { name: 'TECNOLOGIA', title: 'Tecnologia' }
    };

    ///Attributes
    this.KeyStateAttribute = { name: 'UF_SITE', title: 'UF/Site' };
    this.KeyAttribute = { name: 'SIG_SITE', title: 'Sigla Site' };
    this.StateAttribute = { name: 'UF', title: 'F' };
    this.ErbAttribute = { name: 'SIG_LOGICA', title: 'Sigla Lógica' };
    this.ErbStateAttribute = { name: 'UF_LOGICA', title: 'UF/Sigla Lógica' };
    this.ErbAuxAttribute = { name: 'SIG_ERB', title: 'Sigla ERB' };
    this.StatusAttribute = { name: 'STATUS_ERB', title: 'Status' };
    this.QoSAttribute = { name: 'QoS', title: '3G QoS' };
    this.SymbolAttribute = { name: 'TECNOLOGIA', title: 'Tecnologia' };

    this.TableFields = [{ name: 'STATUS_ERB', title: 'Status' },
                        { name: 'TECNOLOGIA', title: 'Tecnologia' },
                        { name: 'SIG_CCC', title: 'Sigla CCC' },
                        { name: 'SIG_BSC', title: 'Sigla BSC' },
                        { name: 'SIG_SITE', title: 'Sigla Site' },
                        { name: 'SIG_LOGICA', title: 'Sigla Lógica' },
                        { name: 'SIG_ERB', title: 'Sigla ERB' },
                        { name: 'POTENCIA', title: 'Potência' },
                        { name: 'BANDA', title: 'Freq'}];

    ///The list
    this.Sites;
    this.List;

    this.status = 'ready';
    this.Status = function (value) {

        ///Verify the new status
        if (value == undefined)
            return this.status;
        else {
            this.status = value;
        }
    }

    ///Resets the attributes
    this.Refresh = function () {

        ///Processing
        this.Status('processing');
    }


    ///Sets the status to the erbs
    this.SetStatus = function (attributeName, status) {

        ///For each site
        $(erbBL.Sites()).each(function (siteIndex, site) {

            if (site.symbol.type == 'textsymbol') return;

            ///For each erb
            $(site.attributes.ERBs).each(function (erbIndex, erb) {

                if (status == 'processing')
                    erb.attributes[attributeName] = status;
                else if (erb.attributes[attributeName] == 'processing') {
                    if (status != 'error')
                        erb.attributes[attributeName] = 'normal';
                    else
                        erb.attributes[attributeName] = 'error';
                }
            });
        });
    }

    ///Refreshs the erb list
    this.FilterActive = function () {

        ///Refreshs the erb list
        erbBL.List = [];
        erbBL.ListActive = [];

        ///For each site
        $(this.Sites()).each(function (siteIndex, site) {

            if (site.symbol.type == 'textsymbol') return;

            ///For each erb
            $(site.attributes.ERBs).each(function (erbIndex, erb) {

                ///Setting the erb
                if (erb.attributes[erbBL.StatusAttribute.name] != 'P')
                    erbBL.ListActive.push(erb);

                erbBL.List.push(erb);
            });
        });
    }

    this.ConsolidateAttributes = function () {

        for (var i = 0; i < erbBL.List.length; i++) {
            var attr = erbBL.List[i].attributes;

            //teste.push(attr.SIG_LOGICA);

            if (attr.SIG_LOGICA == null)
                attr.SIG_LOGICA = attr.SIG_ERB;
        }
    }

    ///Gets the erbs
    this.Get = function (state, erbAcronyms) {

        ///Refresh and processing
        erbBL.Refresh();

        var whereClause = '';

        ///Creating the where clause
        if (erbAcronyms != undefined) {

            var erbsWhereClause = '';

            $(erbAcronyms).each(function (erbAcronymIndex, erbAcronym) {
                if (erbsWhereClause == '')
                    erbsWhereClause = "'" + erbAcronym + "'";
                else
                    erbsWhereClause += ",'" + erbAcronym + "'";
            });

            whereClause = "(UF + (CASE WHEN SIG_LOGICA IS NULL THEN SIG_ERB ELSE SIG_LOGICA END)) in (" + erbsWhereClause + ")";
        }
        if (state != undefined & state != '') {
            if (whereClause != '')
                whereClause += " AND UF = '" + state + "'";
            else
                whereClause = "UF = '" + state + "'";
        }

        ///The query parameters
        var query = new esri.tasks.Query();
        query.returnGeometry = true;
        query.where = whereClause;
        query.outSpatialReference = map.spatialReference;
        query.outFields = ["*"];

        ///The query task
        var queryTask = new esri.tasks.QueryTask(erbBL.mapService);
        queryTask.execute(query, function (points) {

            erbBL.List = points.features;

            erbBL.ConsolidateAttributes();

            ///Adding to the tab and to the map
            erbBL.Sites = erbBL.CreateSites(erbBL.List);

            //erbBL.FilterActive();

            erbBL.Status('ready');

            eventBL.Layer.Consolidate();

        }, function (err, a, b) {

            showMessage('Não foi possível carregar as ERBs');

            ///Setting the status
            erbBL.Status('error');
            eventBL.Window.CriticalEvents.Error(eventBL.Variables.State);
        });
    }

    ///Gets the erbs
    this.CreateSites = function (erbs) {

        ///Sites
        var sites = [];
        sites['SIGLAS'] = [];

        ///For each erb
        for (var erbIndex = 0; erbIndex < erbs.length; erbIndex++) {

            ///Getting the erb
            var erb = erbs[erbIndex];

            ///The acronym
            var siteAcronym = erb.attributes[erbBL.StateAttribute.name] + erb.attributes[erbBL.KeyAttribute.name];
            var erbAcronym = erb.attributes[erbBL.StateAttribute.name] + erb.attributes[erbBL.ErbAttribute.name];

            erb.attributes[erbBL.ErbStateAttribute.name] = erbAcronym;

            ///Verifying the site
            if (sites[siteAcronym] == undefined) {
                ///Creating the sigla
                sites[siteAcronym] = [];
                sites[siteAcronym][erbBL.StatusAttribute.name] = false;
                sites['SIGLAS'].push(siteAcronym);
            }

            if (erb.attributes[erbBL.StatusAttribute.name] == 'P')
                sites[siteAcronym][erbBL.StatusAttribute.name] = true;

            ///Adding the site
            sites[siteAcronym].push(erb);
        }

        return sites;
    }

    ///Selects in the map
    this.Select = function (graphic, siteValue, erbValue) {

        ///Returns
        if (object.symbol.type == 'textsymbol') return;
        

        ///Getting the rows
        rows = getRows(erbBL.ComponentTabTableId, erbBL.KeyAttribute.title, siteValue);

        ///Setting the default siteValues
        setSymbol(erbBL.Layer(), erbBL.GraphicSymbols);

        ///Setting the class to the rows
        graphic.symbol.setSize(erbBL.GraphicSymbolSelectedSize);

        ///Zooming
        zoomTo(graphic.geometry);

        ///Refreshing
        erbBL.Layer().refresh();
    }

    ///The on map clicm method
    this.OnLayerClick = function (erb) {

        ///Selecting
        erbBL.Select(erb.graphic);
    }

    ///Refresh the symbols
    this.RefreshSymbols = function () {

        ///vefify the geocode status
        //if (geocodeBL.Status() != 'done') return;

        ///For each site
        $(this.Sites()).each(function (siteIndex, site) {

            ///Verify the textsymbol
            if (site.symbol.type == 'textsymbol') return;

            ///Verify the site status
            if (site.attributes.Status() == 'normal') {

                ///Setting the symbol
                site.symbol.setColor(erbBL.ColorNormal);
                site.show();

                ///Refreshing
                erbBL.Layer().refresh(true);

            }
        });

        ///Sets the symbols
        setSymbol(erbBL.Layer(), erbBL.GraphicSymbols);
    }

    ///Add the tabs to the detail item
    this.Table = function (erb) {

        ///Creating the detail table
        var detailTable = document.createElement('table');
        $(detailTable).attr('style', 'width :100%');

        ///Creating the right index

        var rightIndex = (((this.TableFields.length / 2) | 0) + (this.TableFields.length % 2 == 0 ? 0 : 1));

        ///For each detail field
        for (var leftIndex = 0; leftIndex < this.TableFields.length && leftIndex < (((this.TableFields.length / 2) | 0) + (this.TableFields.length % 2 == 0 ? 0 : 1)); leftIndex++) {

            ///Creating the variables
            var row = document.createElement('tr');
            var columnLeftField = document.createElement('td');
            var columnLeftValue = document.createElement('td');
            var columnRightField = document.createElement('td');
            var columnRightValue = document.createElement('td');

            columnLeftField.setAttribute('style', 'font-weight: bold; width: 25%');
            columnRightField.setAttribute('style', 'font-weight: bold; width: 25%');

            columnLeftValue.setAttribute('style', 'width: 25%');
            columnRightValue.setAttribute('style', 'width: 25%');

            ///The detail field
            var detailLeftField = this.TableFields[leftIndex];
            var detailRightField = this.TableFields[rightIndex];


            columnLeftField.innerText = detailLeftField.title + ':';
            columnLeftValue.innerText = erb.attributes[detailLeftField.name] == undefined ? '-' : erb.attributes[detailLeftField.name];

            row.appendChild(columnLeftField);
            row.appendChild(columnLeftValue);

            ///Verify if the right field is undefined
            if (detailRightField != undefined) {


                columnRightField.innerText = detailRightField.title + ':';
                columnRightValue.innerText = erb.attributes[detailRightField.name] == undefined ? '-' : erb.attributes[detailRightField.name];

                row.appendChild(columnRightField);
                row.appendChild(columnRightValue);
            }

            detailTable.appendChild(row);

            rightIndex++;
        }

        return detailTable.outerHTML;
    }
}