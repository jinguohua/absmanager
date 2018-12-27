//表单控件基类
function formCtrl(title, elementId) {
    this.title = title;
    this.elementId = elementId;
    this.value = "";

    if (typeof formCtrl._initialized == "undefined") {
        formCtrl.prototype.setValue = function (val) {
            this.value = val;
        }
        formCtrl._initialized = true;
    }
}

function select(title, elementId, optionArray) {
    formCtrl.call(this, title, elementId);
    var _optionArr_;
    if (optionArray != null && optionArray.length > 0) {
        if (!(optionArray[0] instanceof Array)) {

            _optionArr_ = reSetOptions(optionArray);
        }
        else
            _optionArr_ = optionArray;
    }
    this.OptionArray = _optionArr_;
    this.type = "select";
}


function textBox(title, elementId) {
    formCtrl.call(this, title, elementId);
    this.type = "text";

    if (typeof textBox._initialized == "undefined") {
        textBox.prototype.setLimitRangeLength = function (min, max) {
            this.limit = new Limit().rangeLength(min, max);
            return this;
        }
        textBox.prototype.setLimitNumber = function (isDigit, min, max) {
            this.limit = new Limit().number(isDigit, min, max);
        }
        formCtrl._initialized = true;
    }
}

/********Limit******start****************************************/
function LimitRange(min, max) {
    this.min = min;
    this.max = max;
}

function Limit() {
    if (typeof Limit._initialized == "undefined") {
        Limit.prototype.rangeLength = function (min, max) {
            LimitRange.call(this, min, max);
            this.type = "rangelength";
            return this;
        }
        Limit.prototype.number = function (isDigit, min, max) {
            LimitRange.call(this, min, max);
            this.type = "number";
            this.isDigit = isDigit;
            return this;
        }
        Limit._initialized == true;
    }
}
/********Limit*******end***************************************/



//定义增删改实体类
function Entity(title,idKey) {
    this.controls = new Array();
    this.paramsExt = {};
    this.title = title;
    this.idKey = idKey;
    this.id = "";
    this._deleteArgs_;
    this._deleteMsg_ = "";
    this._deleteParams_ = {};
     if (typeof Entity._initialized == "undefined") {
        Entity.prototype.append = function (formCtrl) {
            this.controls.push(formCtrl);
            this[formCtrl.elementId] = formCtrl;
        }
        Entity.prototype.GetAllCtrl = function () {
            return this.controls;
        }
        Entity.prototype.paramsAddKeyVal = function (key, val) {
            this.paramsExt[key] = val;
        }

        Entity.prototype.deleteAppendKeyVal = function (key,value) {
            this._deleteParams_[key] = value;
        }

        Entity.prototype.refreshData = function (func) {
        }


        Entity.prototype.initCtrl = function () {
            for (var ctrl in this.controls) {
                this.controls[ctrl].value = "";
            }
        }

        Entity.prototype.ValidateNotNull = function (value, msg) {
            if (!cnabsHasContent(value)) {
                cnabsMsgError(msg);
            }
        }

        Entity.prototype.deleteMsgAppend = function (appendMsg) {
            this._deleteMsg_= '确认删除' + this.title + '[' + appendMsg + ']？';
        }

        Entity.prototype.setDeleteArgs=function(args){
            this._deleteArgs_= argsToParamObject(args);
         }

        Entity.prototype.bindAllArgsToCtrl = function (args) {
            var funcStr = args.callee.toString();
            var argsNameArr = funcStr.match(/\([\s|\S]+?\)/g)[0].match(/\w+/g);
            for (var i = 0; i < argsNameArr.length; i++) {
                if (this[argsNameArr[i]] != null) {
                    this[argsNameArr[i]].value = args[i];
                }
                else {
                    this.paramsAddKeyVal(argsNameArr[i], args[i]);
                }
            }
        }


        //增加
        Entity.prototype.create = function (url) {
            var paramsExt = this.paramsExt;
            var ent = this;
            ent.initCtrl();
            cnabsAutoDlgYesNo(this.controls, "创建" + ent.title, function (data) {
                var params = data;
                for (var key in paramsExt) {
                    params[key] = paramsExt[key];
                }
                cnabsAjax('创建' + ent.title, url, params, function (data) {
                    ent.refreshData();
                });
            });
        }

        //修改
        Entity.prototype.modify = function (url) {
            var paramsExt = this.paramsExt;
            var ent = this;
            cnabsAutoDlgYesNo(this.controls, "修改" + ent.title, function (data) {
                var params = data;
                for (var key in paramsExt) {
                    params[key] = paramsExt[key];
                }

                cnabsAjax('修改' + ent.title, url, params, function (data) {
                    ent.refreshData();
                });
            });
        }


        //删除
        Entity.prototype.delete = function (url) {
             var ent = this;
             var params = ent._deleteParams_;
            params[ent.idKey] = ent.id;
            var msg = ent._deleteMsg_;
            debugger;
            cnabsAutoDlgYesNo(null, '删除' + ent.title, function () {
                 cnabsAjax('删除' + ent.title, url, params, function (data) {
                    ent.refreshData();
                });
            }, msg);
        }
        Entity._initialized = true;
    }
}


function argsToParamObject(args) {
     var funcStr = args.callee.toString();
    var argsNameArr = funcStr.match(/\([\s|\S]+?\)/g)[0].match(/\w+/g);
    var paramsObj = {};
    for (var i = 0; i < argsNameArr.length; i++) {
        paramsObj[argsNameArr[i]] = args[i];
    }
    return paramsObj;
}


function reSetOptions(array) {
    var resultArray = [];
    for (var i = 0; i < array.length; i++) {
        var arr = [];
        arr.push(array[i]);
        arr.push(array[i]);
        resultArray.push(arr);
    }
    return resultArray;
}

