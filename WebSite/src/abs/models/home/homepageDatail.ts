import Request from '../../../utils/http/request';
import { HomePageApi } from '../../config/api';
// import ABSMessage from '../../../components/ABSMessage';

const initialState = {
  homePageDetail: {},
};

const effects = {
  *getHomePageDetail({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, HomePageApi.homePage, payload);
    yield put({
      type: 'updateHomePageDetail',
      payload: response,
    });
  },
};

const reducers = {
  updateHomePageDetail(state: any, { payload }: any) {
    return {
      ...state,
      homePageDetail: payload,
    };
  },
};

export default { initialState, effects, reducers };