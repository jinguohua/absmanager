import modifyPwd from './modifyPwd';
import { AccountApi } from '../../config/api';
import Request from '../../../utils/http/request';
import userHelp from './userHelp';

export default {
  namespace: 'account',

  state: {
    captcha: '',
    ...modifyPwd.initialState,
    ...userHelp.initialState
  },

  effects: {
    *login({ payload }: any, { call, put }: any) {
      return yield call(Request.post, AccountApi.login, payload);
    },
    ...modifyPwd.effects,
    ...userHelp.effects
  },

  reducers: {
    updateCaptcha(state: any, { payload }: any) {
      return {
        ...state,
        captcha: payload
      };
    },
    ...modifyPwd.reducers,
    ...userHelp.reducers
  }
};
