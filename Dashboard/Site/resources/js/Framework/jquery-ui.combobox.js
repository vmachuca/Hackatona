(function ($) {
    $.widget('custom.combobox', {

        ///implementation

        status: function (value) {

            if (value == undefined)
                return this.Status;
            else {
                this.Status = value;

                $(this.input).val('');

                if (value == 'loading') {
                    $(this.loading).attr('src', 'resources/images/ui/loading_12x12.gif');
                    $(this.loading).css('display', 'block');

                    $(this.input).autocomplete('disable');
                }
                else if (value == 'done') {
                    $(this.loading).css('display', 'none')
                }
                else if (value == 'error') {
                    $(this.loading).attr('src', 'resources/images/ui/error_12x12.png');
                    $(this.loading).css('display', 'block');

                    $(this.input).autocomplete('disable');
                    $(this.button).button('disable');
                }
                else if (value == 'disabled') {
                    $(this.input).autocomplete('disable');
                    $(this.button).button('disable');
                }
            }
        },
        updateSource: function (source) {
            $(this.input).autocomplete('option', 'source', source)

            this.status('done');

            if (source.length == 0) {
                $(this.input).autocomplete('disable');
                $(this.button).button('disable');

                $(this.loading).attr('src', 'resources/images/ui/question_12x12.gif');
                $(this.loading).css('display', 'block')
            }
            else {
                $(this.input).autocomplete('enable');
                $(this.button).button('enable');
            }
        },
        value: function (val) {
            if (val)
                $(this.input).val(val);
            else
                return $(this.input).val();
        },
        enabled: function (val) {

            $(this.input).autocomplete(val ? 'enable' : 'disable');
            $(this.button).button(val ? 'enable' : 'disable');
        },
        _create: function () {
            this.wrapper = $('<span>')
          .addClass('custom-combobox')
          .insertAfter(this.element);

            this.Status = 'done';

            this.element.hide();
            this._createAutocomplete();
            this._createShowAllButton();
            this._createLoadArea();
        },

        _createLoadArea: function () {
            this.loading = $('<img>')
              .appendTo(this.wrapper)
              .attr('src', 'resources/images/ui/loading_12x12.gif')
              .attr('style', 'position:absolute; display:none; top:22%;')
              .css('left', (($(this.input).css('width').replace('px', '') / 2) - 6) + 'px');
        },

        _createAutocomplete: function () {

            if (this.options.autocomplete['value'])
                value = this.options.autocomplete['value'];
            else {
                var selected = this.element.children(':selected'),
                value = selected.val() ? selected.text() : '';
            }

            ///Creating the autocomplete 
            var inputOptions = $.extend({
                delay: 0,
                minLength: 0,
                source: $.proxy(this, '_source')
            }, this.options.autocomplete);

            this.input = $('<input>')
          .appendTo(this.wrapper)
          .val(value)
          .attr('title', '')
          .addClass('custom-combobox-input ui-widget ui-widget-content ui-state-default ui-corner-left')
          .autocomplete(inputOptions)
          .keypress(function (event) {
              event.preventDefault();
          })
          .tooltip({
              tooltipClass: 'ui-state-highlight'
          });

            if (this.options.autocomplete)
                if (this.options.autocomplete.width)
                    this.input.css('width', this.options.autocomplete.width);

            this._on(this.input, {
                autocompleteselect: function (event, ui) {
                    ui.item['selected'] = true;
                    this._trigger('select', event, {
                        item: ui.item
                    });
                },

                autocompletechange: '_removeIfInvalid'
            });
        },

        _createShowAllButton: function () {
            var input = this.input,
          wasOpen = false;

            this.button = $('<a>')
          .attr('tabIndex', -1)
          .attr('title', 'Exibe Itens')
          .tooltip()
          .appendTo(this.wrapper)
          .button({
              icons: {
                  primary: 'ui-icon-triangle-1-s'
              },
              text: false
          })
          .removeClass('ui-corner-all')
          .addClass('custom-combobox-toggle ui-corner-right')
          .mousedown(function () {
              wasOpen = input.autocomplete('widget').is(':visible');
          })
          .click(function () {
              input.focus();

              // Close if already visible
              if (wasOpen) {
                  return;
              }

              // Pass empty string as value to search for, displaying all results
              input.autocomplete('search', '');
          });
        },

        _source: function (request, response) {
            var matcher = new RegExp($.ui.autocomplete.escapeRegex(request.term), 'i');
            response(this.element.children('option').map(function () {
                var text = $(this).text();
                if (this.value && (!request.term || matcher.test(text)))
                    return {
                        label: text,
                        value: text,
                        option: this
                    };
            }));
        },

        _removeIfInvalid: function (event, ui) {

            // Selected an item, nothing to do
            if (ui.item) {
                return;
            }

            // Search for a match (case-insensitive)
            var value = this.input.val(),
          valueLowerCase = value.toLowerCase(),
          valid = false;
            this.element.children('option').each(function () {
                if ($(this).text().toLowerCase() === valueLowerCase) {
                    this.selected = valid = true;
                    return false;
                }
            });

            // Found a match, nothing to do
            if (valid) {
                return;
            }

            // Remove invalid value
            this.input
          .val('')
          .attr('Info', value + ' não condiz a item algum')
          .tooltip('open');
            this.element.val('');
            this._delay(function () {
                this.input.tooltip('close').attr('title', '');
            }, 2500);
            this.input.data('ui-autocomplete').term = '';
        },

        _destroy: function () {
            this.wrapper.remove();
            this.element.show();
        }
    });
})(jQuery);