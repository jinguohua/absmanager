$(document).ready(function () {

    updateMenuLinkByProject(cnabsGetUrlParam('projectGuid'));

    $("#ProjectList").change(function () {
        var list = document.getElementById("ProjectList");
        var index = list.selectedIndex;
        var text = list.options[index].text;
        var projectId = list.options[index].value;
        if (text != "-请选择-") {
            $("#lbName").text(text);
        } else {
            $("#lbName").text("");
            $("#autoRemind").val("false");
            $("#remindDaily").val("false");
            $("#txtNewsKeyWord").val("");
            showSettingByAutoRemind("false");
        }

        clearTable($("#tblAccounts tbody tr:gt(0)"));
        clearTable($("#tblTasks tbody tr:gt(0)"));
        clearTable($("#tblWarn tbody tr:gt(0)"));

        $.ajax({
            url: '/Configuration/GetConfigByProjectId',
            type: 'Post',
            data: { projectId: projectId },
            dataType: 'text',
            success: function (data) {
                if (data != "Error" && data.indexOf("html") == -1) {
                    var json = eval("(" + data + ")");
                    var accounts = json.AccountList;
                    var reminders = json.ReminderList;
                    var typeList = json.AccountTypeList;
                    var dutyList = json.DutyTypeList;
                    var setting = json.RemindSettings;
                    var projectGuid = json.CurrentProjectGuid;
                    updateMenuLinkByProject(projectGuid);
                    showAccounts(accounts, typeList);
                    showWarns(reminders);

                    if (json.NewsSetting != null) {
                        $("#txtNewsKeyWord").val(json.NewsSetting.KeyWords);
                        var range = json.NewsSetting.Range;
                        if (range == null || range.length == 0)
                            $("input[name='radio'][value='negative']").prop("checked", true);
                        else
                            $("input[name='radio'][value=" + range + "]").prop("checked", true);
                    } else {
                        $("#txtNewsKeyWord").val("");
                        $("input[name='radio'][value='negative']").prop("checked", true);
                    }

                    $("#txtUserInfo").val("");

                    showRemindSetting(setting);
                } else {
                    if (text != "-请选择-") {
                        alertify.error("操作失败");
                    }
                    else {
                        updateMenuLinkByProject();
                    }
                }
            },
            error: function () { alertify.error("出错了"); }
        });
        changeAccountTable();
    });

    $("#btnNewsKeyWord").click(function () {
        var key = $("#txtNewsKeyWord").val().toString().replace(/(^\s*)|(\s*$)/g, "");
        var range = $("input[name='radio']:checked").val();
        key = stripscript(key);
        var projectId = $("#ProjectList").val();
        if (projectId == "") {
            cnabsAlert("请先选择一个产品。");
        } else {
            $.ajax({
                url: '/Configuration/NewsKeyWordsConfig',
                type: 'Post',
                data: { projectId: projectId, key: key, range: range },
                dataType: 'text',
                success: function (data) {
                    if (data == "Success") {
                        alertify.success("设置成功");
                    } else {
                        alertify.error("设置失败");
                    }
                },
                error: function () { alertify.error("出错了"); }
            });
        }
    });

    $(document).on("click", "#tblWarn tbody tr td", function (e) {
        var id = $($(this).parent().children()[1]).text();
        var tr = $(this).parent();
        var projectId = $("#ProjectList").val();
        var tr = $(this).parent();
        if ($(this).index() == 0) {
            var value = $($(this).children()[0]).text();
            if (value != 'X') {
                if (projectId == "") {
                    cnabsAlert("请先选择一个产品。");
                } else {
                    alertify.confirm("你确定要将该用户添加到此项目的提醒列表中吗？",
                        function () {
                            var trs = $("#tblWarn tbody tr:gt(0)");
                            var result = false;
                            for (var i = 0; i < trs.length; i++) {
                                if (($($(trs[i]).children()[4]).text() == $("#txtUserInfo").val())) {
                                    result = true;
                                    break;
                                }
                            }
                            if (result) {
                                cnabsAlert("该用户已存在此列表中，请勿重复添加。");
                            } else {

                                var tr = $("#tblWarn tbody tr:first").clone();
                                var tds = $(tr).children();
                                $.ajax({
                                    url: '/Configuration/AddNotifyUser',
                                    type: 'Post',
                                    data: {
                                        projectId: projectId,
                                        userId: $(tds[3]).text(),
                                        userName: $(tds[4]).text(),
                                        name: $(tds[5]).text(),
                                        company: $(tds[6]).text(),
                                        department: $(tds[7]).text(),
                                        email: $(tds[8]).text(),
                                        cellPhone: $(tds[9]).text(),
                                    },
                                    dataType: 'text',
                                    success: function (data) {
                                        if (data.indexOf("html") == -1) {
                                            $(tds[1]).text(data);
                                            $(tds[2]).text(projectId);
                                            alertify.success("添加成功");
                                        } else {
                                            alertify.error("添加失败");
                                        }
                                    },
                                    error: function () {
                                        alertify.error("出错了");
                                    }
                                });

                                tr.appendTo($("#tblWarn"));
                                changeWarnTable();
                            }
                        },
                    function () {
                        alertify.error("撤 销");
                    })
                }
            } else {
                alertify.confirm("您确定要删除该提醒用户吗？",
               function () {
                   $.ajax({
                       url: '/Configuration/DeleteReminder',
                       type: 'Post',
                       data: {
                           id: id,
                           projectId: projectId
                       },
                       dataType: 'text',
                       success: function (data) {
                           if (data == "Success") {
                               tr.fadeOut(400, function () {
                                   tr.remove();
                               });
                               alertify.success("删除成功。");
                           } else {
                               alertify.error("删除失败。");
                           }
                       },
                       error: function () {
                           alertify.error("出错了");
                       }
                   });
               },
               function () {
                   alertify.error("撤销。");
               });
            }
        }
    });

    $("#AccountTypeList").change(function () {
        var list = document.getElementById("AccountTypeList");
        var index = list.selectedIndex;
        var value = list.options[index].value;
        $("#accountTypeId").val(value);
    });

    $("#DutyTypeList").change(function () {
        var list = document.getElementById("DutyTypeList");
        var index = list.selectedIndex;
        var value = list.options[index].value;
        $("#dutyId").val(value);
    });

    $("#btnSearch").click(function () {
        var userInfo = $("#AuthedAccountList").val();
        if (userInfo == "") {
            cnabsAlert("用户搜索条件不能为空。");
        } else {
            $.ajax({
                url: '/Configuration/SearchUser',
                type: 'Post',
                data: { userInfo: userInfo },
                dataType: 'text',
                success: function (data) {
                    if (data != "Error" && data.indexOf("html") == -1) {
                        var json = eval("(" + data + ")");
                        if (json.length != 0) {
                            showSearchUser(json);
                        } else {
                            cnabsAlert("未查询到您所想找的用户，请确认用户信息后再次查询。");
                        }
                    } else {
                        alertify.error("操作失败");
                    }
                },
                error: function () { }
            });
        }
    });

    $("#btnChange").click(function () {
        changeWarnTable();
    });

    $("#btnSaveRemindSetting").click(function () {
        var projectId = $("#ProjectList").val();
        var projectId = $("#ProjectList").val();
        if (projectId == "") {
            cnabsAlert("请先选择一个产品。");
        } else {
            var isSave = "true";
            var rowId = $("#spRowId").text();
            var autoRemind = $("#autoRemind").val();
            var fre = $("#FrequencyList").val();
            fre = (fre == "" ? "-1" : fre);
            var type = $("#RemindTypeList").val();
            type = (type == "" ? "-1" : type);
            var daily = $("#remindDaily").val();
            if (autoRemind == "true") {
                if (fre == "-1" || type == "-1") {
                    alertify.error("提醒时间与方式均不能为空。");
                    isSave = "false";
                }
            }
            if (isSave == "true") {
                $.ajax({
                    url: '/Configuration/SaveRemindSettings',
                    type: 'Post',
                    data: { rowId: rowId, projectId: projectId, autoRemind: autoRemind, frequency: fre, typeId: type, remindDaily: daily },
                    dataType: 'text',
                    success: function (data) {
                        if (data != "error" && data.indexOf("html") == -1) {
                            alertify.success("设置成功");
                        } else {
                            alertify.error("设置失败");
                        }
                    },
                    error: function () { alertify.error("出错了"); }
                });
            }
        }
    });

    $("#autoRemind").change(function () {
        var value = $("#autoRemind").val();
        showSettingByAutoRemind(value);
    });

    $("#sNage").click(function () {
        $("input[name='radio'][value='negative']").prop("checked", true);
    });

    $("#sAll").click(function () {
        $("input[name='radio'][value='all']").prop("checked", true);
    });
});

