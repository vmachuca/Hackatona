$.widget('zak.zDialog', $.ui.dialog, {

    options: {
        closeAction: 'destroy',
        closeConfirm: false,
        confirmTitle: "Close Window?",
        confirmMessage: "Are you sure you want to close this Window?",
        closeOnEscape: false,
        minButton: true,
        maxButton: false,
        minusText: 'minus',
        maxText: 'maximize',
        minMaxAnimation: 'easeOutElastic',
        minMaxAnimateTime: 1000,
        height: 'auto',
        width: 'auto',
        zIndex: '0'
    },

    _getHighestZindex: function () {

        var maxZ = Math.max.apply(null, $.map($('*'),
            function (e, n) {
                if ($(e).css('position') == 'absolute') {
                    return (parseInt($(e).css('z-index'), 10) || 1);
                }
            }));

        if (maxZ === -Infinity) {
            return 0;
        }

        return maxZ;
    },

    _makeResizable: function (handles) {
        handles = (handles === undefined ? this.options.resizable : handles);
        var that = this,
            options = this.options,
        // .ui-resizable has position: relative defined in the stylesheet
        // but dialogs have to use absolute or fixed positioning
            position = this.uiDialog.css("position"),
            resizeHandles = typeof handles === 'string' ?
                handles :
                "n,e,s,w,se,sw,ne,nw";

        function filteredUi(ui) {
            return {
                originalPosition: ui.originalPosition,
                originalSize: ui.originalSize,
                position: ui.position,
                size: ui.size
            };
        }

        this.uiDialog.resizable({
            cancel: ".ui-dialog-content",
            containment: "document",
            alsoResize: this.element,
            maxWidth: options.maxWidth,
            maxHeight: options.maxHeight,
            minWidth: options.minWidth,
            minHeight: this._minHeight(),
            handles: resizeHandles,
            start: function (event, ui) {
                $(this).addClass("ui-dialog-resizing");
                that._trigger("resizeStart", event, filteredUi(ui));
            },
            resize: function (event, ui) {
                that._trigger("resize", event, filteredUi(ui));
            },
            stop: function (event, ui) {
                $(this).removeClass("ui-dialog-resizing");
                options.height = $(this).height();
                options.width = $(this).width();
                that._trigger("resizeStop", event, filteredUi(ui));
                $.ui.dialog.overlay.resize();
                var isMaximized = $(this).hasClass("ui-dialog-maximized");
                if (isMaximized) {
                    $(this).toggleClass("ui-dialog-maximized");
                }
            }
        })
        .css("position", position)
        .find(".ui-resizable-se")
            .addClass("ui-icon ui-icon-grip-diagonal-se");
    },



    _create: function () {

        var that = this;

        //added a content option so content could be loaded using ajax or other functions
        //upon window creation
        var content = this.options.content || this.element.html();
        this.element.html(content);

        if (this.options.zIndex === 'top') {
            this.options.zIndex = this._getHighestZindex();
        }

        //apply the original create function
        this._super("_create");

        //add minimize button
        if (this.options.minButton) {
            uiDialogTitlebarMinus = $("<a href='#'></a>")
                    .addClass("ui-dialog-titlebar-close  ui-corner-all")
                    .attr({ role: "button" })
                    .css("right", "35px")
                    .mousedown(
                        function (event) {
                            event.preventDefault();
                            that.minimize(event);
                        })
                    .appendTo(this.uiDialogTitlebar);

            (this.uiDialogTitlebarMinusText = $("<span>"))
                    .addClass("ui-icon ui-icon-minusthick")
                    .attr("id", "minimizeBtn")
                    .text(this.options.minusText)
                    .appendTo(uiDialogTitlebarMinus);

            this._hoverable(uiDialogTitlebarMinus);
            this._focusable(uiDialogTitlebarMinus);

        }



        //add maximize button
        if (this.options.maxButton) {
            uiDialogTitlebarMax = $("<a href='#'></a>")
                    .addClass("ui-dialog-titlebar-close  ui-corner-all")
                    .attr({ role: "button" })
                    .css("right", "20px")
                    .mousedown(
                        function (event) {
                            event.preventDefault();
                            that.maximize(event);
                        })
                    .appendTo(this.uiDialogTitlebar);

            (this.uiDialogTitlebarMinusText = $("<span>"))
                    .addClass("ui-icon ui-icon-newwin")
                    .attr("id", "maximizeBtn")
                    .text(this.options.maxText)
                    .appendTo(uiDialogTitlebarMax);

            this._hoverable(uiDialogTitlebarMax);
            this._focusable(uiDialogTitlebarMax);
        }

        //set the close button pixels... I'm using pixels instead of em because em
        // scrunches these buttons together when the user default font is scaled down
        // and spaces them further apart when font is scaled up
        $('.ui-icon.ui-icon-closethick').parent().css('right', '5px');


    },

    close: function () {

        if (this.options.closeConfirm) {
            this._confirmPopup();
        } else {
            this._killDialog();
        }
    },

    _killDialog: function () {

        //added option to completely remove the element from the page upon closing
        if (this.options.closeAction === 'destroy') {
            this.destroy();
            this.element.remove();
        }
        else {
            //old default way of closing dialog
            this._super("close");
        }
    },

    _confirmPopup: function () {

        //confirm closing window
        var self = this;
        $('<div>').zDialog({
            title: self.options.confirmTitle,
            content: self.options.confirmMessage,
            resizable: false,
            closeConfirm: false,
            height: 240,
            modal: true,
            buttons: {
                "Close": function () {
                    $(this).zDialog("close");
                    self._killDialog();
                },
                Cancel: function () {
                    $(this).zDialog("close");
                }
            }
        });


    },

    minimize: function () {

        var self = this;

        //toggle minimize button to plus button
        self.element.parent().find('.ui-dialog-titlebar #minimizeBtn')
            .toggleClass("ui-icon-minusthick")
            .toggleClass("ui-icon-plusthick");

        var isMinimized = self.element.parent().hasClass("ui-dialog-minimized");
        var myWidth;
        var myHeight;

        if (isMinimized) {//restore 

            //if minimized from maximized state... restore to max
            var minFromMax = self.element.parent().hasClass('minFromMax');
            if (minFromMax) { self.maximize(); return 0; }

            //get custom attributes to restore width and height
            myWidth = self.element.parent().attr("myWidth");
            myHeight = self.element.parent().attr("myHeight");

            //show the dialog content
            self.element.parent().find('.ui-dialog-content').show();

            //restore width & height with animation
            self.element.parent().animate({
                "height": [myHeight, self.options.minMaxAnimation],
                "width": [myWidth, self.options.minMaxAnimation]
            }, self.options.minMaxAnimateTime,
                    function () { //callback after animation complete
                        $.ui.dialog.overlay.resize();
                    }
                );

            if (self.options.resizable) {
                self._makeResizable();
            }

            self.element.parent().toggleClass("ui-dialog-minimized");

        } else { //minimize this dialog

            myWidth = self.element.parent().width();
            myHeight = self.element.parent().height();

            //get height of titlebar so we can minimize to that height
            var minHeight = self.element.parent().find('.ui-dialog-titlebar').height();
            minHeight += parseInt(self.element.parent().find('.ui-dialog-titlebar').css('padding-top'), 10);
            minHeight += parseInt(self.element.parent().find('.ui-dialog-titlebar').css('padding-bottom'), 10);
            minHeight += 2; //for border size

            //set some custom attributes so we can restore the pre-minimized width and height
            self.element.parent().attr("myWidth", myWidth);
            self.element.parent().attr("myHeight", myHeight);

            //hide the content so it doesn't show in the margins around the titlebar when minimized
            self.element.parent().find('.ui-dialog-content').hide();

            //minimize with animation
            self.element.parent().animate({
                "height": [minHeight, self.options.minMaxAnimation],
                "width": [self.options.minWidth, self.options.minMaxAnimation]
            }, self.options.minMaxAnimateTime
                );

            //we don't want to be able to resize this when we're minimized
            if (self.options.resizable) {
                self.element.parent().resizable("destroy");
            }

            self.element.parent().toggleClass("ui-dialog-minimized");
        }

        //check if window was maximized when user clicked the min button
        var isMaximized = self.element.parent().hasClass("ui-dialog-maximized");
        if (isMaximized) {
            self.element.parent().toggleClass("ui-dialog-maximized");
            self.element.parent().addClass('minFromMax');
        }

    },

    maximize: function () {
        var self = this;

        //if window is already maximized, return 0
        var isMaximized = self.element.parent().hasClass("ui-dialog-maximized");
        if (isMaximized) {
            return 0;
        }

        self.element.parent().toggleClass("ui-dialog-maximized");

        //get width and height of window
        var myHeight = $(window).height() - 10;
        var myWidth = $(window).width() - 10;

        //change dialog options to match new size
        self.options.height = myHeight;
        self.options.width = myWidth;

        //maximize with animation
        self.element.parent().animate({
            "top": ['0px', self.options.minMaxAnimation],
            "left": ['0px', self.options.minMaxAnimation],
            "width": [myWidth, self.options.minMaxAnimation],
            "height": [myHeight, self.options.minMaxAnimation]
        }, self.options.minMaxAnimateTime,
                function () { //callback after animation complete
                    //call _size function to reset content area size
                    self._size();
                    $(this).css('width', myWidth);
                    $(this).css('height', myHeight);
                }
            );

        //makeResizable if it isn't already
        var isResizable = self.element.parent().hasClass("ui-resizable");
        if (isResizable === false) {
            self._makeResizable();
        }


        //if user maximized from a minimized state then change appropriate classes
        var isMinimized = self.element.parent().hasClass("ui-dialog-minimized");
        if (isMinimized) {
            var minFromMax = self.element.parent().hasClass('minFromMax');
            if (minFromMax) {
                self.element.parent().removeClass('minFromMax');
            } else {
                self.element.parent().find('.ui-dialog-titlebar #minimizeBtn')
                    .toggleClass("ui-icon-minusthick")
                    .toggleClass("ui-icon-plusthick");
            }
            self.element.parent().toggleClass("ui-dialog-minimized");
        }

    },

    _size: function () {
        this._super("_size");

        //set width and height to a px unit... fixes problems with auto width and height
        this.element.parent().css("width", this.element.parent().width());
        this.element.parent().css("height", this.element.parent().height());
    }

});