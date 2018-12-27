function drawNoteStructure() {
    var origPoolbalance = $("#CreditEnhancement_OriginalAPB").val();
    var obj = document.getElementById("noteStructure");
    obj.innerHTML = "";
    var width = obj.scrollWidth;
    var height = obj.scrollHeight;
    var notes = $(".notes>.note-full");
    var count = notes.size();
    if (count > 0) {
        var Data = new Array();
        var note, ordinal, dotPos;
        for (var i = 0; i < count; i++) {
            note = notes.eq(i);
            ordinal = note.find(".note-full-PaymentOrdinal").val();
            dotPos = ordinal.indexOf(".");
            Data.push("{'seniority':'" + (dotPos > 0 ? ordinal.substring(0, dotPos) : ordinal)
             + "','name':'" + note.find(".note-full-Name").val()
             + "','notional':'" + note.find(".note-full-Notional").val() + "'}");
        }
        var jsonData = "[" + Data.join(",") + "]";
        $.ajax({
            url: "/Chart/CreateNoteStructureTable",
            type: "POST",
            data: {
                origPool: origPoolbalance,
                table: jsonData,
                width: width,
                height: height
            },
            dataType: "html",
            error: function () { },
            success: function (data) {
                if (data) {
                    $('#noteStructure').html(data);
                    if (data == "证券金额超出资产池本金") {
                        //alertify.alert("证券金额超出资产池本金!")
                    }
                    return;
                }
            }
        });
    }
    var origPool = "100%";
    $('#noteStructure').html("<table style='width:100%;height:100%;'><tr><td>资产池剩余本金 <br/>"
         + "<span class='note-structure-table-balance'>"
         + origPool + "</span></td></tr></table>");
}
function showCalculationDialog() {
    var sfound = false, mfound = false;
    $(".note-data").each(function () {
        if (sfound && mfound)
            return false;

        var self = $(this);
        if (!sfound && self.find(".note-payment-ordinal").val() < 2
            && self.find(".note-coupon").val().indexOf("%") < 0) {
            $("#txtSeniorRate").val(parsePercent(parseFloat(self.find(".note-coupon").val())));
            sfound = true;
        }
    });
    $("#divCalculationSetting").dialog("open");
}

function addCommas(nStr) {
    nStr += '';
    x = nStr.split('.');
    x1 = x[0];
    x2 = x.length > 1 ? '.' + x[1] : '';
    var rgx = /(\d+)(\d{3})/;
    while (rgx.test(x1)) {
        x1 = x1.replace(rgx, '$1' + ',' + '$2');
    }
    return x1 + x2;
}
function parsePercent(fStr, mode) {
    if (fStr.length == 0 || isNaN(fStr)) {
        return "";
    }
    var f = parseFloat(fStr);
    if (mode == "bps")
        return Math.round((f / 100) * 100) / 100;
    else
        return Math.round((parseFloat(f * 100) * 100)) / 100;
}
function round2(fStr) {
    if (fStr == null || fStr.length == 0 || isNaN(fStr)) {
        return "";
    }
    var f = parseFloat(fStr);
    return Math.round(f * 100) / 100;
}
function round4(fStr) {
    if (fStr == null || fStr.length == 0 || isNaN(fStr)) {
        return "";
    }
    var f = parseFloat(fStr);
    return Math.round(f * 10000) / 10000;
}
function convertIfHasPercent(tb) {
    var value = tb.val();
    if (value.indexOf("%") > -1) {
        value = value.replace("%", "").trim();
        var cv = parseFloat(value);
        if (cv) {
            tb.val(Math.formatFloat(value / 100));
        }
        else {
            alertify.message("请输入正确的数字");
        }
    }
}
function toPercent(f, n) {
    n = n || 2;
    return (Math.round(f * Math.pow(10, n + 2)) / Math.pow(10, n)).toFixed(n) + '%';
}
function validateNumber(nStr) {
    return nStr.match(/^-?\d+\.?\d*%?$/);
}
Math.formatFloat = function (f) {
    var m = Math.pow(10, 5);
    return parseInt(f * m, 10) / m;
}
String.prototype.endWith = function (endStr) {
    var d = this.length - endStr.length;
    return (d >= 0 && this.lastIndexOf(endStr) == d)
}
function convertArrayToObject(arrName, array) {
    var obj = {};
    for (var i in array) {
        for (var j in array[i]) {
            obj[arrName + "[" + i + "]." + j] = array[i][j];
        }
    }
    return obj;
}

/* change visibility*/
function sChangeVisibility(rule) {
    if (rule) {
        if (rule == "Simple") {
            $(".simpleDeterminationRule").fadeIn();
            $(".complexDeterminationRule").hide();
        } else {
            $(".simpleDeterminationRule").hide();
            $(".complexDeterminationRule").fadeIn();
        }
    } else {
        $(".simpleDeterminationRule").hide();
        $(".complexDeterminationRule").hide();
    }
}
function fChangeVisibility(select, type) {
    var table = select.parent("td").parent("tr").parent("tbody").parent("table");
    if (type == "IsProRated") {
        var simple = table.find(".simpleFee");
        var prorated = table.find(".proratedFee");
        var btnConfirm = table.parent("td").parent("tr").prev().find("td:last").find(":button");
        var feeType = select.val();
        if (feeType) {
            if (feeType == "false") {
                simple.fadeIn();
                prorated.hide();
            } else {
                simple.hide();
                prorated.fadeIn();
            }
            fChangeVisibility(table.find(".fee-full-IsFixedRate"), "IsFixedRate");
        } else {
            simple.hide();
            prorated.hide();
        }
    }
    else if (type == "NeedPriorityExpenseCap") {
        var priorityCap = table.find(".fee-full-PriorityExpenseCap").parent("td");
        if (select.prop("checked")) {
            priorityCap.fadeIn();
        } else {
            priorityCap.find(".fee-full-PriorityExpenseCap").val("");
            priorityCap.hide();
        }
    }
    else if (type == "HasDiffFirstRate") {
        var firstRate = table.find("td.diffFirstRate");
        if (select.prop("checked")) {
            firstRate.fadeIn();
        } else {
            firstRate.hide();
        }
    }
    else {
        if (table.find(".fee-full-IsProRated").val() == "true") {
            var fix = table.find(".feeFixedRate");
            var noFix = table.find(".feeNoFixedRate");
            if (select.prop("checked")) {
                fix.fadeIn();
                noFix.hide();
            } else {
                fix.hide();
                noFix.fadeIn();
            }
        }
    }
}
function nChangeVisibility(click, type) {
    if (type == "IsFixed") {
        var table = click.parent("td").parent("tr").parent("tbody").parent("table");
        var floating = table.find(".floatingRate");
        var fixed = table.find(".fixedRate");
        if (click.prop("checked")) {
            floating.hide();
            fixed.fadeIn();
        } else {
            floating.fadeIn();
            fixed.hide();
        }
    } else if (type == "HasAmortizationSchedule") {
        var trs = click.parent("td").parent("tr").siblings(".hasAmortizationSchedule");
        if (click.prop("checked")) {
            trs.fadeIn();
        } else {
            trs.find(".amort-schedule").html("");
            trs.hide();
        }
    } else if (type == "HasUnpaidInterest") {
        var td = click.parent("td").siblings(".hasUnpaidInterest");
        if (click.prop("checked")) {
            td.fadeIn();
        } else {
            td.find(".amort-schedule").html("");
            td.hide();
        }
    }
}
function crChangeVisibility(has, type) {
    var items;
    var cbTd, items, fixed, sub;//cb->checkbox
    if (type == "HasReinvestment") {
        cbTd = $(".noReinvestment");
        items = $(".hasReinvestment");
        fixed = 1;
        sub = $("tr.hasReinvestment").size() + $("td.hasReinvestment").size() / 2;
    }
    if (has) {
        cbTd.attr("rowspan", sub);
        cbTd.removeAttr("colspan");
        cbTd.prev().attr("rowspan", fixed + sub - 1);
        items.fadeIn();
    } else {
        cbTd.removeAttr("rowspan");
        cbTd.attr("colspan", 3);
        cbTd.prev().attr("rowspan", fixed);
        items.hide();
    }
}
function ceChangeVisibility(has, type) {
    var items;
    var cbTd, items, fixed, sub;
    if (type == "HasRiskReserve") {
        cbTd = $(".noRiskReserve");
        items = $(".hasRiskReserve");
        fixed = 1;
        sub = $("tr.hasRiskReserve").size() + $("td.hasRiskReserve").size() / 2;
    } else if (type == "HasCumlossTurbo") {
        cbTd = $(".noCumlossTurbo");
        items = $(".hasCumlossTurbo");
        fixed = 2;
        sub = $("tr.hasCumlossTurbo").size() + $("td.hasCumlossTurbo").size() / 2;
    } else if (type == "HasEod") {
        var cbTd = $(".noEod");
        var items = $(".hasEod");
        fixed = 2;
        sub = items.size();
    } else if (type == "HasDeficiencyPayment") {
        cbTd = $(".noDeficiencyPayment");
        items = $(".hasDeficiencyPayment");
        fixed = 2;
        sub = items.size();
    }
    if (has) {
        cbTd.attr("rowspan", sub);
        cbTd.removeAttr("colspan");
        cbTd.prev().attr("rowspan", fixed + sub - 1);
        items.fadeIn();
    } else {
        cbTd.removeAttr("rowspan");
        cbTd.attr("colspan", 3);
        cbTd.prev().attr("rowspan", fixed);
        items.hide();
    }
}

