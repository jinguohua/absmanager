import Request from '../../../utils/http/request';
import { ProductApi } from '../../config/api';
import defaultChart from '../../../components/ABSChart/chartData';
import {
  formatDate, formatComputationResult,
  getAPPaymentTrendChartData,
  getCOCChartData, getAPInterestRateTrendChartData
} from '../../views/product/BasicAnalysis/util';

const initialState = {
  paymentList: [],
  computationResultList: [],
  simulateDate: '',
  analyzeDate: '',
  paymentChartData: defaultChart, // 各级证券偿付图
  paymentChartLoading: true,
  assetPoolPaymentTrendChartData: defaultChart, // 资产池偿付走势图
  assetPoolPaymentTrendChartDataLoading: true,
  ICOCTrendChartData: defaultChart, // IC、OC走势图
  ICOCTrendChartDataLoading: true,
  assetPoolInterestRateTrendChartData: defaultChart, // 资产池利率分布走势图
  assetPoolInterestRateTrendChartDataLoading: true,
};

const effects = {
  *getPaymentList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.paymentList, payload);
    yield put({
      type: 'updatePaymentList',
      payload: response,
    });
  },
  *getComputationResultList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.computationResult, payload);
    if (!response) {
      return;
    }
    const simulateDate = formatDate(response.simulation_date);
    const analyzeDate = formatDate(response.analysis_date);
    yield put({
      type: 'updateSimulateDate',
      payload: simulateDate,
    });
    yield put({
      type: 'updateAnalyzeDate',
      payload: analyzeDate,
    });
    const computationResult = formatComputationResult(response.analysis_results);
    yield put({
      type: 'updateComputationResultList',
      payload: computationResult,
    });
  },
  *getAssetPoolPaymentTrendChartData({ payload }: any, { call, put }: any) {
    yield put({ type: 'updateAssetPoolPaymentTrendChartLoading', payload: true });
    const response = yield call(Request.post, ProductApi.assetPoolPaymentTrendChart, payload);
    const chartOption = getAPPaymentTrendChartData(response);
    yield put({
      type: 'updateAssetPoolPaymentTrendChartData',
      payload: chartOption,
    });
    yield put({ type: 'updateAssetPoolPaymentTrendChartLoading', payload: false });
  },
  *getICOCTrendChartData({ payload }: any, { call, put }: any) {
    yield put({ type: 'updateICOCTrendChartLoading', payload: true });
    const response = yield call(Request.post, ProductApi.ICOCTrendChart, payload);
    const chartOption = getCOCChartData(response);
    yield put({
      type: 'updateICOCTrendChartData',
      payload: chartOption,
    });
    yield put({ type: 'updateICOCTrendChartLoading', payload: false });
  },
  *getAssetPoolInterestRateTrendChartData({ payload }: any, { call, put }: any) {
    yield put({ type: 'updateAssetPoolInterestRateTrendChartLoading', payload: true });
    const response = yield call(Request.post, ProductApi.assetPoolInterestRateTrendChart, payload);
    const chartOption = getAPInterestRateTrendChartData(response);
    yield put({
      type: 'updateAssetPoolInterestRateTrendChartData',
      payload: chartOption,
    });
    yield put({ type: 'updateAssetPoolInterestRateTrendChartLoading', payload: false });
  },
};

const reducers = {
  updatePaymentList(state: any, { payload }: any) {
    return {
      ...state,
      paymentList: payload,
    };
  },
  updateComputationResultList(state: any, { payload }: any) {
    return {
      ...state,
      computationResultList: payload,
    };
  },
  updateSimulateDate(state: any, { payload }: any) {
    return {
      ...state,
      simulateDate: payload,
    };
  },
  updateAnalyzeDate(state: any, { payload }: any) {
    return {
      ...state,
      analyzeDate: payload,
    };
  },
  updateAssetPoolPaymentTrendChartData(state: any, { payload }: any) {
    return {
      ...state,
      assetPoolPaymentTrendChartData: payload,
    };
  },
  updateAssetPoolPaymentTrendChartLoading(state: any, { payload }: any) {
    return {
      ...state,
      assetPoolPaymentTrendChartDataLoading: payload,
    };
  },
  updateICOCTrendChartData(state: any, { payload }: any) {
    return {
      ...state,
      ICOCTrendChartData: payload,
    };
  },
  updateICOCTrendChartLoading(state: any, { payload }: any) {
    return {
      ...state,
      ICOCTrendChartDataLoading: payload,
    };
  },
  updateAssetPoolInterestRateTrendChartData(state: any, { payload }: any) {
    return {
      ...state,
      assetPoolInterestRateTrendChartData: payload,
    };
  },
  updateAssetPoolInterestRateTrendChartLoading(state: any, { payload }: any) {
    return {
      ...state,
      assetPoolInterestRateTrendChartDataLoading: payload,
    };
  },
};

export default { initialState, effects, reducers };