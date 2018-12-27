import Request from '../../../utils/http/request';
import { ProductApi } from '../../config/api';

const initialState = {
    distributeList: [],
    riskList: [],
    changeList: [],
    assetDetail: [],
    paymentChartList: {},
    paymentChartLoading: true,
    assetPrepaymentChart: {},
    assetDefaultChart: {},
    assetPrepaymentChartLoading: true,
    assetDefaultChartLoading: true,
};

const effects = {
    *getDistributeList({ payload }: any, { call, put }: any) {
        const response = yield call(Request.post, ProductApi.assetDistribute, payload);
        yield put({
            type: 'updateDistributeList',
            payload: response,
        });
    },
    *getAssetRiskList({ payload }: any, { call, put }: any) {
        const response = yield call(Request.post, ProductApi.assetRiskList, payload);
        yield put({
            type: 'updateAssetRiskList',
            payload: response,
        });
    },
    *getAssetChangeList({ payload }: any, { call, put }: any) {
        const response = yield call(Request.post, ProductApi.assetChangeList, payload);
        yield put({
            type: 'updateAssetChangeList',
            payload: response,
        });
    },
    *getAssetDetail({ payload }: any, { call, put }: any) {
        const response = yield call(Request.post, ProductApi.assetDetail, payload);
        yield put({
            type: 'updateAssetDetail',
            payload: response,
        });
    },
    *getAssetPaymentChart({ payload }: any, { call, put }: any) {
        yield put({ type: 'updateAssetPaymentChartLoading', payload: true });
        const response = yield call(Request.post, ProductApi.assetPaymentChart, payload);
        yield put({
            type: 'updateAssetPaymentChart',
            payload: response,
        });
        yield put({ type: 'updateAssetPaymentChartLoading', payload: false });
    },
    *getAssetPrepaymentChart({ payload }: any, { call, put }: any) {
        yield put({ type: 'updateAssetPrepaymentChartLoading', assetPrepaymentChartLoading: true });
        const response = yield call(Request.post, ProductApi.assetPrepaymentChart, payload);
        yield put({
        type: 'updateAssetPrepaymentChart',
        payload: response,
        });
        yield put({ type: 'updateAssetPrepaymentChartLoading', assetPrepaymentChartLoading: false });
    },
    *getAssetDefaultChart({ payload }: any, { call, put }: any) {
        yield put({ type: 'updateAssetDefaultChartLoading', assetDefaultChartLoading: true });
        const response = yield call(Request.post, ProductApi.assetDefaultChart, payload);
        yield put({
            type: 'updateAssetDefaultChart',
            payload: response,
        });
        yield put({ type: 'updateAssetDefaultChartLoading', assetDefaultChartLoading: false });
    },
};

const reducers = {
    updateDistributeList(state: any, { payload }: any) {
        return {
            ...state,
            distributeList: payload,
        };
    },
    updateAssetRiskList(state: any, { payload }: any) {
        return {
            ...state,
            riskList: payload,
        };
    },
    updateAssetChangeList(state: any, { payload }: any) {
        return {
            ...state,
            changeList: payload,
        };
    },
    updateAssetDetail(state: any, { payload }: any) {
        return {
            ...state,
            assetDetail: payload,
        };
    },
    updateAssetPaymentChart(state: any, { payload }: any) {
        return {
            ...state,
            paymentChartList: payload,
        };
    },
    updateAssetPaymentChartLoading(state: any, { payload }: any) {
        return {
            ...state,
            paymentChartLoading: payload,
        };
    },
    updateAssetPrepaymentChart(state: any, { payload }: any) {
        return {
            ...state,
            assetPrepaymentChart: payload,
        };
    },
    updateAssetPrepaymentChartLoading(state: any, { payload }: any) {
        return {
            ...state,
            assetPrepaymentChartLoading: payload,
        };
    },
    updateAssetDefaultChart(state: any, { payload }: any) {
        return {
            ...state,
            assetDefaultChart: payload,
        };
    },
    updateAssetDefaultChartLoading(state: any, { payload }: any) {
        return {
            ...state,
            assetDefaultChartLoading: payload,
        };
    },
};

export default { initialState, effects, reducers };