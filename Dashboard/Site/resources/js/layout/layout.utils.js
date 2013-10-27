function LayoutUtils() {

    this.make = function (id, title, content, options, postCreationFunction) {

        ///Verify if the options is defined
        if (options == undefined)
            options = {};

        options['content'] = content;
        options['title'] =
            options['titleIcon'] == undefined ?
            title :
            '<img src="' + options['titleIcon'] + '" style="height: 16px"/>   ' + title + '</span>';
        options['closeConfirm'] = false;
        options['maxButton'] = false;

        position = options['position'] ? options['position'] : [0, 0];

        if (options['x'])
            position[0] = options['x'];
        if (options['y'])
            position[1] = options['y'];

        if (options['float']) {
            if (options['float'] == 'right')

                position[0] = $(window).width() - options['width'] - (position[0] + 10);
        }

        if (!options['noInitPosition'])
            options['position'] = position;

        if ($('#' + id).length == 1)
            $('#' + id).zDialog('moveToTop');
        else {
            $("<div id  ='" + id + "'>").zDialog(options);

            if (postCreationFunction != undefined)
                postCreationFunction();
        }
    }

    this.getHS = function(hsName) {


        if (hsName == undefined){
            var html = 
                '<div>'+
                    '<input type="text" id="hsName" VALUE="">'+
                    '<input type="button" value="Pegar" onclick="layoutUtils.getHS(true)"/>'+
                '</div>';

            this.make('hsWindow','Pega HS', html);
        }
        else
            alert("H: "+ $("#" + $("#hsName").val()).css('height') + "\nW: "+ $("#" + $("#hsName").val()).css('width')+
            "\n\nX: "+ ($("#" + $("#hsName").val()).offset().left - 4) + "\nY: "+ ($("#" + $("#hsName").val()).offset().top - 35));
                    
    }
}