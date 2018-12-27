export default [
  // user
  {
    path: '/user',
    component: '../layouts/UserLayout',
    routes: [
      { path: '/user', redirect: '/user/login' },
      { path: '/user/login', component: './User/Login' },
    ],
  },
  // app
  {
    path: '/',
    component: '../layouts/BasicLayout',
    Routes: ['src/pages/Authorized'],
    //authority: ['admin', 'user'],
    routes: [
      { path: '/', redirect: '/dashboard' },
      {
        path: '/dashboard',
        name: '工作台',
        icon: 'dashboard',
      },
      {
        name: '项目管理',
        icon: 'project',
        path: '/project',
        routes: [
          {
            name: '项目列表',
            path: '/project/list',
            component: './Project/DisplayList',
          },
          {
            name: '项目详情',
            path: '/project/detail/ProInfo',
            component: '/ProDetail/ProDetail.js',
          },
        ],
      },
      {
        name: '系统设置',
        icon: 'setting',
        path: '/system',
        routes: [
          {
            name: '用户设置',
            path: '/system/user',
          },
          {
            name: '机构管理',
            path: '/system/Company',
          },
        ],
      },
      {
        name: 'account',
        icon: 'user',
        path: '/account',
        routes: [
          {
            path: '/account/center',
            name: 'center',
            component: './Account/Center/Center',
            routes: [
              {
                path: '/account/center',
                redirect: '/account/center/articles',
              },
              {
                path: '/account/center/articles',
                component: './Account/Center/Articles',
              },
              {
                path: '/account/center/applications',
                component: './Account/Center/Applications',
              },
              {
                path: '/account/center/projects',
                component: './Account/Center/Projects',
              },
            ],
          },
          {
            path: '/account/settings',
            name: 'settings',
            component: './Account/Settings/Info',
            routes: [
              {
                path: '/account/settings',
                redirect: '/account/settings/base',
              },
              {
                path: '/account/settings/base',
                component: './Account/Settings/BaseView',
              },
              {
                path: '/account/settings/security',
                component: './Account/Settings/SecurityView',
              },
              {
                path: '/account/settings/binding',
                component: './Account/Settings/BindingView',
              },
              {
                path: '/account/settings/notification',
                component: './Account/Settings/NotificationView',
              },
            ],
          },
        ],
      },
      {
        component: '404',
      },
    ],
  },
];
