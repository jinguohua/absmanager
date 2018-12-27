export const menusData = {
  siteMenus: [
    {
      id: 218,
      name: "工作台",
      key: "HomePage",
      url: "/index.html#/main/home",
      isNew: false,
      isOpen: true,
      isFree: true,
      parentId: 92,
      children: []
    },
    {
      id: 93,
      name: "项目管理",
      key: "Projects",
      url: null,
      isNew: false,
      isOpen: true,
      isFree: true,
      parentId: 92,
      children: [
        {
          id: 94,
          name: "项目中心",
          key: "ProjectCeneter",
          url: null,
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 93,
          children: [
            {
              id: 98,
              name: "项目列表",
              key: "ProjectList",
              url: "/projects.html#/projects/list",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            },
            {
              id: 98,
              name: "项目维护",
              key: "ProjectEdit",
              url: "/projects.html#/project/base",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            },
            {
              id: 98,
              name: "证券维护",
              key: "ProjectNoteEdit",
              url: "/projects.html#/project/notes",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            },
            {
              id: 98,
              name: "费用管理",
              key: "ProjectFeeEdit",
              url: "/projects.html#/project/fees",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            },
            {
              id: 98,
              name: "账户管理",
              key: "ProjectAccountEdit",
              url: "/projects.html#/project/accounts",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            }
          ]
        }
      ]
    },
    {
      id: 93,
      name: "存续期管理",
      key: "ProductCenter",
      url: null,
      isNew: false,
      isOpen: true,
      isFree: true,
      parentId: 92,
      children: [
        {
          id: 94,
          name: "产品中心",
          key: "ProductList",
          url: "",
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 93,
          children: [
            {
              id: 94,
              name: "证券偿付计算",
              key: "ProductPayments",
              url: "/projects.html#/duration/paymentcal",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            },
            {
              id: 94,
              name: "证券偿付反馈",
              key: "ProductPaymentResult",
              url: "/projects.html#/duration/payment",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 93,
              children: []
            }
          ]
        }
      ]
    },
    {
      id: 86,
      name: "资产管理",
      key: "AssetModule",
      url: null,
      isNew: false,
      isOpen: true,
      isFree: true,
      parentId: 92,
      children: [
        {
          id: 203,
          name: "资产池",
          key: "AssetPool",
          url: null,
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 86,
          children: [
            {
              id: 204,
              name: "资产列表",
              key: "AssetList",
              url: "/assets.html#/list/assets",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            },
            {
              id: 204,
              name: "筛选模板",
              key: "AssetFilter",
              url: "/assets.html#/list/templates",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            }
          ]
        },
        {
          id: 203,
          name: "资产包",
          key: "AssetPackage",
          url: null,
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 86,
          children: [
            {
              id: 204,
              name: "列表",
              key: "AssetPackageList",
              url: "/assets.html#/list/packages",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            },
            {
              id: 204,
              name: "详情",
              key: "AssetPackageDetail",
              url: "/assets.html#/detail/package",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            }
          ]
        },
        {
          id: 203,
          name: "资产偿付",
          key: "AssetRepaymentCenter",
          url: null,
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 86,
          children: [
            {
              id: 204,
              name: "正常偿付",
              key: "AssetRepayment",
              url: "/assets.html#/list/payments",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            },
            {
              id: 204,
              name: "早偿",
              key: "AssetPrepayment",
              url: "/assets.html#/list/prepayment",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            },
            {
              id: 204,
              name: "违约",
              key: "AssetDefault",
              url: "/assets.html#/list/default",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            },
            {
              id: 204,
              name: "跑批",
              key: "AssetCalculate",
              url: "/assets.html#/list/batch",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 203,
              children: []
            }
          ]
        }
      ]
    },
    {
      id: 80,
      name: "投资者平台",
      key: "InvestCenter",
      url: null,
      isNew: false,
      isOpen: true,
      isFree: true,
      parentId: 92,
      children: [
        {
          id: 95,
          name: "产品中心",
          key: "InvestProducts",
          url: null,
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 80,
          children: [
            {
              id: 14,
              name: "产品列表",
              key: "InvestProductList",
              url: "/product.html#/list/product",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 95,
              children: []
            },
            {
              id: 16,
              name: "产品详情",
              key: "InvestProductInfo",
              url: "/product.html#/detail/basic-info",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 95,
              children: []
            },
            {
              id: 15,
              name: "证券列表",
              key: "InvestSecurityList",
              url: "/product.html#/list/security",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 95,
              children: []
            }
          ]
        }
      ]
    },
    {
      id: 84,
      name: "系统管理",
      key: "SystemManagement",
      url: null,
      isNew: false,
      isOpen: true,
      isFree: true,
      parentId: 92,
      children: [
        {
          id: 96,
          name: "功能管理",
          key: "SystemManagement1",
          url: null,
          isNew: false,
          isOpen: true,
          isFree: true,
          parentId: 84,
          children: [
            {
              id: 28,
              name: "用户管理",
              key: "UserManagement",
              url: "/system.html#/list/user",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 96,
              children: []
            },
            {
              id: 90,
              name: "机构管理",
              key: "Organization",
              url: "/system.html#/list/organization",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 96,
              children: []
            },
            {
              id: 90,
              name: "菜单管理",
              key: "System_Menus",
              url: "/system.html#/list/menu",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 96,
              children: []
            },
            {
              id: 90,
              name: "列表项维护",
              key: "CodeItems",
              url: "/system.html#/list/codeitem",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 96,
              children: []
            },
            {
              id: 28,
              name: "日历管理",
              key: "CalendarManager",
              url: "/system.html#/calendar",
              isNew: false,
              isOpen: true,
              isFree: true,
              parentId: 96,
              children: []
            }
          ]
        }
      ]
    }
  ],
  personMenu: {
    id: 115,
    name: "个人中心",
    key: "PersonCenter",
    url: null,
    isNew: false,
    isOpen: true,
    isFree: true,
    parentId: 92,
    children: [
      {
        id: 233,
        name: "用户信息",
        key: "Profile",
        url: "/account.html#/user/profile",
        isNew: false,
        isOpen: true,
        isFree: true,
        parentId: 115,
        children: []
      },
      {
        id: 234,
        name: "修改密码",
        key: "ResetPwd",
        url: "/account.html#/user/modify-password",
        isNew: false,
        isOpen: true,
        isFree: true,
        parentId: 115,
        children: []
      }
    ]
  },
  navigationMenus: [
    {
      id: 3,
      name: "项目维护",
      key: "ProjectEdit",
      url: null,
      icon: null,
      parentId: 1,
      siteMenuKey: null,
      children: [
        {
          id: 7,
          name: "项目信息",
          key: "Project.BaseInfo",
          url: "/projects.html#/project/base",
          icon: "mission",
          parentId: 3,
          siteMenuKey: "ProjectEdit",
          children: []
        },
        {
          id: 9,
          name: "证券信息",
          key: "Project.Notes",
          url: "/projects.html#/project/notes",
          icon: "department-m",
          parentId: 3,
          siteMenuKey: "ProjectEdit",
          children: []
        },
        {
          id: 10,
          name: "账号信息",
          key: "Project.Accounts",
          url: "/projects.html#/project/accounts",
          icon: "project",
          parentId: 3,
          siteMenuKey: "ProjectEdit",
          children: []
        },
        {
          id: 17,
          name: "费用信息",
          key: "Project.Fees",
          url: "/projects.html#/project/fees",
          icon: "progress",
          parentId: 3,
          siteMenuKey: "ProjectEdit",
          children: []
        },
        {
          id: 11,
          name: "底层资产",
          key: "Project.AssetPackage",
          url: "/projects.html#/project/assets",
          icon: "budget",
          parentId: 3,
          siteMenuKey: "ProjectEdit",
          children: []
        }
      ]
    },
    {
      id: 23,
      name: "投资",
      key: "Investment",
      url: null,
      icon: null,
      parentId: 1,
      siteMenuKey: "Investment",
      children: [
        {
          id: 28,
          name: "证券信息",
          key: "Investment.Security",
          url: "/investment.html#/security/info",
          icon: "mission-regular",
          parentId: 23,
          siteMenuKey: "SecurityInfo",
          children: []
        },
        {
          id: 26,
          name: "交易定价",
          key: "Investment.Pring",
          url: "/investment.html#/security/pricing",
          icon: "process",
          parentId: 23,
          siteMenuKey: "SecurityInfo",
          children: []
        },
        {
          id: 29,
          name: "收益率",
          key: "Investment.YiledRate",
          url: "/investment.html#/security/yield-rate",
          icon: "chart-BL",
          parentId: 23,
          siteMenuKey: "SecurityInfo",
          children: []
        },
        {
          id: 30,
          name: "现金流",
          key: "Investment.Cashflow",
          url: "/investment.html#/security/cashflow",
          icon: "budget-plus",
          parentId: 23,
          siteMenuKey: "SecurityInfo",
          children: []
        },
        {
          id: 31,
          name: "情景分析",
          key: "Investment.ScenarioAnalysis",
          url: "/investment.html#/security/scenario-analysis",
          icon: "assessment",
          parentId: 23,
          siteMenuKey: "SecurityInfo",
          children: []
        },
        {
          id: 32,
          name: "情景设置",
          key: "Investment.ScenarioSetting",
          url: "/investment.html#/security/scenario-setting",
          icon: "mission-manage",
          parentId: 23,
          siteMenuKey: "SecurityInfo",
          children: []
        }
      ]
    },
    {
      id: 45,
      name: "个人中心",
      key: "Account",
      url: null,
      icon: null,
      parentId: 1,
      siteMenuKey: null,
      children: [
        {
          id: 46,
          name: "账户信息",
          key: "Account.Profile",
          url: "/account.html#/user/profile",
          icon: "rectangle-box",
          parentId: 45,
          siteMenuKey: "Profile",
          children: []
        },
        {
          id: 47,
          name: "修改密码",
          key: "Account.ResetPwd",
          url: "/account.html#/user/modify-password",
          icon: "write-mail",
          parentId: 45,
          siteMenuKey: "ResetPwd",
          children: []
        }
      ]
    }
  ]
};