function showSearchUser(json) {
    var trs = $("#tblWarn tbody tr");
    for (var i = 0; i < trs.length; i++) {
        if (i != 0) {
            $(trs[i]).hide()
        }
    }
    for (var i = 0; i < json.length; i++) {
        var tr = $("#tblWarn tbody tr:first");
        tr.show();
        var tds = tr.children();
        $($(tds[0]).children()[0]).text('+')
        $(tds[1]).text(json[i].ReminderId);
        $(tds[2]).text(json[i].ProjectId);
        $(tds[3]).text(json[i].UserId);
        $(tds[4]).text(json[i].UserName);
        $(tds[5]).text(json[i].Name);
        $(tds[6]).text(json[i].Company);
        $(tds[7]).text(json[i].Department);
        $(tds[8]).text(json[i].Email);
        $(tds[9]).text(json[i].CellPhone);
    }
}

function showAccounts(json, types) {
    var trs = $("#tblAccounts tbody tr");
    for (var i = 0; i < json.length; i++) {
        var tr = $("#tblAccounts tbody tr:first").clone();
        tr.appendTo($("#tblAccounts"));
        tr.show();
        var index = parseInt(json[i].AccountTypeId);
        var tds = tr.children();
        $(tds[1]).text(json[i].AccountId);
        $(tds[2]).text(json[i].ProjectId);
        $(tds[3]).text(json[i].AccountGuid);
        $(tds[4]).text(json[i].AccountTypeId);
        $(tds[5]).text(types[index].Text);
        $(tds[6]).text(json[i].Name);
        $(tds[7]).text(json[i].IssuerBank);
        $(tds[8]).text(json[i].BankAccount);
    }
    changeAccountTable();
}

