/**
* @preserve Copyright (c) 2012-2013 Attila Losonc
* Dual-licensed under the Microsoft Public License and MIT License: https://bitbucket.org/attilax/jqueryuihelpers/raw/58df63b2c5854996f5950f4b243c4c0892c073cf/license.txt
*/

(function ($) {
    $(function () {
        var elements = $('[data-jqui-type]');
        initJQueryUI(elements);

        if ($['validator'] && $['validator']['methods']) {
            var originalDate = $['validator']['methods']['date'];
            $['validator']['methods']['date'] = function (value, element) {
                var format = $(element).data('jqui-dpicker-dateformat');
                if (format) {
                    var date = null;
                    try {
                        date = $.datepicker.parseDate(format, value);
                    }
                    catch (e) { }
                    return this['optional'](element) || date != null;
                }
                return originalDate.call(this, value, element);
            }
            if (typeof Globalize !== "undefined") {
                var originalNumber = $['validator']['methods']['number'];
                $['validator']['methods']['number'] = function (value, element) {
                    var culture = $(element).data('jqui-spinner-culture');
                    if (culture) {
                        var number = Globalize['parseFloat'](value, 10, culture);
                        return this['optional'](element) || !isNaN(number);
                    }
                    return originalNumber.call(this, value, element);
                }
            }
        }
    });

    var utils = {};

    $['fn']['jQueryUIHelpers'] = function () {
        var elements = this.find('[data-jqui-type]').andSelf();
        initJQueryUI(elements);
        return this;
    };

    function initJQueryUI(elements) {
        var dpc = elements.filter('[data-jqui-type="datepicker-culture"]').first();
        if (dpc.length) {
            $['datepicker']['setDefaults']($['datepicker']['regional'][dpc.val()]);
        }
        elements.filter('[data-jqui-type="dialog"]').each(function () {
            var dl = $(this);
            var options = createOptions(dl, 'data-jqui-dialog-',
        ['appendTo', 'autoOpen', 'closeOnEscape', 'closeText', 'dialogClass', 'draggable', 'height', 'maxHeight',
        'maxWidth', 'minHeight', 'minWidth', 'modal', 'resizable', 'title', 'width'], [],
        ['beforeClose', 'close', 'create', 'drag', 'dragStart', 'dragStop', 'focus', 'open', 'resize', 'resizeStart', 'resizeStop'],
        ['position'], []);
            setupDialogUniqueAttributes(dl, options);
            dl.dialog(options);
            setupDialogBindings(dl);
        });
        elements.filter('[data-jqui-type="slider"]').each(function () {
            var sl = $(this);
            var options = createOptions(sl, 'data-jqui-slider-',
        ['animate', 'disabled', 'max', 'min', 'orientation', 'range', 'step', 'value'], ['values'],
        ['create', 'start', 'slide', 'change', 'stop']);
            setupSliderUniqueAttributes(sl, options);
            sl.slider(options);
        });
        elements.filter('[data-jqui-type="button"]').each(function () {
            var bt = $(this);
            var options = createOptions(bt, 'data-jqui-button-', ['disabled', 'text', 'label'], [], ['create'], ['icons']);
            bt.button(options);
        });
        elements.filter('[data-jqui-type="buttonset"]').each(function () {
            var bs = $(this);
            bs.buttonset();
        });
        elements.filter('[data-jqui-type="accordion"]').each(function () {
            var ac = $(this);
            var options = createOptions(ac, 'data-jqui-acc-', ['disabled', 'active', 'collapsible', 'event', 'header', 'heightStyle'],
        [], ['create', 'activate', 'beforeActivate'], ['animate', 'icons']);
            ac.accordion(options);
        });
        elements.filter('[data-jqui-type="tabs"]').each(function () {
            var ta = $(this);
            var options = createOptions(ta, 'data-jqui-tabs-',
        ['active', 'collapsible', 'deselectable', 'event', 'heightStyle'], [],
        ['create', 'load', 'beforeActivate', 'activate', 'beforeLoad'], ['hide', 'show'], ['disabled']);
            resolveFunctions(options['ajaxOptions'], ['beforeSend', 'complete', 'dataFilter', 'error', 'success', 'xhr']);
            ta.tabs(options);
        });
        elements.filter('[data-jqui-type="menu"]').each(function () {
            var mn = $(this);
            var options = createOptions(mn, 'data-jqui-menu-',
            ['disabled', 'role'], [], ['blur', 'create', 'focus', 'select'], ['icons', 'position']);
            mn.menu(options);
        });
        elements.filter('[data-jqui-type="progressbar"]').each(function () {
            var pb = $(this);
            var options = createOptions(pb, 'data-jqui-pbar-',
            ['disabled', 'max', 'value'], [],
            ['change', 'complete', 'create']);
            pb.progressbar(options);
        });
        elements.filter('[data-jqui-type="datepicker"]').each(function () {
            var dp = $(this);
            var options = createOptions(dp, 'data-jqui-dpicker-',
            ['altField', 'altFormat', 'appendText', 'autoSize', 'buttonImage', 'buttonImageOnly', 'buttonText',
            'changeMonth', 'changeYear', 'closeText', 'constrainInput', 'currentText', 'dateFormat', 'defaultDate', 'disabled',
            'duration', 'firstDay', 'gotoCurrent', 'hideIfNoPrevNext', 'isRTL', 'maxDate', 'minDate', 'navigationAsDateFormat',
            'nextText', 'prevText', 'selectOtherMonths', 'shortYearCutoff', 'showAnim', 'showButtonPanel', 'showCurrentAtPos',
            'showMonthAfterYear', 'showOn', 'showOptions', 'showOtherMonths', 'showWeek', 'stepMonths', 'weekHeader',
            'yearRange', 'yearSuffix'],
            ['dayNames', 'dayNamesMin', 'dayNamesShort', 'monthNames', 'monthNamesShort'],
            ['calculateWeek', 'beforeShow', 'beforeShowDay', 'onChangeMonthYear', 'onClose', 'onSelect'], [],
            ['numberOfMonths']);
            setupDatepickerUniqueAttributes(dp, options);
            dp.datepicker(options);
            initDatepicker(dp);
        });
        elements.filter('[data-jqui-type="autocomplete"]').each(function () {
            var ac = $(this);
            var options = createOptions(ac, 'data-jqui-acomp-',
            ['appendTo', 'autoFocus', 'delay', 'disabled', 'minLength'], [],
            ['create', 'search', 'open', 'focus', 'select', 'close', 'change', 'response'], ['position']);
            setupAutocompleteUniqueAttributes(ac, options);
            ac.autocomplete(options);
        });
        elements.filter('[data-jqui-type="spinner"]').each(function () {
            var sp = $(this);
            var options = createOptions(sp, 'data-jqui-spinner-',
            ['culture', 'disabled', 'incremental', 'max', 'min','numberFormat', 'page', 'step'], [],
            ['create', 'change', 'start', 'spin', 'stop'], ['icons']);
            sp.spinner(options);
        });
        elements.filter('[data-jqui-type="sortable"]').each(function () {
            var so = $(this);
            var options = createOptions(so, 'data-jqui-sort-',
            ['appendTo', 'axis', 'cancel', 'connectWith', 'containment', 'cursor', 'delay', 'disabled', 'distance',
            'dropOnEmpty', 'forceHelperSize', 'forcePlaceholderSize', 'handle', 'items', 'opacity', 'placeholder',
            'revert', 'scroll', 'scrollSensitivity', 'scrollSpeed', 'tolerance', 'zIndex'], ['grid'],
            ['activate', 'beforeStop', 'change', 'create', 'deactivate', 'out', 'over', 'receive', 'remove', 'sort',
            'start', 'stop', 'update'],
            ['cursorAt']);
            setupSortableUniqueAttributes(so, options);
            so.sortable(options);
        });
        elements.filter('[data-jqui-type="resizable"]').each(function () {
            var rs = $(this);
            var options = createOptions(rs, 'data-jqui-resiz-',
            ['alsoResize', 'animate', 'animateDuration', 'animateEasing', 'aspectRatio', 'autoHide', 'cancel', 'containment',
            'delay', 'disabled', 'distance', 'ghost', 'helper', 'maxHeight', 'maxWidth', 'minHeight', 'minWidth', 'handles'],
            ['grid'], ['create', 'resize', 'start', 'stop'], []);
            rs.resizable(options);
        });
        elements.filter('[data-jqui-type="draggable"]').each(function () {
            var dg = $(this);
            var options = createOptions(dg, 'data-jqui-drag-',
            ['addClasses', 'appendTo', 'axis', 'cancel', 'connectToSortable', 'cursor', 'delay', 'disabled', 'distance',
            'handle', 'iframeFix', 'opacity', 'refreshPositions', 'revert', 'revertDuration', 'scope', 'scroll',
            'scrollSensitivity', 'scrollSpeed', 'snap', 'snapMode', 'snapTolerance', 'stack', 'zIndex'],
            ['grid'], ['create', 'drag', 'start', 'stop'], ['cursorAt'], ['containment']);
            setupDraggableUniqueAttributes(dg, options);
            dg.draggable(options);
        });
        elements.filter('[data-jqui-type="droppable"]').each(function () {
            var dp = $(this);
            var options = createOptions(dp, 'data-jqui-drop-',
            ['activeClass', 'addClasses', 'disabled', 'greedy', 'hoverClass', 'scope', 'tolerance'],
            [], ['activate', 'create', 'deactivate', 'drop', 'out', 'over']);
            setupDroppableUniqueAttributes(dp, options);
            dp.droppable(options);
        });
        elements.filter('[data-jqui-type="selectable"]').each(function () {
            var se = $(this);
            var options = createOptions(se, 'data-jqui-select-',
            ['autoRefresh', 'cancel', 'delay', 'disabled', 'distance', 'filter', 'tolerance'],
            [], ['create', 'selected', 'selecting', 'start', 'stop', 'unselected', 'unselecting']);            
            se.selectable(options);
        });
        elements.filter('[data-jqui-type="tooltip"]').each(function () {
            var tt = $(this);
            var options = createOptions(tt, 'data-jqui-ttip-',
            ['disabled', 'items', 'tooltipClass', 'track'], [], ['create', 'close', 'open'], ['hide', 'position', 'show']);
            setupTooltipUniqueAttributes(tt, options);
            var selector = $(this).attr('data-jqui-ttip-selector');
            if (selector) {
                $(selector).tooltip(options);
            }
            else {
                $(document).tooltip(options);
            }
        });
        if ($['fn']['jQueryUIHelpers']['actions']) {
            for (var i = 0; i < $['fn']['jQueryUIHelpers']['actions'].length; i++) {
                elements.filter($['fn']['jQueryUIHelpers']['actions'][i]['filter']).each(function () {
                    $['fn']['jQueryUIHelpers']['actions'][i]['action']($(this), utils);
                });
            }
        }
    };

    function createOptions(element, prefix, propertyNames, arrayPropertyNames, functionPropertyNames, objectPropertyNames, variablePropertyNames) {
        var options = {};
        var attributeName, value, parts, arrayValue, i, j;
        for (i = 0; i < propertyNames.length; i++) {
            attributeName = prefix + propertyNames[i].toLowerCase();
            value = element.attr(attributeName);
            if (value !== undefined) {
                options[propertyNames[i]] = tryConvert(value);
            }
        }
        if (arrayPropertyNames) {
            for (i = 0; i < arrayPropertyNames.length; i++) {
                attributeName = prefix + arrayPropertyNames[i].toLowerCase();
                value = element.attr(attributeName);
                if (value !== undefined) {
                    parts = value.split(',');
                    arrayValue = [];
                    for (j = 0; j < parts.length; j++) {
                        arrayValue.push(tryConvert(parts[j]));
                    }
                    options[arrayPropertyNames[i]] = arrayValue;
                }
            }
        }
        if (variablePropertyNames) {
            for (i = 0; i < variablePropertyNames.length; i++) {
                attributeName = prefix + variablePropertyNames[i].toLowerCase();
                value = element.attr(attributeName);
                if (value !== undefined) {
                    parts = value.split(',');
                    if (parts.length === 1) {
                        options[variablePropertyNames[i]] = tryConvert(value);
                    }
                    else {
                        arrayValue = [];
                        for (j = 0; j < parts.length; j++) {
                            arrayValue.push(tryConvert(parts[j]));
                        }
                        options[variablePropertyNames[i]] = arrayValue;
                    }
                }
            }
        }
        if (functionPropertyNames) {
            for (i = 0; i < functionPropertyNames.length; i++) {
                attributeName = prefix + functionPropertyNames[i].toLowerCase();
                value = element.attr(attributeName);
                if (value !== undefined) {
                    options[functionPropertyNames[i]] = findObject(value);
                }
            }
        }
        if (objectPropertyNames) {
            for (i = 0; i < objectPropertyNames.length; i++) {
                attributeName = prefix + objectPropertyNames[i].toLowerCase();
                value = element.attr(attributeName);
                if (value !== undefined) {
                    if (value.substring(0, 1) === '{') {
                        options[objectPropertyNames[i]] = JSON.parse(value);
                    }
                    else {
                        options[objectPropertyNames[i]] = tryConvert(value);
                    }
                }
            }
        }
        return options;
    }


    function tryConvert(value) {
        if (value.toLowerCase() === 'false') {
            return false;
        }
        if (value.toLowerCase() === 'true') {
            return true;
        }
        if (value === '(null)') {
            return null;
        }
        if (/^-?\d+$/.test(value)) {
            return parseInt(value, 10);
        }
        return stringValue(value);
    }

    function stringValue(value) {
        if (value.indexOf('<script') >= 0) {
            return $('<div/>').text(value).html();
        }
        return value;
    }

    function findObject(fullName) {
        var parts = fullName.split('.');
        var func = window[parts[0]];
        for (var i = 1; i < parts.length; i++) {
            if (func) {
                func = func[parts[i]];
            }
        }
        return func;
    }

    function resolveFunctions(obj, functionPropertyNames) {
        if (!obj) return;
        var propertyName;
        for (var i = 0; i < functionPropertyNames.length; i++) {
            propertyName = functionPropertyNames[i];
            if (obj[propertyName]) {
                obj[propertyName] = findObject(obj[propertyName]);
            }
        }
    }

    function setFunctionOrValue(attributeName, optionName, element, options) {
        var value = element.attr(attributeName);
        if (value) {
            var func = findObject(value);
            if (func && typeof (func) === 'function') {
                options[optionName] = func;
            }
            else {
                options[optionName] = stringValue(value);
            }
        }
    }

    utils['createOptions'] = createOptions;
    utils['tryConvert'] = tryConvert;
    utils['findObject'] = findObject;
    utils['resolveFunctions'] = resolveFunctions;
    utils['setFunctionOrValue'] = setFunctionOrValue;

    function setupDatepickerUniqueAttributes(element, options) {
        var valueSelector = element.attr('data-jqui-dpicker-hiddenvalue');
        if (valueSelector) {
            var validator = element.closest('form').data('validator');
            if (validator) {
                validator['settings']['ignore'] = '';
            }
            var valueElement = $('#' + valueSelector);
            var userOnSelect = options['onSelect'];
            options['onSelect'] = function (dateText, inst) {
                if (userOnSelect) {
                    var result = userOnSelect.call(this, dateText, inst);
                    if (result === false) {
                        return false;
                    }
                }
                valueElement.val(dateText);
                return false;
            };
        }
    }

    function initDatepicker(element) {
        var valueSelector = element.attr('data-jqui-dpicker-hiddenvalue');
        if (valueSelector) {
            var valueElement = $('#' + valueSelector);
            if (valueElement.val()) {
                element.datepicker('setDate', valueElement.val());
            }
        }
    }

    function setupAutocompleteUniqueAttributes(element, options) {
        var source = element.attr('data-jqui-acomp-source');
        if (source) {
            // if source does not contain '/', treat it as an object
            if (source.indexOf('/') < 0) {
                source = findObject(source);
            }
            options['source'] = source;
        }
        var valueSelector = element.attr('data-jqui-acomp-hiddenvalue');
        if (valueSelector) {
            var validator = element.closest('form').data('validator');
            if (validator) {
                validator['settings']['ignore'] = '';
            }
            var valueElement = $('#' + valueSelector);
            var userFocus = options['focus'];
            options['focus'] = function (event, ui) {
                if (userFocus) {
                    userFocus.call(this, event, ui);
                }
                return false; // do not put the value into the input field
            };
            var userChange = options['change'];
            options['change'] = function (event, ui) {
                if (userChange) {
                    var result = userChange.call(this, event, ui);
                    if (result === false) {
                        return false;
                    }
                }
                if (!ui['item']) {
                    valueElement.val('');
                }
            };
            var userSelect = options['select'];
            options['select'] = function (event, ui) {
                if (userSelect) {
                    var result = userSelect.call(this, event, ui);
                    if (result === false) {
                        return false;
                    }
                }
                if (ui['item']['value']) {
                    element.val(ui['item']['label']);
                    valueElement.val(ui['item']['value']);
                }
                return false;
            };
            element.blur(function () {
                if (!element.val()) {
                    valueElement.val('');
                }
            });
        }
    }

    function setupSliderUniqueAttributes(element, options) {
        var names = element.attr('data-jqui-slider-names');
        if (names) {
            var userSlide = options['slide'],
                userChange = options['change'];
            options['slide'] = createSliderUpdateFunction(names, userSlide);
            options['change'] = createSliderUpdateFunction(names, userChange);
        }
    }

    function createSliderUpdateFunction(names, userEventHandler) {
        return function (event, ui) {
            if (userEventHandler) {
                var result = userEventHandler.call(this, event, ui);
                if (result === false) {
                    return false;
                }
            }
            var ids = names.split(',');
            if (ui['values']) {
                var value = '';
                var separator = ui['values'].length > 2 ? ', ' : '-';
                for (var i = 0; i < ui['values'].length; i++) {
                    value += ui['values'][i];
                    if (i !== ui['values'].length - 1) {
                        value += separator;
                    }
                    $('#' + ids[i]).val(ui['values'][i]);
                }
                $(this).prev().html(value);
            }
            else {
                $(this).prev().html(ui['value']);
                $('#' + ids[0]).val(ui['value']);
            }
        };
    }

    function setupDialogUniqueAttributes(element, options) {
        var acceptText, cancelText;
        var buttons = element.attr('data-jqui-dialog-buttons');
        var buttonArray = [];
        if (buttons) {
            var parts = buttons.split('|');
            for (var i = 0; i < parts.length; i++) {
                buttonArray[i] = JSON.parse(parts[i]);
                buttonArray[i]['click'] = findObject(buttonArray[i]['click']);
            }
            options['buttons'] = buttonArray;
        }
        if (element.attr('data-jqui-dialog-confirm')) {
            acceptText = element.attr('data-jqui-dialog-confirm-accept');
            cancelText = element.attr('data-jqui-dialog-confirm-cancel');
            buttonArray.push({ text: acceptText, click: function () {
                element.dialog('close');
                window.location = element.data('href');
            }
            });
            buttonArray.push({ text: cancelText, click: function () {
                element.dialog('close');
            }
            });
            options['buttons'] = buttonArray;
        }
        if (element.attr('data-jqui-dialog-confirmajax')) {
            acceptText = element.attr('data-jqui-dialog-confirm-accept');
            cancelText = element.attr('data-jqui-dialog-confirm-cancel');
            buttonArray.push({ text: acceptText, click: function () {
                element.dialog('close');
                var settings = element.data('ajaxSettings');
                $.ajax(element.data('href'), settings);
            }
            });
            buttonArray.push({ text: cancelText, click: function () {
                element.dialog('close');
            }
            });
            options['buttons'] = buttonArray;
        }
        var hide = element.attr('data-jqui-dialog-hide');
        if (hide) {
            if (hide.substring(0, 1) === '{') {
                options['hide'] = JSON.parse(hide);
            }
            else {
                options['hide'] = tryConvert(hide);
            }
        }
        var show = element.attr('data-jqui-dialog-show');
        if (show) {
            if (show.substring(0, 1) === '{') {
                options['show'] = JSON.parse(show);
            }
            else {
                options['show'] = tryConvert(show);
            }
        }
    }

    function setupDialogBindings(element) {
        attrValue = element.attr('data-jqui-dialog-triggerclick');
        var triggerElement;
        if (attrValue) {
            triggerElement = $(attrValue);
            if (triggerElement.length) {
                triggerElement.on('click.jqueryuihelpers', function (e) {
                    e.preventDefault();
                    if (!element.dialog('isOpen')) {
                        element.dialog('open');
                    }
                });
            }
        }
        attrValue = element.attr('data-jqui-dialog-triggerhover');
        if (attrValue) {
            triggerElement = $(attrValue);
            if (triggerElement.length) {
                triggerElement.on('mouseenter.jqueryuihelpers', function (e) {
                    e.preventDefault();
                    if (!element.dialog('isOpen')) {
                        element.dialog('open');
                    }
                });
                triggerElement.on('mouseleave.jqueryuihelpers', function (e) {
                    e.preventDefault();
                    if (element.dialog('isOpen')) {
                        element.dialog('close');
                    }
                });
            }
        }
        attrValue = element.attr('data-jqui-dialog-confirm');
        if (attrValue) {
            triggerElement = $(attrValue);
            if (triggerElement.length) {
                triggerElement.on('click.jqueryuihelpers', function (e) {
                    e.preventDefault();
                    element.data('href', e.target['href']);
                    if (!element.dialog('isOpen')) {
                        element.dialog('open');
                    }
                });
            }
        }
        attrValue = element.attr('data-jqui-dialog-confirmajax');
        if (attrValue) {
            var ajaxSettings = JSON.parse(element.attr('data-jqui-dialog-confirm-ajaxsettings'));
            resolveFunctions(ajaxSettings, ['beforeSend', 'complete', 'dataFilter', 'error', 'success', 'xhr']);
            var verificationToken = $('input[name="__RequestVerificationToken"]').val();
            if (verificationToken) {
                ajaxSettings['data'] = ajaxSettings['data'] || {};
                ajaxSettings['data']['__RequestVerificationToken'] = verificationToken;
            }
            element.data('ajaxSettings', ajaxSettings);
            $(document).on('click.jqueryuihelpers', attrValue, function (e) {
                e.preventDefault();
                element.data('href', e.target['href']);
                if (!element.dialog('isOpen')) {
                    element.dialog('open');
                }
            });
        }
    }

    function setupTooltipUniqueAttributes(element, options) {
        setFunctionOrValue('data-jqui-ttip-content', 'content', element, options);
    }

    function setupDroppableUniqueAttributes(element, options) {
        setFunctionOrValue('data-jqui-drop-accept', 'accept', element, options);
    }

    function setHelper(attributeName, element, options) {
        var helper = element.attr(attributeName);
        if (helper) {
            if (helper !== 'original' && helper !== 'clone') {
                helper = findObject(helper);
            }
            options['helper'] = helper;
        }
    }

    function setupSortableUniqueAttributes(element, options) {
        setHelper('data-jqui-sort-helper', element, options);
    }

    function setupDraggableUniqueAttributes(element, options) {
        setHelper('data-jqui-drag-helper', element, options);
    }

} (jQuery));