import Request from '../../../utils/http/request';
import { HomePageApi } from '../../config/api';

const initialState = {
  organizationList: [],
};

const effects = {
  *getOrganizationList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, HomePageApi.partnerImgs, payload);
    yield put({
      type: 'updateOrganizationList',
      payload: response,
    });
  },
};

const reducers = {
  updateOrganizationList(state: any, { payload }: any) {
    return {
      ...state,
      organizationList: payload,
    };
  },
};

export default { initialState, effects, reducers };