function showTasks(json) {
    var trs = $("#tblTasks tbody tr");
    for (var i = 0; i < json.length; i++) {
        var tr = $("#tblTasks tbody tr:first").clone();
        tr.appendTo($("#tblTasks"));
        tr.show();
        var row = json[i];
        var tds = tr.children();
        $(tds[0]).text(row.TaskName);
        $(tds[1]).text(row.StartTime);
        $(tds[2]).text(row.EndTime);
        $(tds[3]).text(row.ShortCode);
        $(tds[4]).text(row.PrevTaskShortCodeArray);
        $(tds[5]).text(row.Status);
        //$(tds[6]).text(subString(row.TaskDetail, 10));
        //$(tds[7]).text(subString(row.TaskTarget, 10));
    }

    changeTaskTable();
}

function subString(str, length) {
    if (str.length > length) {
        return str.substring(0, length) + "...";
    }

    return str;
}

function changeTaskTable() {
    var trs = $("#tblTasks tbody tr:gt(0)");
    for (var i = 0; i < trs.length; i++) {
        $(trs[i]).show();
    }
}

function showWarns(json) {
    var trs = $("#tblWarn tbody tr");
    for (var i = 0; i < json.length; i++) {
        var tr = $("#tblWarn tbody tr:first").clone();
        tr.appendTo($("#tblWarn"));
        tr.show();
        var tds = tr.children();
        $(tds[1]).text(json[i].ReminderId);
        $(tds[2]).text(json[i].ProjectId);
        $(tds[3]).text(json[i].UserId);
        $(tds[4]).text(json[i].UserName);
        $(tds[5]).text(json[i].Name);
        $(tds[6]).text(json[i].Company);
        $(tds[7]).text(json[i].Department);
        $(tds[8]).text(json[i].Email);
        $(tds[9]).text(json[i].CellPhone);
    }
    changeWarnTable();
}

function showSettings(json) {
    $("#autoRemind").val(json.AutoRemind);
    $("#FrequencyList").val(json.Frequency);
    $("#RemindTypeList").val(json.RemindType);
    $("#remindDaily").val(json.RemindDaily);
    showSettingByAutoRemind(json.AutoRemind);
}

function showRemindSetting(json) {
    if (json == null) {
        $("#spRowId").text("-1");
        $("#FrequencyList").val("24");
        $("#RemindTypeList").val("3");
        $("#remindDaily").val("false");
        $("#autoRemind").val("false");
    } else {
        $("#spRowId").text(json.RowId);
        $("#autoRemind").val(json.AutoRemind.toString());
        $("#FrequencyList").val(json.Frequency);
        $("#RemindTypeList").val(json.RemindType);
        $("#remindDaily").val(json.RemindDaily.toString());
    }
    showSettingByAutoRemind(json.AutoRemind);
}

function changeWarnTable() {
    var trs = $("#tblWarn tbody tr");
    for (var i = 0; i < trs.length; i++) {
        if (i == 0) {
            $(trs[i]).hide()
        } else {
            $(trs[i]).show()
            $($($(trs[i]).children()[0]).children()[0]).text('X')
        }
    }
}

function showSettingByAutoRemind(value) {
    if (value.toString() == "true") {
        $("#spFre").show();
        $("#spRem").show();
        $("#FrequencyList").show();
        $("#RemindTypeList").show();
    } else {
        $("#spFre").hide();
        $("#spRem").hide();
        $("#FrequencyList").hide();
        $("#RemindTypeList").hide();
    }
}

function stripscript(s) {
    var pattern = new RegExp("[`~!@#$^&*()=|{}':;'\\[\\].<>/?~！@#￥……&*（）——|{}【】‘；：”“'。，、？]")
    var rs = "";
    for (var i = 0; i < s.length; i++) {
        rs = rs + s.substr(i, 1).replace(pattern, ',');
    }
    return rs;
}

function changeAccountTable() {
    var trs = $("#tblAccounts tbody tr:gt(0)");
    for (var i = 0; i < trs.length; i++) {
        $(trs[i]).show();
    }
}

function checkData(container, inputs) {
    var elem;
    var pass = true;
    for (var i = 0; i < inputs.length; i++) {
        elem = container.find("#" + inputs[i]);
        if (!elem.size() || !$.trim(elem.val())) {
            elem.addClass("modeling-field-error");
            pass = false;
        }
        else {
            elem.removeClass("modeling-field-error");
        }
    }
    return pass;
}

function clearTable(trs) {
    for (var i = 0; i < trs.length; i++) {
        $(trs[i]).remove();
    }
}