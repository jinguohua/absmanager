import Request from '../../../utils/http/request';
import { ProductApi } from '../../config/api';

const initialState = {
  structureImage: '',
  triggerEventList: [],
  paymentModel: {},
};

const effects = {
  *getStructureImage({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.structureImage, payload);
    yield put({
      type: 'updateStructureImage',
      payload: response,
    });
  },
  *getTriggerEventList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.triggerEventList, payload);
    yield put({
      type: 'updateTriggerEventList',
      payload: response,
    });
  },
  *getPaymentModel({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.paymentModel, payload);
    yield put({
      type: 'updatepaymentModel',
      payload: response,
    });
  }
  
};

const reducers = {
  updateStructureImage(state: any, { payload }: any) {
    return {
      ...state,
      structureImage: payload,
    };
  },
  updateTriggerEventList(state: any, { payload }: any) {
    return {
      ...state,
      triggerEventList: payload,
    };
  },
  updatepaymentModel(state: any, { payload }: any) {
    return {
      ...state,
      paymentModel: payload,
    };
  }
};

export default { initialState, effects, reducers };