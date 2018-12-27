import userList from './userList';

export default {
  namespace: 'system',

  state: {
    ...userList.initState,
  },

  effects: {
    ...userList.effects
  },

  reducers: {
    ...userList.reducers
  }
};
