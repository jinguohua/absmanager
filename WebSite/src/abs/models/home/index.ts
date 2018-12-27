
import homepageDatail from './homepageDatail';
import organizationLists from './introduction';

export default {
  namespace: 'home',
  // 初始状态
  state: {
    ...homepageDatail.initialState,
    ...organizationLists.initialState,
  },

  effects: {
    ...homepageDatail.effects,
    ...organizationLists.effects,
  },

  reducers: {
    ...homepageDatail.reducers,
    ...organizationLists.reducers,
  },
};  
