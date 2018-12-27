import Request from '../../../utils/http/request';
import { ProductApi, GlobalApi } from '../../config/api';
import { formatCycleData } from '../../views/product/List/DealListIssuing/data';

const initialState = {
  dealIssuingList: [],
  globalFilterList: [],
  globalDownload: [],
  dealPipelineScheduleList: []
};

const effects = {
  *getDealIssuingList({ payload }: any, { call, put }: any) {
    const response = yield call(
      Request.post,
      ProductApi.dealPipelinePager,
      null,
      payload
    );
    yield put({
      type: 'updateDealIssuingList',
      payload: response
    });
  },
  *getFilterList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, GlobalApi.filter, null, payload);
    yield put({
      type: 'updateFilterList',
      payload: response
    });
  },
  *getDealPipelineExport({ payload }: any, { call, put }: any) {
    yield call(Request.post, ProductApi.dealPipelineExport, null, payload);
  },

  *getDealPipelineScheduleList({ payload }: any, { call, put }: any) {
    const response = yield call(
      Request.post,
      ProductApi.dealPipelineSchedule,
      null,
      payload
    ); 
    const productCycle = formatCycleData(response);
    yield put({
      type: 'updateDealPipelineScheduleList',
      payload: productCycle
    });
  }
};

const reducers = {
  updateDealIssuingList(state: any, { payload }: any) {
    return {
      ...state,
      dealIssuingList: payload
    };
  },
  updateFilterList(state: any, { payload }: any) {
    return {
      ...state,
      globalFilterList: payload
    };
  },
  updateDealPipelineScheduleList(state: any, { payload }: any) {
    return {
      ...state,
      dealPipelineScheduleList: payload
    };
  }
};

export default { initialState, effects, reducers };
