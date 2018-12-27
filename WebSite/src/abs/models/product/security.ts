import Request from '../../../utils/http/request';
import { ProductApi } from '../../config/api';

const initialState = {
  securityList: [],
  securityID: null,
};

const effects = {
  *getSecurityList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.securityList, payload);
    yield put({
      type: 'updateSecurityList',
      payload: response,
    });
  }
};

const reducers = {
  updateSecurityList(state: any, { payload }: any) {
    return {
      ...state,
      securityList: payload,
    };
  },
  deleteSecurity(state: any, { payload }: any) {
    const { securityList } = state;
    const { row_id } = payload;
    return {
      ...state,
      securityList: securityList.filter((security) => security.id !== row_id),
    };
  }
};

export default { initialState, effects, reducers };