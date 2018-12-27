export default {
  namespace: 'absui',
  // 初始状态
  state: {
    navigationMenus: [
      {
        url: '/filtered-list',
        icon: 'file',
        name: '筛选列表'
      },
      {
        url: '/link',
        icon: 'file',
        name: '跳转链接'
      },
      {
        url: '/progress',
        icon: 'desktop',
        name: '进度条',
        children: [],
      },
      {
        url: '/structure',
        icon: 'desktop',
        name: '结构图',
        children: [],
      },
      {
        url: '/expert-list',
        icon: 'desktop',
        name: '专家列表',
        children: [],
      },
      {
        url: '/news-list',
        icon: 'desktop',
        name: '新闻列表',
        children: [],
      },
      {
        url: '/anchor-point',
        icon: 'desktop',
        name: '简历动态',
        children: [],
      },
      {
        url: '/label-value-list',
        icon: 'desktop',
        name: '简单列表',
      },
      {
        url: '/errorpage',
        icon: 'desktop',
        name: '错误提示页面',
        children: [],
      },
      {
        url: '/countdown',
        icon: 'desktop',
        name: '倒计时',
        children: [],
      },
      {
        url: '/security-list',
        icon: 'desktop',
        name: '证券列表',
        children: [],
      },
      {
        url: '/steps',
        icon: 'desktop',
        name: '步骤',
        children: [],
      },
      {
        url: '/form-inspect',
        icon: 'file',
        name: '表单验证'
      },
      {
        url: '/switch',
        icon: 'file',
        name: '开关按钮'
      },
      {
        url: '/button',
        icon: 'paper-clip',
        name: '按钮',
        children: [],
      },
      {
        url: '/paragraph',
        icon: 'desktop',
        name: '段落',
        children: [],
      },
      {
        url: '/emphasize-text',
        icon: 'desktop',
        name: '强调的文本',
        children: [],
      },
      {
        url: '/icon',
        icon: 'smile',
        name: '图标',
        children: [],
      },
      {
        url: '/icon-text',
        icon: 'desktop',
        name: '图标+文本',
        children: [],
      },

      {
        url: '/registered-process',
        icon: 'desktop',
        name: '注册流程',
        children: [],
      },
      {
        url: '/captcha',
        icon: 'desktop',
        name: '验证码',
        children: [],
      },
      {
        url: '/panel',
        icon: 'desktop',
        name: '面板',
        children: [],
      },
      {
        url: '/simple-image',
        icon: 'desktop',
        name: '简单图片',
        children: [],
      },
      {
        url: '/avatar',
        icon: 'file',
        name: '头像',
        children: [],
      },
      {
        url: '/bubble',
        icon: 'file',
        name: '气泡',
        children: [],
      },
      {
        url: '/alert',
        icon: 'file',
        name: '页头提示',
        children: [],
      },
      {
        url: '/modal',
        icon: 'folder-open',
        name: '弹框',
        children: [],
      },
      {
        url: '/message',
        icon: 'file',
        name: '自动消失提示',
        children: [],
      },
      {
        url: '/input',
        icon: 'file',
        name: '文本框',
        children: [],
      },
      {
        url: '/select',
        icon: 'file',
        name: '下拉框',
        children: [],
      },
      {
        url: '/calendar',
        icon: 'file',
        name: '日历',
        children: [],
      },
      {
        url: '/tag',
        icon: 'file',
        name: '不可点标签',
        children: [],
      },
      {
        url: '/list',
        icon: 'file',
        name: '列表',
      },
      {
        url: '/product-page-title',
        icon: 'desktop',
        name: '内容头部',
        children: [],
      },
      {
        url: '/url-list',
        icon: 'desktop',
        name: '介绍页链接卡片',
        children: [],
      },
      {
        url: '/other-login',
        icon: 'desktop',
        name: '第三方登录',
        children: [],
      },
      {
        url: '/search-bar',
        icon: 'desktop',
        name: '搜索框',
        children: [],
      },
      {
        url: '/pagination',
        icon: 'desktop',
        name: '分页',
        children: [],
      },
      {
        url: '/upload',
        icon: 'desktop',
        name: '上传',
        children: [],
      },
      {
        url: '/radio',
        icon: 'desktop',
        name: '单选框',
        children: [],
      },
      {
        url: '/checkbox',
        icon: 'desktop',
        name: '多选框',
        children: [],
      },
      {
        url: '/tab',
        icon: 'desktop',
        name: 'Tab切换',
        children: [],
      },
      {
        url: '/comment',
        icon: 'desktop',
        name: '注释',
        children: [],
      },
      {
        url: '/no-content',
        icon: 'desktop',
        name: '暂无数据',
        children: [],
      },
      {
        url: '/upload-avatar',
        icon: 'desktop',
        name: '上传头像',
        children: [],
      },
      {
        url: '/loading',
        icon: 'desktop',
        name: '加载',
        children: [],
      },
      {
        url: '/page-title',
        icon: 'desktop',
        name: '页面头部',
        children: [],
      },
      {
        url: '/double-date-picker',
        icon: 'desktop',
        name: '时间选择',
        children: [],
      },
      {
        url: '/sub-title',
        icon: 'desktop',
        name: '子标题',
        children: [],
      },
      {
        url: '/expert-chain',
        icon: 'desktop',
        name: '专家链',
        children: [],
      }, {
        url: '/upload-file',
        icon: 'desktop',
        name: '上传附件',
        children: [],
      }, {
        url: '/file-list',
        icon: 'desktop',
        name: '文章列表',
        children: [],
      }, {
        url: '/file-header',
        icon: 'desktop',
        name: '文章标题',
        children: [],
      },
    ],
    filterData: [],
  },

  effects: {

  },

  reducers: {
    updateFilterData(state: any, { payload }: any) {
      return {
        ...state,
        filterData: payload,
      };
    },
  },
};
