import Request from '../../../utils/http/request';
import { SystemAPI } from '../../config/api';

const initState = {
  userList: []
};

const effects = {
  *getUserList({ payload }: any, { put, call }: any) {
    const response = yield call(Request.get, SystemAPI.userList, payload);
    yield put({
      type: 'updateUserList',
      payload: response
    });
  }
};

const reducers = {
  updateUserList(state: any, { payload }: any) {
    return {
      ...state,
      userList: payload
    };
  }
};

export default { initState, effects, reducers };