/* element operation */
/*Fee*/
function addFee() {
    var count = $(".fees>.fee").size();
    var template = $("#template_fee").html();
    $(".fees").append(template);
    var newFeeFull = $(".fees").find(".fee-full:last");
    setFeeAttr(newFeeFull, count);
    newFeeFull.show();
    fCheckData($(".module.module-fees>table:first"));
}
function editFee(button) {
    var btn = $(button);
    if (btn.val() == "修改") {
        btn.parent("td").parent("tr").next().show();
        btn.val("完成");
        btn.text("完成");
    } else {
        var tr = btn.parent("td").parent("tr");
        var detail = tr.next();
        tr.find(".fee-ordinal").text(detail.find(".fee-full-PaymentOrdinal").val());
        var select = detail.find(".fee-full-IsProRated>:selected");
        if (select.val())
            tr.find(".fee-type").text(select.text());
        else
            tr.find(".fee-type").text("");
        tr.find(".fee-name").text(detail.find(".fee-full-Name").val());
        tr.find(".fee-description").text(detail.find(".fee-full-Description").val());
        fCheckData($(".module.module-fees>table:first"));
        detail.hide();
        btn.val("修改");
        btn.text("修改");
    }
}
function removeFee(button) {
    var btn = $(button);
    var subscript = parseInt(btn.attr("data-subscript"));
    var tr = btn.parent("td").parent("tr");
    tr.next().remove();
    tr.remove();
    var i = subscript;
    var count = $(".fees>.fee").size();
    var feefulls = $(".fees").find(".fee-full");
    for (; i < count; i++) {
        setFeeAttr(feefulls.eq(i), i);
    }
    fCheckData($(".module.module-fees>table:first"));
}
function setFeeAttr(fee, subscript) {
    var items = ["Name", "NeedUnpaidAccount", "NeedInterestOnUnpaidAccount", "Description", "FeeBasisType",
		"IsFixedRate", "IsPerPaymentRate", "HasDiffFirstRate", "FirstRate", "FloatingIndex", "Spread", "FixedRate", "Cap", "IsCumulativeCap", "Floor",
        "FixedAmount", "PaymentOrdinal", "IsProRated", "NeedPriorityExpenseCap", "PriorityExpenseCap", "AccrualMethod"];
    var i = 0;
    var feeInput;
    for (; i < items.length; i++) {
        feeInput = fee.find(".fee-full-" + items[i]);
        feeInput.attr("id", "Fees_FeeComponents_" + subscript + "__" + items[i]);
        feeInput.attr("name", "Fees.FeeComponents[" + subscript + "]." + items[i]);
        fee.find(".lbl-fee-full-" + items[i]).attr("for", "Fees_FeeComponents_" + subscript + "__" + items[i]);
    }

    var selects = ["NeedPriorityExpenseCap", "NeedUnpaidAccount", "NeedInterestOnUnpaidAccount", "IsFixedRate", "IsPerPaymentRate", "HasDiffFirstRate",
        "IsCumulativeCap"];
    i = 0;
    for (; i < selects.length; i++) {
        feeInput = fee.find("input[name*='" + selects[i] + "']");
        feeInput.attr("name", "Fees.FeeComponents[" + subscript + "]." + selects[i]);
        if (feeInput.size() == 1) {
            feeInput.next("input").attr("name", "Fees.FeeComponents[" + subscript + "]." + selects[i]);
        } else if (feeInput.size() == 2) {
            $(feeInput[1]).attr("name", "Fees.FeeComponents[" + subscript + "]." + selects[i]);
        }
    }
    fee.prev().find(":button").attr("data-subscript", subscript);

    fee.find(".tbFeeDetail").data("name-prefix", "Fees.FeeComponents[" + subscript+"].PaymentSchedules")
}
/*Note*/
function addNote() {
    var count = $(".notes>.note").size();
    var template = $("#template_note").html();
    $(".notes").append(template);
    var newNoteFull = $(".notes").find(".note-full:last");
    setNoteAttr(newNoteFull, count);
    newNoteFull.show();
    nCheckData($(".module.module-notes>table:first"));
}
function editNote(button) {
    var btn = $(button);
    var oapb = $("#CreditEnhancement_OriginalAPB").val();
    var tr = btn.parent("td").parent("tr");
    var detail = tr.next();
    if (btn.val() == "修改") {
        detail.show();
        btn.val("完成");
        btn.text("完成");
        btn.focus();
    } else {
        detail.find(".note-full-ExpectedMaturityDate").attr("placeholder", "(选填)");
        tr.find(".note-description").text(detail.find(".note-full-Description").val());
        tr.find(".note-name").text(detail.find(".note-full-Name").val());
        var notional = detail.find(".note-full-Notional");
        if (!notional.hasClass("modeling-field-error")) {
            tr.find(".note-notional").text(parsePercent(notional.val()) + "%");
        }
        else {
            tr.find(".note-notional").text("");
            notional.val("");
        }
        if (detail.find(".note-full-IsFixed").prop("checked")) {
            var rate = parsePercent(detail.find(".note-full-FixedRate").val(), "pct");
            if (rate === "") {
                tr.find(".note-rate").text("");
            }
            else {
                tr.find(".note-rate").text(rate + "%");
            }
        }
        else {
            var tdRate = tr.find(".note-rate");
            var spread = parsePercent(detail.find(".note-full-Spread").val(), "bps");
            if (spread === "") {
                tdRate.text("");
            }
            else {
                var tmp = spread.toString().replace("-", "- ");
                if (spread >= 0) {
                    tmp = "+ " + tmp;
                }
                tdRate.text("基准 " + tmp + "%");
            }
        }
        tr.find(".note-ordinal").text(detail.find(".note-full-PaymentOrdinal").val());
        detail.hide();
        drawNoteStructure();
        btn.val("修改");
        btn.text("修改");
    }
}
function removeNote(button) {
    var btn = $(button);
    var subscript = parseInt(btn.attr("data-subscript"));
    var tr = btn.parent("td").parent("tr");
    tr.next().remove();
    tr.remove();
    var i = subscript;
    var count = $(".notes>.note").size();
    var notefulls = $(".notes").find(".note-full");
    for (; i < count; i++) {
        setNoteAttr(notefulls.eq(i), i);
    }
    drawNoteStructure();
    nCheckData($(".module.module-notes>table:first"));

}
function addAmortSchedule(button) {
    var btn = $(button);
    var parentSubscript = parseInt(btn.attr("data-parent-subscript"));
    var parentSchedule = $(".amort-schedule:eq(" + parentSubscript + ")");
    var count = parentSchedule.find(".amort-schedule-item").size();
    var template = $("#template_amort_schedule_item").html();
    parentSchedule.append(template);
    var newItem = parentSchedule.find(".amort-schedule-item:last");
    var newInputs = newItem.find("input:text");
    newInputs.addClass("modeling-field-error");
    setScheduleAttr(newItem, count, parentSubscript);
    $(".date").datepicker({
        format: "yyyy-mm-dd"
    });
}
function removeAmortSchedule(button) {
    var btn = $(button);
    var i = parseInt(btn.attr("data-subscript"));
    var j = parseInt(btn.attr("data-parent-subscript"));
    var tr = btn.parent("td").parent("tr");
    tr.remove();
    var count = $(".amort-schedule:eq(" + j + ")").find(".amort-schedule-item").size();
    var amortSchedules = $(".amort-schedule").find(".amort-schedule-item");
    for (; i < count; i++) {
        setScheduleAttr(amortSchedules.eq(i), i, j);
    }
}
function setNoteAttr(note, subscript, type) {
    var items = ["Name", "Description", "IsEquity", "IsFixed", "HasInterestOnUnpaidInterest", "Notional",
		"FixedRate", "HasUnpaidInterest", "HasInterestOnUnpaidInterest", "FloatingIndex", "Spread", "AccrualMethod", "UnAdjustedAccrualPeriods",
		"PaymentOrdinal", "NoFixedAmount", "ExpectedMaturityDate", "HasAmortizationSchedule"];
    var selects = ["IsEquity", "IsFixed", "HasUnpaidInterest", "HasInterestOnUnpaidInterest", "UnAdjustedAccrualPeriods", "HasAmortizationSchedule"];
    var i = 0;
    var noteInput;
    for (; i < items.length; i++) {
        noteInput = note.find(".note-full-" + items[i]);
        noteInput.attr("id", "Notes_NoteList_" + subscript + "__" + items[i]);
        noteInput.attr("name", "Notes.NoteList[" + subscript + "]." + items[i]);
        note.find(".lbl-note-full-" + items[i]).attr("for", "Notes_NoteList_" + subscript + "__" + items[i]);
    }
    i = 0;
    for (; i < selects.length; i++) {
        noteInput = note.find("input[name*='" + selects[i] + "']");
        noteInput.attr("name", "Notes.NoteList[" + subscript + "]." + selects[i]);
        if (noteInput.size() == 1) {
            noteInput.next("input").attr("name", "Notes.NoteList[" + subscript + "]." + selects[i]);
        } else if (noteInput.size() == 2) {
            $(noteInput[1]).attr("name", "Notes.NoteList[" + subscript + "]." + selects[i]);
        }
    }
    note.find(":button").attr("data-parent-subscript", subscript);
    note.prev().find(":button").attr("data-subscript", subscript);
}
function setScheduleAttr(item, subscript, parentSubscript) {
    var items = ["Dates", "Values"];
    var i = 0;
    var itemInput;
    for (; i < items.length; i++) {
        itemInput = item.find(".amort-schedule-item-" + items[i]);
        itemInput.attr("id", "Notes_NoteList_" + parentSubscript + "__AmortizationSchedule_" + items[i] + "_" + subscript + "_");
        itemInput.attr("name", "Notes.NoteList[" + parentSubscript + "].AmortizationSchedule." + items[i] + "[" + subscript + "]");
    }
    item.find(":button").attr("data-subscript", subscript);
    item.find(":button").attr("data-parent-subscript", parentSubscript);
}

