function LayoutWindows() {

    ///The search window
    this.search = function () {

        var html = '<div class="label claro">' +
                        '<label>UF</label><br />' +
                        '<select id="' + erbBL.ComponentSearchCombobox + '"/>' +
                        '<button id="' + erbBL.ComponentSearchButton + '" type="button"></button>' +
        //'<img src="resources/images/ui/loading_16x16.gif" />'+
                   '</div>';

        var options = {
            height: 130,
            width: 250,
            position: [66, 57]
        };

        postCreatingFunction = function () {

            try {
                dijit.byId(erbBL.ComponentSearchCombobox).destroy(true);
                dijit.byId(erbBL.ComponentSearchButton).destroy(true);
            } catch (e) { }

            ///Query hour combobox
            new dijit.form.ComboBox({
                style: 'width:40px;',
                trim: true,
                invalidMessage: "Selecione um Estado Válido",
                maxSize: "2",
                store: new dojo.data.ItemFileReadStore({ data: { items:
                [{ name: "AC" }, { name: "AL" }, { name: "AP" }, { name: "AM" }, { name: "BA" }, { name: "CE" },
                 { name: "DF" }, { name: "ES" }, { name: "GO" }, { name: "MA" }, { name: "MT" }, { name: "MS" },
                 { name: "MG" }, { name: "PA" }, { name: "PB" }, { name: "PR" }, { name: "PE" }, { name: "PI" }, 
                 { name: "RJ" }, { name: "RN" }, { name: "RS" }, { name: "RO" }, { name: "RR" }, { name: "SC" },
                 { name: "SP" }, { name: "SE" }, { name: "TO"}]
                }
                })
            }, erbBL.ComponentSearchCombobox);

            new dojox.form.BusyButton({
                busyLabel: "Consultar",
                style: 'font-size:13pt;',
                label: "Consultar",
                onClick: function () {

                    ///Gets the erbs
                    erbBL.Get($('#' + erbBL.ComponentSearchCombobox).val());
                }
            }, erbBL.ComponentSearchButton);
        };

        ///Show the window
        layoutUtils.make('searchWindow', 'Consulta', html, options, postCreatingFunction);

    }

}