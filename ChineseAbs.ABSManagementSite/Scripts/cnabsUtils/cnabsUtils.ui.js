
function cnabsParseUIChainContent(content) {
    var array = [];
    var iBegin = content.indexOf('{');
    while (iBegin != -1) {
        var element = {};
        element.isKey = false;
        element.text = content.substr(0, iBegin);
        if (element.text.length > 0) {
            array.push(element);
        }

        content = content.substr(iBegin, content.length - iBegin);

        var iEnd = content.indexOf('}');
        if (iEnd != -1) {
            var element = {};
            element.isKey = true;
            element.text = content.substr(1, iEnd - 1);

            array.push(element);
            content = content.substr(iEnd + 1, content.length - iEnd - 1);
        } else {
            break;
        }

        iBegin = content.indexOf('{');
    }

    return array;
};

function cnabsConvertUIChainElement2Html(prefix, element, params) {
    if (element.isKey) {
        var html = '';

        var dateType = {
            type: 'select',
            style: 'formElement ' + prefix + 'ConditionUnitType',
            value: params[0],
            option: [
                { key: 'TradingDay', value: '交易日' },
                { key: 'Day', value: '自然日' },
                { key: 'WorkingDay', value: '工作日' }
            ]
        };

        switch (element.text) {
            case 'DateDirection':
                var dateDirection = {
                    type: 'select',
                    style: 'formElement ' + prefix + 'TimeMoveDirection',
                    value: params[0],
                    option: [
                        { key: 'Plus', value: '未来' },
                        { key: 'Minus', value: '过去' }
                    ]
                };
                html = cnabsGenerateUIChainSelect(dateDirection);
                break;
            case 'DateCount':
                var dateCount = {
                    type: 'text',
                    style: 'formElement ' + prefix + 'Interval',
                    value: params[0],
                };
                html = cnabsGenerateUIChainInput(dateCount);
                break;
            case 'DateType':
                html = cnabsGenerateUIChainSelect(dateType);
                break;
            case 'DateType-BindA':
                dateType.onchange = '"bindSameValue(' + 1 + ')"';
                html = cnabsGenerateUIChainSelect(dateType);
                break;
            case 'DateType-BindB':
                dateType.style = 'formElement ' + prefix + 'SecondConditionUnitType',
                dateType.onchange = '"bindSameValue(' + 2 + ')"';
                html = cnabsGenerateUIChainSelect(dateType);
                break;
            case 'PeriodType':
                var periodType = {
                    type: 'select',
                    style: 'formElement ' + prefix + 'PeriodType',
                    value: params[0],
                    option: [
                        { key: 'Month', value: '月' },
                        { key: 'Week', value: '周' },
                        { key: 'Year', value: '年' }
                    ]
                };
                html = cnabsGenerateUIChainSelect(periodType);
                break;
        }

        params.splice(0, 1);
        return html;
    } else {
        return cnabsGenerateUIChainLabel(element.text);
    }
};

function cnabsGenerateUIChain(prefix, content, params) {
    var chainElems = cnabsParseUIChainContent(content);
    var html = '';
    $.each(chainElems, function () {
        html += cnabsConvertUIChainElement2Html(prefix, this, params);
    });

    return html;
}

function cnabsGenerateUIChainSelect(obj) {
    var html = '<select class="' + obj.style + '" value="' + obj.value + '"';
    if (obj.onchange != undefined) {
        html += ' onchange=' + obj.onchange;
    }
    html += '>';

    $.each(obj.option, function () {
        html += '<option value="' + this.key + '">' + this.value + '</option>';
    });
    html += '</select>';
    return html;
};

function cnabsGenerateUIChainInput (obj) {
    return '<input class="' + obj.style + '" type="' + obj.type + '" value="' + obj.value + '">';
};

function cnabsGenerateUIChainLabel(obj) {
    return ' ' + obj + ' ';
}