/* validate data */
function checkData(container, module, inputs) {
    var elem;
    var fromModule = !!module;
    var regDate = /^((1|2)\d{3})-(0\d{1}|1[0-2])-(0\d{1}|[12]\d{1}|3[01])$/;
    var length = fromModule ? inputs.length : container.length;
    var pass = true;
    for (var i = 0; i < length; i++) {
        elem = fromModule ? container.find("#" + module + "_" + inputs[i]) : $("#" + container[i]);
        if (!elem.size() || !$.trim(elem.val()) || (elem.hasClass("number") && !validateNumber(elem.val()))
            || (elem.hasClass("date") && (elem.val() == "" || elem.val() == "0001-01-01") && !regDate.test(elem.val()))
            || ((module.indexOf("Fee") > -1 || module.indexOf("Note") > -1) && inputs[i] == "Name" && (cnabsIsFloat(elem.val()) || elem.val().match(/[-\\\/:\*\?<>&%+#@!\|]+/)))) {
            elem.addClass("modeling-field-error");
            pass = false;
            if (elem.hasClass("date") && inputs[i] == "ExpectedMaturityDate") {
                elem.val("");
                elem.attr("placeholder", "(请输入正确合理的日期)");
                elem.removeClass("modeling-field-error");
                pass = true;
            }
        }
        else {
            elem.removeClass("modeling-field-error");
        }
    }
    return pass;
}
function bCheckData() {
    var div = $(".module.basicinformation");
    var inputs = ["DealFullName", "DealName"];
    var result = checkData(div, "BasicInformation", inputs);
    div.find(".data-failed").toggle(!result);
    div.find(".data-checked").toggle(result);
    return result;
}
var errmessage = "";
function sCheckData() {
    var div = $(".schedule");
    var inputs = ["ClosingDate", "FirstPaymentDate", "PaymentFrequency",
		"LegalMaturity", "PaymentRolling", "DeterminationRuleType"];
    if ($("#Schedule_DeterminationRuleType").val() == "Simple") {
        inputs.push("DeterminationOffset");
    } else {
        inputs.push.apply(inputs, ["FirstDeterminationDateBegin", "FirstDeterminationDateEnd", "DeterminationDateRolling"]);
    }
    if (checkData(div, "Schedule", inputs)) {
        var pass = true;
        var err = new Array();
        var field_cd = $("#Schedule_ClosingDate"),
            field_fpd = $("#Schedule_FirstPaymentDate"),
                field_fddb = $("#Schedule_FirstDeterminationDateBegin"),
                    field_fdde = $("#Schedule_FirstDeterminationDateEnd"),
                        field_lmd = $("#Schedule_LegalMaturity");
        var cd = new Date(Date.parse(field_cd.val())),
            fpd = new Date(Date.parse(field_fpd.val())),
                fddb = new Date(Date.parse(field_fddb.val())),
                    fdde = new Date(Date.parse(field_fdde.val())),
                        lmd = new Date(Date.parse(field_lmd.val()));
        if ($("#Schedule_PenultimateDate").val() != "" && $("#Schedule_PenultimateDate").val() != "0001-01-01") {
            var regDate = /^((1|2)\d{3})-(0\d{1}|1[0-2])-(0\d{1}|[12]\d{1}|3[01])$/;
            var field_pnd = $("#Schedule_PenultimateDate")
            if ((field_pnd.val() != "" && field_pnd.val() != "0001-01-01") && !regDate.test(field_pnd.val())) {
                field_pnd.addClass("modeling-field-error");
                pass = false;
                err.push("到期日前一支付日格式不正确，该项为选填，如果需要填写，请填写正确格式");
            }
            var pnd = new Date(Date.parse(field_pnd.val()));
            if (pnd > lmd) {
                err.push("法定到期日前一支付日 须在 法定到期日 之前");
                field_pnd.addClass("modeling-field-error");
                field_lmd.addClass("modeling-field-error");
                $("#Schedule_PenultimateDate").val("");
                pass = false;
            }
            if (pnd < fpd) {
                err.push("法定到期日前一支付日 须在 首次支付日 之后");
                field_fpd.addClass("modeling-field-error");
                field_pnd.addClass("modeling-field-error");
                $("#Schedule_PenultimateDate").val("");
                pass = false;
            }
            if (pass) {
                field_pnd.removeClass("modeling-field-error");
            }
        }
        else {
            $("#Schedule_PenultimateDate").removeClass("modeling-field-error");
        }
        if (fpd <= cd) {
            err.push("首次支付日须在起息日之后");
            field_fpd.addClass("modeling-field-error");
            field_cd.addClass("modeling-field-error");
            pass = false;
        }
        if (lmd <= fpd) {
            err.push("法定到期日须在首次支付日之后");
            field_fpd.addClass("modeling-field-error");
            field_lmd.addClass("modeling-field-error");
            pass = false;
        }

        if ($("#Schedule_DeterminationRuleType").val() == "Complex") {
            if (fpd <= fdde) {
                err.push("首次支付日须在首个收款期截止日之后");
                field_fpd.addClass("modeling-field-error");
                field_fdde.addClass("modeling-field-error");
                pass = false;
            }
            if (fdde <= fddb) {
                err.push("首个收款期截止日须在首个收款期起始日之后");
                field_fdde.addClass("modeling-field-error");
                field_fddb.addClass("modeling-field-error");
                pass = false;
            }
        }

        if (err.length == 0) {
            div.find(".data-failed").hide();
            div.find(".data-checked").show();
            pass = true;
        }

        if ($("#Schedule_AdjustPaymentSchedule").prop("checked")) {
            var regDate = /^((1|2)\d{3})-(0\d{1}|1[0-2])-(0\d{1}|[12]\d{1}|3[01])$/;
            $(".sched-custom-date").each(function () {
                var cpd = $(this);
                if ((cpd.val() != "" && cpd.val() != "0001-01-01") && !regDate.test(cpd.val())) {
                    cpd.addClass("modeling-field-error");
                    pass = false;
                    err.push("自定义偿付日格式不正确");
                }
                else {
                    cpd.removeClass("modeling-field-error");
                }

            });
        }
        if ($("#Schedule_AdjustCollectionSchedule").prop("checked")) {
            var regDate = /^((1|2)\d{3})-(0\d{1}|1[0-2])-(0\d{1}|[12]\d{1}|3[01])$/;
            $(".sched-custom-date").each(function () {
                var cpd = $(this);
                if ((cpd.val() != "" && cpd.val() != "0001-01-01") && !regDate.test(cpd.val())) {
                    cpd.addClass("modeling-field-error");
                    pass = false;
                    err.push("自定义收款日格式不正确");
                }
                else {
                    cpd.removeClass("modeling-field-error");
                }

            });
        }

        if (pass && err.length == 0) {
            div.find(".data-failed").hide();
            div.find(".data-checked").show();
            return true;
        }
        else {
            if (errmessage != err.join("<br />")) {
                alertify.error(err.join("<br />"));
                div.find(".data-checked").hide();
                div.find(".data-failed").show();
                errmessage = err.join("<br />");
            }

            return false;
        }
    }
    div.find(".data-checked").hide();
    div.find(".data-failed").show();
    return false;
}
function fCheckData() {
    var mainTable = $(".module.module-fees>table:first");
    var pass = true;
    var fees = mainTable.find(".fee-full table.inner");
    if (fees.size() > 0) {
        var inputs;
        var table;
        var module;
        for (var i = 0; i < fees.size() ; i++) {
            table = $(fees[i]);
            mainTable.find("tr.fee:eq(" + i + ")").removeClass("modeling-item-error");
            table.find(".modeling-field-error").removeClass("modeling-field-error");
            var inputs = ["PaymentOrdinal", "IsProRated", "Name", "Description"];
            module = "Fees_FeeComponents_" + i + "_";
            if ($("#" + module + "_NeedPriorityExpenseCap").prop("checked")) {
                inputs.push("PriorityExpenseCap");
            }
            if ($("#" + module + "_HasDiffFirstRate").prop("checked")) {
                inputs.push("FirstRate");
            }
            if ($("#" + module + "_IsProRated").val() == "false") {
                inputs.push("FixedAmount");
            } else {
                inputs.push("FeeBasisType");
                if ($("#" + module + "_IsFixedRate").prop("checked")) {
                    inputs.push("FixedRate");
                } else {
                    inputs.push("FloatingIndex");
                    inputs.push("Spread");
                }
            }
            var details = table.find(".tbFeeDetail tbody tr");
            for (var j = 0; j < details.length; j++) {
                inputs.push("PaymentSchedules_" + j + "__Date");
            }

            if (!checkData(table, module, inputs)) {
                var fee = mainTable.find("tr.fee:eq(" + i + ")");
                fee.addClass("modeling-item-error");
                pass = false;
            }
        }
    }
    if (pass) {
        mainTable.find(".modeling-item-error").removeClass("modeling-item-error");
        mainTable.find(".data-failed").hide();
        mainTable.find(".data-checked").show();
    } else {
        mainTable.find(".data-checked").hide();
        mainTable.find(".data-failed").show();
    }
    return pass;
}
function nCheckData() {
    var mainTable = $(".module.module-notes>table:first");
    var pass = true;
    var notes = mainTable.find(".note-full").find("table:first");
    if (notes.size() > 0) {
        var inputs, table, module;
        var sumNotional = 0;
        for (var i = 0; i < notes.size() ; i++) {
            var notePass = true;
            table = $(notes[i]);
            mainTable.find("tr.note:eq(" + i + ")").removeClass("modeling-item-error");
            table.find(".modeling-field-error").removeClass("modeling-field-error");
            var inputs = ["Notional", "Description", "AccrualMethod", "Name", "PaymentOrdinal"];
            module = "Notes_NoteList_" + i + "_";
            if ($("#" + module + "_ExpectedMaturityDate").val() != "") {
                inputs.push("ExpectedMaturityDate");
            }
            if ($("#" + module + "_IsFixed").prop("checked")) {
                inputs.push("FixedRate");
            } else {
                inputs.push.apply(inputs, ["FloatingIndex", "Spread"]);
            }
            if ($("#" + module + "_HasAmortizationSchedule").prop("checked")) {
                var items = table.find(".amort-schedule-item");
                if (items.size() == 0) {
                    inputs.push("AmortizationSchedule_Dates_-1_");
                }
                else {
                    for (var j = 0; j < items.size() ; j++) {
                        inputs.push("AmortizationSchedule_Dates_" + j + "_");
                        inputs.push("AmortizationSchedule_Values_" + j + "_");
                    }
                }
            }
            if (!checkData(table, module, inputs)) {
                notePass = false;
            }
            else {
                var notional = table.find(".note-full-Notional");
                var nval = parseFloat(notional.val());
                if (nval && nval >= 0) {
                    sumNotional += nval;
                }
                else {
                    notional.addClass("modeling-field-error");
                    notePass = false;
                }
            }
            if (!notePass) {
                var note = mainTable.find("tr.note:eq(" + i + ")");
                note.addClass("modeling-item-error");
                pass = false;
            }
        }
        if (sumNotional > 1) {
            $("#noteStructure").addClass("modeling-item-error");
            $("#noteStructure").addClass("error");
            pass = false;
        }
        else {
            $("#noteStructure").removeClass("error");
            $("#noteStructure").removeClass("modeling-item-error");
        }
    } else {
        pass = false;
    }
    if (pass) {
        mainTable.find(".modeling-item-error").removeClass("modeling-item-error");
        mainTable.find(".data-failed").hide();
        mainTable.find(".data-checked").show();
    } else {
        mainTable.find(".data-checked").hide();
        mainTable.find(".data-failed").show();
    }
    return pass;
}
function crCheckData() {
    var div = $(".module.collateralrule");
    var inputs = new Array();
    if ($("#CollateralRule_HasReinvestment").prop("checked")) {
        inputs.push("ReinvestmentRule_ReinvestmentEndDate");
        if ($("#CollateralRule_ReinvestmentRule_ReinvestmentRuleType").val() != "similar") {
            inputs.push.apply(inputs, ["ReinvestmentRule_Wal", "ReinvestmentRule_Wac"]);
        }
        if (!$("#CollateralRule_ReinvestmentRule_ReinvestmentRuleType").val()
            || !checkData(div, "CollateralRule", inputs)) {
            div.find(".data-checked").hide();
            div.find(".data-failed").show();
            return false;;
        }
    }
    div.find(".data-failed").hide();
    div.find(".data-checked").show();
    return true;
}
function ceCheckData() {
    var div = $(".module.creditenhancement");
    var inputs = new Array();
    if ($("#CreditEnhancement_HasCumlossTurbo").prop("checked")) {
        inputs = ["CumlossTurboBasisType", "CumlossTurboThreshold"];
    }
    if ($("#CreditEnhancement_HasRiskReserve").prop("checked")) {
        inputs.push.apply(inputs, ["RiskReserveCap", "RiskReserveInterestRate"]);
    }
    var result = checkData(div, "CreditEnhancement", inputs);
    div.find(".data-failed").toggle(!result);
    div.find(".data-checked").toggle(result);
    return result;
}
function scCheckData() {
    var div = $(".module.scenario");
    var divCdrCpr = $("#divCdrCpr");
    var result = true;
    var CDR = divCdrCpr.find("#ScenarioCDR"),
        CPR = divCdrCpr.find("#ScenarioCPR");
    if (divCdrCpr.find("#btnCustom").hasClass("sc-button-current")) {
        div.find("#ScenarioRule_UseCustomScenario").prop("checked", true);
        var inputs = ["ScenarioCDR", "ScenarioCPR"];
        result = checkData(inputs);
        if (result) {
            div.find("#ScenarioRule_CDR").val($.trim(CDR.val()));
            div.find("#ScenarioRule_CPR").val($.trim(CPR.val()));
        }
    } else {
        div.find("#ScenarioRule_UseCustomScenario").prop("checked", false);
    }
    divCdrCpr.find(".scenario-data-failed").toggle(!result);
    divCdrCpr.find(".scenario-data-checked").toggle(result);

    var result = true;
    var divRates = $("#divIndexRates");
    if (divRates.find("#btnChangeRate").prop("checked")) {
        divRates.find(".confirm-change-rate:checked").each(function () {
            var tb = $(this).parent("td").parent("tr").find(":text");
            if (validateNumber(tb.val())) {
                tb.removeClass("modeling-field-error");
            }
            else {
                result = false;
                tb.addClass("modeling-field-error");
            }
        });
    }
    divRates.find(".scenario-data-failed").toggle(!result);
    divRates.find(".scenario-data-checked").toggle(result);
    return result;
}

/* submit handler */
var saving = false;
function checkStatus() {
    bCheckData();
    //sCheckData();
    fCheckData();
    nCheckData();
    crCheckData();
    ceCheckData();
    scCheckData();
    return !$("img.data-failed:visible").size();
}
function ajaxSaveForm(callback, silence) {
    saving = true;
    if (typeof callback == "boolean") {
        silence = callback;
        callback = null;
    }
    if (!silence) {
        alertify.message('正在保存模型数据...');
    }
    $("#IsInfoComplete").val(checkStatus());
    var formData = $("#formModeling").serialize();
    $.ajax({
        url: '/Model/SaveForm',
        type: 'post',
        data: formData,
        dataType: 'text',
        success: function (data) {
            var result = JSON.parse(data);
            var code = result.Code; 
            if (result.Code === 0) {
                alertify.success("数据保存成功");

            } else {
                alertify.error("数据保存失败:" + result.Message );
            }
            //saving = false;
            //if (data == "success") {
            //    if (callback)
            //        callback(data);
            //}
            //else if (!silence) {
            //    alertify.error("数据保存失败");
            //}
        },
        error: function (e) {
            saving = false;
            if (!silence) {
                alertify.error("数据保存失败");
            }
        }
    });
}
function ajaxSaveFormBefor(callback, silence) {
    saving = true;
    if (typeof callback == "boolean") {
        silence = callback;
        callback = null;
    }
    $("#IsInfoComplete").val(checkStatus());
    var formData = $("#formModeling").serialize();
    $.ajax({
        url: '/MyModels/SaveForm',
        type: 'post',
        data: formData,
        dataType: 'text',
        success: function (data) {
            saving = false;
            if (data == "success") {
                if (callback)
                    callback(data);
            }
            else if (!silence) {
                alertify.error("数据保存失败");
            }
        },
        error: function (e) {
            saving = false;
            if (!silence) {
                alertify.error("数据保存失败");
            }
        }
    });
}
function ajaxSaveScenario(scenarioId, callback) {
    //var divRates = $("#divIndexRates");
    //if (divRates.find("#btnChangeRate").prop("checked")
    //    && divRates.find(".confirm-change-rate:checked").size() > 0) {

    //    saving = true;
    //    alertify.message('正在保存情景数据...');

    //    var arr = new Array();
    //    divRates.find(".confirm-change-rate:checked").each(function () {
    //        var rate = {};
    //        var tr = $(this).parent("td").parent("tr");
    //        rate.IndexCode = $.trim(tr.find("td:eq(1)").attr("data-code"));
    //        rate.BeginDate = $.trim(tr.find("td:eq(2)").text());
    //        rate.InterestRate = $.trim(tr.find(":text").val());
    //        arr.push(rate);
    //    });

    $.ajax({
        url: '/MyProjects/SaveScenario',
        type: 'post',
        data: { scenarioGuid: scenarioId },
        dataType: 'text',
        success: function (data) {
            saving = false;
            if (data == "success") {
                if (callback)
                    callback(data);
            }
            else {
                alertify.error("数据保存失败");
            }
        },
        error: function (e) {
            saving = false;
            alertify.error("数据保存失败");
        }
    });
}
function submitHandler(operation) {
    switch (operation) {
        case 0:
            if (!saving) {
                //if (!checkStatus()) {
                //    alertify.error("信息不完整，无法加载或分析");
                //    return;
                //}
                ajaxSaveForm(function () {
                    alertify.success('数据保存成功');
                });
            }
            break;
        case 1:
            if (!checkStatus()) {
                alertify.error("信息不完整，无法加载或分析");
                return;
            }
            alertify.message("正在加载偿付模型...");
            var divWaterfall = $("#divWaterfall");
            divWaterfall.dialog("open");
            divWaterfall.dialog("moveToTop");
            if ($("#Guid").val() != null) {
                cashflowChartDealLabByModelId($("#Guid").val(), $("#chtWaterfall"), divWaterfall);
            }
            else {
                cashflowChartDealLabByProjectId($("#ProjectGuid").val(), $("#chtWaterfall"), divWaterfall);
            }
            break;
        case 2:
            if (!checkStatus()) {
                alertify.error("信息不完整，无法加载或分析");
                return;
            }
            if ($("img.scenario-data-checked").size() != $("img.scenario-data-checked:visible").size()) {
                alertify.message("请设置有效的情景参数");
                return;
            }
            var scenarioId = 0
            if ($("#selectedScenarioId").val() != null) {
                scenarioId = $("#selectedScenarioId").val();
            }
            var scenario = { scenarioId: scenarioId };

            alertify.message("正在生成报告...");
            $("#formProjectSetting").submit();
            break;
        case 3:
            if (!sCheckData()) {
                alertify.error("信息不完整，无法加载或分析");
                return;
            }
            var scenarioId = 0
            if ($("#selectedScenarioId").val() != null) {
                scenarioId = $("#selectedScenarioId").val();
            }
            alertify.message("正在加载分析结果...");

            $(".tr-result").remove();
            $(".analysis-chart").html('<img src="/Images/ajaxloader.gif" style="padding-top: 100px" />');

            var guid = $("#ProjectGuid").val();

            $.ajax({
                type: "POST",
                timeout: 18000000,
                url: "/Chart/GetStaticAnalyzerResult",
                data: { projectGuid: guid, needData: true, scenarioId: scenarioId },
                dataType: "json",
                success: function (data) {
                    if (data != null)
                    {
                        var tableData = data[0];
                        var columnCount = tableData[0][0].length;
                        $(".full-col").attr("colspan", columnCount);

                        var noteResultHead = $(".note-result-head");
                        var noteResultBody = $(".note-result-body");
                        var headCode, bodyCode;
                        $(tableData).each(function (index, table) {
                            $(table).each(function (ind, element) {
                                if (ind == 0) {
                                    headCode = "<tr class='tr-result'>", bodyCode = "";
                                    $(element).each(function (i, e) {
                                        headCode += "<th>" + e + "</th>";
                                    });
                                    headCode += "</tr>";
                                    noteResultHead.eq(index).append(headCode);
                                    return;
                                }
                                bodyCode += "<tr class='tr-result'>";
                                $(element).each(function (i, e) {
                                    bodyCode += "<td>" + e + "</td>";
                                });
                                bodyCode += "</tr>";
                            });
                            noteResultBody.eq(index).prepend(bodyCode);
                        });

                        var chartData = data[1];
                        LineChart(chartData[0], 'IRR', 'cht_IRR');
                        LineChart(chartData[1], 'Loss', 'cht_Loss');
                        LineChart(chartData[2], 'NPV', 'cht_NPV');

                        var scenario = data[2];
                        var sceStr = data[3];
                        $("span.base-scenario").html(sceStr);
                    }
                },
                error: function () {
                    $(".note-result-body .analysis-chart").text("数据加载失败");
                }
            });
            $.ajax({
                type: "POST",
                url: "/Chart/GetNoteRatingAnalyzerDistribution",
                data: { projectGuid: guid, needData: true },
                dataType: "json",
                success: function (data) {
                    var htmlCode = "";
                    $(data[0].Results).each(function (index, element) {
                        htmlCode += "<tr class='tr-result'>";
                        htmlCode += "<td>" + element.NoteName + "</td>";
                        htmlCode += "<td>" + toPercent(element.ExpectedLoss) + "</td>";
                        htmlCode += "<td>" + element.Wal.toFixed(1) + "</td>";
                        htmlCode += "<td>" + Math.round(element.CnabsRatingScore) + "</td>";
                        htmlCode += "<td>" + element.ImpliedChineseRatingStr + "</td>";
                        htmlCode += "</tr>";
                    });
                    $("#betResult").prepend(htmlCode);
                    PieChart(data[1], '证券评级分布', 'cht_BetResults');
                },
                error: function () {
                    $("#cht_BetResults").text("数据加载失败");
                }
            });
            /* it takes too long to calculate */
            //$.ajax({
            //    type: "POST",
            //    url: "/GetRatingAnalyzerResult/Chart",
            //    data: { id: guid },
            //    dataType: "json",
            //    success: function (data) {
            //        LinePlotChartRating(data.ListLineSeries, "", "chtDiversityRating", data.PlotLabel, data.PlotValue);
            //    },
            //    error: function () {
            //        alertify.alert("NoteRating Distribution Error");
            //    }
            //});

            $('#divScenario').dialog('close');
            $(".results").dialog("open");
            break;
        case 4:
            var ocPct = parseFloat($("#txtOcPct").val());
            if ((!ocPct && ocPct != 0) || ocPct > 100 || ocPct < 0) {
                alertify.message("请输入正确的超额抵押率");
                return;
            }

            var nm = $("#cbNeedMezzanine").prop("checked");

            var sr = parseFloat($("#txtSeniorRate").val()),
                mr = parseFloat($("#txtMezzRate").val());
            if (!sr) {
                alertify.message("请输入正确的优先级利率");
                return;
            }
            if (nm) {
                if (!mr) {
                    alertify.message("请输入正确的夹层级利率");
                    return;
                }
            }
            else {
                mr = null;
            }

            alertify.message("正在提交计算请求...");
            var id = $("#ProjectGuid").val();
            var str = $("#slSeniorTargetRating").val();
            var mtr = $("#slMezzTargetRating").val();
            $.ajax({
                url: '/MyProjects/CalculateTrancheSize',
                type: 'post',
                data: {
                    Guid: id,
                    OcPct: ocPct / 100,
                    NeedMezzanine: nm,
                    SeniorTargetRating: str,
                    MezzTargetRating: mtr,
                    SeniorRate: sr / 100,
                    MezzRate: mr / 100
                },
                dataType: 'text',
                success: function (data) {
                    switch (data) {
                        case "success":
                            $("#divCalculationSetting").dialog("close");
                            alertify.alert("提交成功", "如需手动修改模型，请取消测算", function () { location.replace(location.href); });
                            break;
                        case "failed":
                            alertify.alert("测算失败", "未能测算出符合要求的结果");
                            break;
                        case "complete":
                            alertify.alert("测算完成", "点击按钮刷新页面查看", function () { location.replace(location.href); });
                            break;
                        case "duplicated":
                            alertify.alert("错误", "测算中，请不要重复提交", function () { location.replace(location.href); });
                            break;
                        default:
                            alertify.error("提交失败");
                    }
                },
                error: function () {
                    alertify.error("提交失败");
                }
            });
            break;
        case 5:
            alertify.confirm("确认", "取消自动测算？",
                    function () {
                        alertify.message("正在取消...");
                        var id = $("#ProjectGuid").val();
                        $.ajax({
                            url: '/MyProjects/CancelTrancheSizeCalculation',
                            type: 'post',
                            data: {
                                projectGuid: id
                            },
                            dataType: 'text',
                            success: function (data) {
                                switch (data) {
                                    case "success":
                                        alertify.alert("取消成功", "点击按钮刷新页面", function () { location.replace(location.href); });
                                        break;
                                    case "failed":
                                        alertify.alert("测算失败", "未能测算出符合要求的结果", function () { location.replace(location.href); });
                                        break;
                                    case "complete":
                                        alertify.alert("测算完成", "点击按钮刷新页面查看", function () { location.replace(location.href); });
                                        break;
                                    case "none":
                                        alertify.alert("取消失败", "当前没有测算的项目");
                                        break;
                                    default:
                                        alertify.error("取消失败");
                                }
                            },
                            error: function () {
                                alertify.error("取消失败");
                            }
                        });
                    },
                    null
                );
            break;
        default:
            $("form").submit();
    }
}

/* tooltip */
function changeTooltipVisibility(btn) {
    if ($(btn).val() == "显示提示") {
        tooltipVisible = true;
        $(btn).val("关闭提示");
    }
    else {
        tooltipVisible = false;
        $(btn).val("显示提示");
    }
}
function showTip(target, e) {
    if (tooltipVisible) {
        var tip = target.tooltip;
        if (!tip) {
            tip = document.createElement("div");
            var content = target.getAttribute("data-help");

            if (target.getAttribute("id") != "") {
                tip.setAttribute("id", target.getAttribute("id") + "tooltip");
            }

            tip.className = "tooltip";
            tip.innerHTML += '<div>' + content + '</div>';

            tip.style.position = "absolute";
            tip.style.left = $(target).offset().left + "px";
            document.getElementsByTagName("body")[0].appendChild(tip);
            tip.style.top = $(target).offset().top - 8 - tip.clientHeight + "px";
            target.tooltip = tip;
        }
        if (!$(tip).is(":visible")) {
            $(target.tooltip).show();
            tip.style.top = $(target).offset().top - 8 - tip.clientHeight + "px";
        }
    }
}
function hideTip(target) {
    if (target.tooltip != null) {
        $(target.tooltip).hide();
    }
}
function bindTips() {
    var data;
    $.getJSON("/Scripts/modelHelper.js", function (d) {
        data = d;
        var modules = ["BasicInformation", "Schedule", "Fees", "Notes", "CollateralRule", "CreditEnhancement"];
        $(modules).each(function () {
            bindModule(this);
        });
    });
    function bindModule(module) {
        if (!data[module]) {
            return true;
        }
        $(data[module]).each(function () {
            var el;
            if ("FeesNotes".indexOf(module) > -1) {
                el = $("." + this.field);
            }
            else {
                el = $("#" + module + "_" + this.field)
            }
            el.addClass("hastip");
            el.attr("data-help", this.content);
        });
    };
}
function cutstr(val, max) {
    var returnValue = '';
    var byteValLen = 0;

    for (var i = 0; i < val.length; i++) {
        if (val[i].match(/[^\x00-\xff]/ig) != null)
            byteValLen += 2;
        else
            byteValLen += 1;
        if (byteValLen > max) {
            returnValue += "...";
            break;
        }

        returnValue += val[i];
    }

    return returnValue;
}

var tooltipVisible = true;
$(document).ready(function () {
    var guid = $("#Guid").val();
    var status = $("#ProjectStatus").val();
    var operation = 0;
    // showSteps(2, guid);
    var projectGuid = $("#ProjectGuid").val();
    if (status == "Optimizing") {
        $(".modeling-guide :button").click(function () {
            alertify.message("自动测算中，工程已被锁定，暂时无法修改");
        });
        $("#btnCancelCalculation").click(function () { submitHandler(5); });
        alertify.message("自动测算中，工程已被锁定，暂时无法修改");
        $(".np-button").prop("disabled", true);
        $("#btnShowAnalysisResult").prop("disabled", true);
        $("#btnComplete").prop("disabled", true);
        $("#btnSelectCollateral").prop("disabled", true);

        $(":text,textarea").prop("readonly", true);
        if ($("#Schedule_PenultimateDate").val() == "0001-01-01") {
            $("#Schedule_PenultimateDate").val("");
        }
        $(":checkbox,select").prop("disabled", true);
        var queryStatus = function () {

            $.ajax({
                type: "GET",
                url: "/MyProjects/GetProjectStatus",
                data: { projectGuid: projectGuid },
                dataType: "text",
                success: function (data) {
                    if (data == "Optimized" || data == "OptimizeFailed")
                        location.href = location.href;
                }
            });
        };
        setInterval(queryStatus, 10 * 1000);
    }
    else {
        if (status == "Optimized") {
            var confirm = alertify.confirm("自动测算完成，请查看结果");
            confirm.callback = function () {
                if (!$("img.data-failed:visible").size()) {
                    $.ajax({
                        url: "/MyProjects/SaveSettings",
                        type: 'post',
                        data: $("#formProjectSetting").serialize(),
                        dataType: 'text',
                        success: function (data) {
                            saving = false;
                            if (data == "success") {
                                alertify.success("设置保存成功！");
                                location.reload();
                            }
                            else {
                                alertify.alert("未能自动保存，请填写完整通过验证后再次保存设置。", data);
                            }
                        },
                        error: function (e) {
                            saving = false;
                            alertify.alert("未能自动保存，请填写完整通过验证后再次保存设置。", "服务器错误");
                        }
                    });
                }
                else {
                    alertify.alert("未能自动保存，请填写完整通过验证后再次保存设置。");
                }
            }
        }
        else if (status == "OptimizeFailed") {
            var confirm = alertify.alert("自动测算失败，未能找到符合要求的结果");
            confirm.callback = function () {
                var guid = $("#Guid").val();
                $.ajax({
                    type: "Post",
                    url: "/MyProjects/UpdateProjectStatus",
                    data: { projectGuid: projectGuid, status: "ScheduleSaved" },
                    dataType: "text",
                    success: function (data) {
                        if (data == "updated") {
                            alertify.success("已自动保存设置");
                        }
                    }
                });
            }
        }

        $(":text,textarea").prop("readonly", false);
        $(":checkbox,select").prop("disabled", false);
        $(".date").each(function () {
            var self = $(this);
            if (self.val() == "0001-01-01") {
                self.val("");
            }
            // self.prop("readonly", true);
        });
        $("#divScenario").dialog({
            closeText: "",
            title: "设置情景",
            autoOpen: false,
            closeOnEscape: false,
            show: true,
            hide: false,
            height: 220,
            width: 400,
            resizable: false,
            modal: true,
            open: function (event, ui) {
                if (!checkStatus()) {
                    $(this).dialog("close");
                    alertify.alert("错误", "信息不完整，无法加载或分析");
                }
            }
        });
        $("#divCalculationSetting").dialog({
            closeText: "",
            title: "分层评级设置",
            autoOpen: false,
            closeOnEscape: true,
            show: true,
            hide: true,
            height: 450,
            width: 400,
            resizable: true,
            modal: true
        });
        $("#divWaterfall").dialog({
            closeText: "",
            title: "偿付模型",
            autoOpen: false,
            closeOnEscape: true,
            show: true,
            hide: true,
            height: 600,
            width: 1050,
            resizable: true,
            modal: true,
            open: function () {
                $(this).animate({ scrollTop: 0 }, 1);
            }
        });
        $(".results").dialog({
            closeText: "",
            title: "分析结果",
            autoOpen: false,
            closeOnEscape: true,
            show: true,
            hide: true,
            height: 600,
            width: 1050,
            resizable: true,
            modal: true
        });

        checkStatus();
        bindTips();

        $(document).on("mouseover", ".hastip", function (e) { showTip(this, e); });
        $(document).on("mouseout", ".hastip", function (e) { hideTip(this, e); });
        $(document).on("change", ".small-value", function () {
            if (!$(this).hasClass("modeling-field-error"))
                convertIfHasPercent($(this));
        });
        $(document).on("click", ".button-remove-fee", function (e) { removeFee(this); });

        $("#btnAddFee").click(function () { addFee(); });
        $("#btnAddNote").click(function () { addNote(); });

        $(document).on("click", ".button-remove-note", function (e) { removeNote(this); });
        $(document).on("click", ".button-remove-amort-schedule", function (e) { removeAmortSchedule(this); });
        $(document).on("click", ".button-add-amort-schedule", function (e) { addAmortSchedule(this); });

        $("#btnAutoSetNotes").click(showCalculationDialog);
        $("#cbNeedMezzanine").click(function () {
            $("#trMezzTargetRating").fadeToggle($(this).prop("checked"));
        });
        $("#btnSave").click(function () {
            submitHandler(0);
        });
        $("#btnShowWaterfall").click(function () {
            ajaxSaveFormBefor(function () {
                 
            });

            setTimeout(submitHandler(1), 1000);
        });
        $("#btnShowAnalysisResult").click(function () {
            //$("#divScenario").dialog("open");
            //operation = 3;
            submitHandler(3);
        });
        $("#btnComplete").click(function () {
            //$("#divScenario").dialog("open");
            submitHandler(2);
        });
        $("#btnChangeRate").click(function () {
            if ($(this).prop("checked")) {
                ajaxSaveForm(function () {
                    alertify.message("数据加载中...");
                    $.ajax({
                        url: '/MyModels/GetRateSchedule',
                        type: 'get',
                        data: { modelGuid: guid },
                        dataType: 'json',
                        success: function (data) {
                            if (data.Key == 0) {
                                var code = "";
                                var list = data.Value;
                                $(list).each(function () {
                                    code = code
                                        + '<tr><td><input type="checkbox" class="confirm-change-rate" /></td>'
                                        + '<td data-code="' + this.IndexCode + '">' + this.IndexName + '</td><td>'
                                        + new Date(parseInt(this.BeginDate.replace("/Date(", "").replace(")/", ""), 10)).toLocaleDateString()
                                        + '</td><td><input class="number" readonly type="text" data-v="' + this.InterestRate + '" value="'
                                        + this.InterestRate + '" /> %</td></tr>';
                                });
                                $("#divRateSchedule table tbody").html(code);
                                $("#divRateSchedule").fadeIn()
                                $("#divScenario").dialog("option", "height", $("#divRateSchedule").height() + 220);
                            }
                            else {
                                alertify.error("数据获取失败");
                            }
                        },
                        error: function (e) {
                            alertify.error("数据获取失败");
                        }
                    });
                }, true);
            }
            else {
                $("#divScenario").dialog("option", "height", 220);
                $("#divRateSchedule").hide();
            }
        });

        $(document).on("click", ".confirm-change-rate", function (e) {
            var self = $(this);
            var tb = self.parent("td").parent("tr").find(":text");
            if (self.prop("checked")) {
                tb.prop("readonly", false);
            }
            else {
                tb.val(tb.attr("data-v"));
                tb.removeClass("modeling-field-error");
                tb.prop("readonly", true);
            }
        });
        $(document).on("change", "#divRateSchedule .number", function (e) {
            var self = $(this);
            var td = self.parent("td");
            var code = td.siblings().eq(1).attr("data-code");
            var tr = td.parent("tr");
            tr.nextAll().find("td[data-code='" + code + "']").siblings().find(":text").val(self.val());
        });

        $("#btnSetScenario").click(function () {
            submitHandler(operation);
        });
        $("#btnCalculateTrancheSize").click(function () {
            var status = $("#ProjectStatus").val();
            var projectGuid = $("#ProjectGuid").val();
            if (status == "Optimized") {
                alertify.confirm("确认", "重新测算?",
                    function () {
                        $.ajax({
                            url: '/MyProjects/UpdateProjectStatus',
                            type: 'post',
                            data: {
                                projectGuid: projectGuid, status: "ScheduleSaved"
                            },
                            dataType: 'text',
                            success: function (data) {
                                if (data == "updated") {
                                    submitHandler(4);
                                }
                            }
                        });
                    },
                    null);
            }
            else {
                submitHandler(4);
            }
        });
        $("#btnChangeTooltipVisibility").click(function () {
            changeTooltipVisibility(this);
        });
    }
    $(document).on("click", ".button-edit-fee", function (e) { editFee(this); });
    $(document).on("click", ".button-edit-note", function (e) { editNote(this); });
});