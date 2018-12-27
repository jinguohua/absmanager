import Request from '../../../utils/http/request';
import { ProductApi, GlobalApi } from '../../config/api';

const initialState = {
  dealSecurityList: [],
  dealProductList: [],
  globalFilterList: [],
  globalDownload: []
};

const effects = {
  *getDealSecurityList({ payload }: any, { call, put }: any) {
    const response = yield call(
      Request.post,
      ProductApi.dealSecurityListPager,
      null,
      payload
    );
    yield put({
      type: 'updateDealSecurityList',
      payload: response
    });
  },
  *getDealSecurityListExport({ payload }: any, { call, put }: any) {
    yield call(Request.post, ProductApi.dealSecurityListExport, null, payload);
  },
  *getDealProductList({ payload }: any, { call, put }: any) {
    const response = yield call(
      Request.post,
      ProductApi.dealListPager,
      null,
      payload
    );
    yield put({
      type: 'updateDealProductList',
      payload: response
    });
  },
  *getDealProductListExport({ payload }: any, { call, put }: any) {
    yield call(Request.post, ProductApi.dealListExport, null, payload);
  },
  *getFilterLists({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, GlobalApi.filter, null, payload);
    yield put({
      type: 'updateFilterList',
      payload: response
    });
  }
};

const reducers = {
  updateDealSecurityList(state: any, { payload }: any) {
    return {
      ...state,
      dealSecurityList: payload
    };
  },
  updateDealProductList(state: any, { payload }: any) {
    return {
      ...state,
      dealProductList: payload
    };
  },
  updateFilterList(state: any, { payload }: any) {
    return {
      ...state,
      globalFilterList: payload
    };
  }
};

export default { initialState, effects, reducers };
