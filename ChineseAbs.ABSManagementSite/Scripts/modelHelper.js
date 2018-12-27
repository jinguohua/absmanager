{
    "BasicInformation" : [{
        "field" : "DealFullName",
        "content" : "信托专项计划或专项资产管理计划的全称。"
    }, {
        "field" : "DealName",
        "content" : "信托专项计划或专项资产管理计划在银行间市场或交易所流通时的简称。"
    }, {
        "field" : "Issuer",
        "content" : "作为受托人管理信托财产，持续披露资产和支持证券的信息，在企业资产证券化产品中是专项计划的管理人。"
    }, {
        "field" : "Originator",
        "content" : "提供被证券化的原始资产，目前如果资产是银行的贷款，原始权益人（银行）还兼做贷款的服务机构。"
    }, {
        "field" : "LeadUnderwriter",
        "content" : "销售和推广资产支持证券的主要机构。"
    }, {
        "field" : "Comment",
        "content" : "产品的备注和补充信息。"
    }
    ],
    "Schedule" : [{
        "field" : "ClosingDate",
        "content" : "证券的起息日期。"
    }, {
        "field" : "FirstPaymentDate",
        "content" : "证券的首次偿付日期。"
    }, {
        "field" : "PaymentFrequency",
        "content" : "证券端的支付频率，如果证券的支付频率不统一，则选用证券中支付频次最高的。"
    }, {
        "field" : "PaymentAtMonthEnd",
        "content" : "如果证券是月末支付，即每月最后一天为理论支付日的话，请勾选此选项。"
    }, {
        "field" : "LegalMaturity",
        "content" : "产品的法定到期日，所有支持证券的到期日都必须在这个日期之前，否则将强制赎回。"
    }, {
        "field" : "PaymentRolling",
        "content" : "支付日按照中国法定节假日的调整规则。目前常见模式是向后顺延，即支付日如果是节假日则向后顺延至第一个工作日。"
    }, {
        "field" : "DeterminationRuleType",
        "content" : "资产端现金流归集期的日期设置，分成简单和复杂两种模式，简单模式下规定资产现金流决定日相对支付日T的第T-N天, 即支付日前第N天。归集从上一个决定日到当前决定日之间的所有入池现金流。"
    }, {
        "field" : "DeterminationOffset",
        "content" : "现金流决定日相对支付日的天数，通常为负数-N，即表示在支付日前的第N天。"
    }, {
        "field" : "FirstDeterminationDateBegin",
        "content" : "首次现金流归集期的起始日期，之后按支付频率依次递归。"
    }, {
        "field" : "FirstDeterminationDateEnd",
        "content" : "首次现金流归集期的起始日期（现金流决定日），之后按支付频率依次递归。"
    }, {
        "field" : "DeterminationDateRolling",
        "content" : "归集期日期是否根据法定节假日进行调整，及调整的规则。"
    }, {
        "field" : "DeterminationDateAtMonthEnd",
        "content" : "现金流归集决定日是否是月末，即该月最后一天。"
    }
    ],
	"Fees" : [{
	    "field" : "fee-full-PaymentOrdinal",
	    "content" : "费用项整体将优先于其它任何偿付项，这里用来设置费用子项的偿付顺序，数字越小越先偿付，如果两项或多项设置成同样的优先级，则按应付金额同权支付。"
	}, {
	    "field" : "fee-full-Description",
	    "content" : "费用的文字描述。"
	}, {
	    "field" : "fee-full-IsProRated",
	    "content" : "费用计算的模式，简单模式用于直接定义金额，费率模式用于每期的费用按某种费率计算得出。"
	}, {
	    "field" : "fee-full-Name",
	    "content" : "用来显示在报告中该费用的名称。"
	}, {
	    "field" : "fee-full-FeeBasisType",
            "content" : "用于计算费用金额的基数， 费用 = 基数 x 费率。"
        }, {
            "field" : "fee-full-IsFixedRate",
            "content" : "是固定费率还是按基准利率调整的浮动费率。如果勾选此项则是固定费率。"
        }, {
            "field" : "fee-full-IsPerPaymentRate",
            "content" : "费率是否是年化费率，如果勾选此项则该费率不做年化处理，直接乘以费用基数用来得出每期的费用金额。"
        }, {
            "field" : "fee-full-FixedRate",
            "content" : "费率，此项支持百分数或者小数。例如5%或0.05。"
        }, {
            "field" : "fee-full-NeedUnpaidAccount",
            "content" : "是否要跟踪未偿金额，如果勾选此项，系统将记录累计未偿金额，当有足够金额用于偿付该费用时，将优先偿付累计未偿金额。"
        }, {
            "field" : "fee-full-NeedInterestOnUnpaidAccount",
            "content" : "如果勾选此项，累计未偿金额将计息，默认利率等同于该费用的费率。"
        }, {
            "field" : "fee-full-FixedAmount",
            "content" : "每期支付的固定金额。"
        }, {
            "field" : "fee-full-Cap",
            "content" : "费用的上限，如果没用上限则留空。"
        }, {
            "field" : "fee-full-Floor",
            "content" : "费用的下限，如果没用下限则留空。"
        }, {
            "field" : "fee-full-IsCumulativeCap",
            "content" : "勾选此项表示上限为累计上限。"
        }, {
            "field" : "fee-full-FloatingIndex",
            "content" : "费用浮动费率时的基准利率。"
        }, {
            "field" : "fee-full-Spread",
            "content" : "费用浮动费率时的利差。"
        }, {
            "field" : "fee-full-PriorityExpenseCap",
            "content" : "优先支付上限金额"
        }
    ],
    "Notes" : [{
            "field" : "note-full-PaymentOrdinal",
            "content" : "各支持证券的偿付顺序，数字越小越先偿付，如果两项或多项设置成同样的优先级，则按应付金额同权支付。支付顺序小于2的证券（例如，1 1.1 1.2)默认为优先级证券。"
        }, {
            "field" : "note-full-Name",
            "content" : "支持证券的名称（简写），该名称也将用于在报告中显示。"
        }, {
            "field" : "note-full-Description",
            "content" : "支持证券的描述。"
        }, {
            "field" : "note-full-Notional",
            "content" : "支持证券的本金。"
        }, {
            "field" : "note-full-HasAmortizationSchedule",
            "content" : "支持证券是否有固定的本金摊还时间表（摊还型）。"
        }, {
            "field" : "note-full-ExpectedMaturityDate",
            "content" : "支持证券的预计到期日。"
        }, {
            "field" : "note-full-IsFixed",
            "content" : "是否是固定利率。"
        }, {
            "field" : "note-full-IsEquity",
            "content" : "是否是次级证券，次级证券可以收取剩余现金流。"
        }, {
            "field" : "note-full-AccrualMethod",
            "content" : "支持证券计息的计日规则。"
        }, {
            "field" : "note-full-HasUpaidInterest",
            "content" : "否要跟踪证券利息未偿金额，如果勾选此项，系统将记录累计未偿金额，当有足够金额用于偿付该证券的利息时，将优先偿付累计未偿金额。。"
        }, {
            "field" : "note-full-HasInterestOnUnpaidInterest",
            "content" : "如果勾选此项，累计未偿金额将计息，默认利率等同于该证券的票面利率。"
        }, {
            "field" : "note-full-FloatingIndex",
            "content" : "证券浮动利率的基准利率。"
        }, {
            "field" : "note-full-Spread",
            "content" : "证券浮动利率的利差。"
        }, {
            "field" : "note-full-UnAdjustedAccrualPeriods",
            "content" : "当偿付日期因为节假日调整时，计息期不作调整（仍旧按照理论计息期计算），否则跟偿付日一起调整。"
        }, {
            "field" : "note-full-FixedRate",
            "content" : "证券的固定利率。"
        }, {
            "field" : "amort-schedule-item-Dates",
            "content" : "证券的预计摊还时间表的日期。"
        }, {
            "field" : "amort-schedule-item-Values",
            "content" : "证券的预计摊还时间表的本金摊还上限。"
        }
    ],
    "CollateralRule" : [{
            "field" : "HasReinvestment",
            "content" : "资产池是否设置循环购买。"
        }, {
            "field" : "ReinvestmentRule_ReinvestmentEndDate",
            "content" : "循环购买的终止日期条件，在所填日期之后将终止循环购买。内部事件的触发也将提前终止循环购买。"
        }, {
            "field" : "ReinvestmentRule_ReinvestmentRuleType",
            "content" : "资产吃循环购买时，替换资产的挑选规则。"
        }
    ],
    "CreditEnhancement" : [{
            "field" : "HasEod",
            "content" : "是否设置违约事件，违约事件由证券应付利息是否付足来触发，违约事件触发后合并资产池的现金流并改变偿付顺序优先支付级别高的证券的本息，当高级别证券本金偿付至0后，继续支付下一级证券的本息。"
        }, {
            "field" : "EodCheckHighestPriorityNoteOnly",
            "content" : "如果勾选此项，则违约事件仅有最优先级证券利息未能偿清触发，当最优先级证券本金偿付完时，次优先级证券成为最优先级证券。"
        }, {
            "field" : "EodCanCure",
            "content" : "违约事件触发后是否可被修复，如果勾选此项，则每次偿付都会判断最新的违约事件情况，否则一旦触发，状态将保持至产品终止。"
        }, {
            "field" : "HasCumlossTurbo",
            "content" : "资产池的累积违约是否会触发加速清偿。"
        }, {
            "field" : "CumlossTurboBasisType",
            "content" : "资产池累积违约率测算的基准金额类型。"
        }, {
            "field" : "CumlossTurboThreshold",
            "content" : "触发加速清偿的累积违约率阈值。"
        }, {
            "field" : "HasNoteExpectedMaturityTurbo",
            "content" : "如果勾选此项，对于所有拥有预计到期日的支持证券，在预期到期日后证券本金未被偿清将触发加速清偿。"
        }, {
            "field" : "HasRiskReserve",
            "content" : "是否设置风险储备金账户，风险储备金用于收集偿付完证券额定支付（本金和利息后）的多余现金流，来填补未来现金流的不足。"
        }, {
            "field" : "RiskReserveCap",
            "content" : "风险储备金上限。"
        }, {
            "field" : "RiskReserveInterestRate",
            "content" : "风险储备金计息利率。"
        }, {
            "field" : "RiskReserveForSeniorOnly",
            "content" : "如果勾选，则风险储备金仅用于补足优先级的摊还证券预计本息，否则将包含左右优先级证券。"
        }, {
            "field" : "HasGuarantee",
            "content" : "设置外部担保。"
        }, {
            "field" : "HasDeficiencyPayment",
            "content" : "设置差额支付。"
        }, {
            "field" : "UseLeaseMarginAsDeficiencyPayment",
            "content" : "用租赁保证金优先作差额支付。"
        }, {
            "field" : "PayWaterfallByCombinedProceeds",
            "content" : "在偿付时合并资产池本金和利息现金流。"
        }, {
            "field" : "PaySubResidualInterestBeforeOtherNotesPaidOff",
            "content" : "勾选此项时收入帐付完当期应付税费和证券利息后将支付次级收益，否则将直接转入本金帐。"
        }, {
            "field" : "LiquidateWhenEod",
            "content" : "如果勾选则一旦发生违约事件将立即清算产品。"
        }, {
            "field" : "RiskReserveInitValue",
            "content" : "在期初存入风险储备金账户的金额。"
        }
	]
}
