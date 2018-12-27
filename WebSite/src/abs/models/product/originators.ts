import Request from '../../../utils/http/request';
import { ProductApi } from '../../config/api';
import { getColumnChartConfig } from '../../views/product/Originator/utils';

const initialState = {
  originatorDetail: {},
  participateProject: [],
  toalAsset: {},
  liabilityAssetRatio: {},
  assetYield: {},
  capitalAbundanceRatio: {},
  coreCapitalAbundanceRatio: {},
  nplRatio: {},
  toalAssetLoading: true,
  liabilityAssetRatioLoading: true,
  assetYieldLoading: true,
  capitalAbundanceRatioLoading: true,
  coreCapitalAbundanceRatioLoading: true,
  nplRatioLoading: true,
};
const effects = {
  // 基本信息
  *getOriginatorDetail({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.originatorDetail, payload);
    yield put({
      type: 'updateOriginatorDetail',
      payload: response,
    });
  },
  // 参与项目
  *getParticipateProject({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.participateProject, payload);
    yield put({
      type: 'updateParticipateProject',
      payload: response,
    });
  },
  // deal_id
  *getToalAsset({ chartInfo }: any, { call, put }: any) {
    yield put({ type: 'updateToalAssetLoading', toalAssetLoading: true});
    const response = yield call(Request.post, ProductApi.toalAsset, chartInfo);
    const chartData = getColumnChartConfig(response);
    yield put({
      type: 'updateToalAsset',
      toalAsset: chartData,
    });
    yield put({ type: 'updateToalAssetLoading', toalAssetLoading: false});
  },
  // deal_id
  *getLiabilityAssetRatio({ chartInfo }: any, { call, put }: any) {
    yield put({ type: 'updateLiabilityAssetRatioLoading', liabilityAssetRatioLoading: true});
    const response = yield call(Request.post, ProductApi.liabilityAssetRatio, chartInfo);
    const chartData = getColumnChartConfig(response);
    yield put({
      type: 'updateLiabilityAssetRatio',
      liabilityAssetRatio: chartData,
    });
    yield put({ type: 'updateLiabilityAssetRatioLoading', liabilityAssetRatioLoading: false});
  },
  // deal_id
  *getAssetYield({ chartInfo }: any, { call, put }: any) {
    yield put({ type: 'updateAssetYieldLoading', assetYieldLoading: true});
    const response = yield call(Request.post, ProductApi.assetYield, chartInfo);
    const chartData = getColumnChartConfig(response);
    yield put({
      type: 'updateAssetYield',
      assetYield: chartData,
    });
    yield put({ type: 'updateAssetYieldLoading', assetYieldLoading: false});
  },
  // deal_id
  *getCapitalAbundanceRatio({ chartInfo }: any, { call, put }: any) {
    yield put({ type: 'updateCapitalAbundanceRatioLoading', capitalAbundanceRatioLoading: true});
    const response = yield call(Request.post, ProductApi.capitalAbundanceRatio, chartInfo);
    const chartData = getColumnChartConfig(response);
    yield put({
      type: 'updateCapitalAbundanceRatio',
      capitalAbundanceRatio: chartData,
    });
    yield put({ type: 'updateCapitalAbundanceRatioLoading', capitalAbundanceRatioLoading: false});
  },
  // deal_id
  *getCoreCapitalAbundanceRatio({ chartInfo }: any, { call, put }: any) {
    yield put({ type: 'updateCoreCapitalAbundanceRatioLoading', coreCapitalAbundanceRatioLoading: true});
    const response = yield call(Request.post, ProductApi.coreCapitalAbundanceRatio, chartInfo);
    const chartData = getColumnChartConfig(response);
    yield put({
      type: 'updateCoreCapitalAbundanceRatio',
      coreCapitalAbundanceRatio: chartData,
    });
    yield put({ type: 'updateCoreCapitalAbundanceRatioLoading', coreCapitalAbundanceRatioLoading: false});
  },
  // deal_id
  *getPlRatio({ chartInfo }: any, { call, put }: any) {
    yield put({ type: 'updatePlRatioLoading', nplRatioLoading: true});
    const response = yield call(Request.post, ProductApi.nplRatio, chartInfo);
    const chartData = getColumnChartConfig(response);
    yield put({
      type: 'updatePlRatio',
      nplRatio: chartData,
    });
    yield put({ type: 'updatePlRatioLoading', nplRatioLoading: false});
  },
};

const reducers = {
  updateOriginatorDetail(state: any, { payload }: any) {
    return {
      ...state,
      originatorDetail: payload,
    };
  },
  updateParticipateProject(state: any, { payload }: any) {
    return {
      ...state,
      participateProject: payload,
    };
  },
  updateToalAsset(state: any, { toalAsset }: any) {
    return {
      ...state,
      toalAsset: toalAsset,
    };
  },
  updateToalAssetLoading(state: any, { toalAssetLoading }: any) {
    return {
      ...state,
      toalAssetLoading: toalAssetLoading,
    };
  },
  updateLiabilityAssetRatio(state: any, { liabilityAssetRatio }: any) {
    return {
      ...state,
      liabilityAssetRatio: liabilityAssetRatio,
    };
  },
  updateLiabilityAssetRatioLoading(state: any, { liabilityAssetRatioLoading }: any) {
    return {
      ...state,
      liabilityAssetRatioLoading: liabilityAssetRatioLoading,
    };
  },
  updateAssetYield(state: any, { assetYield }: any) {
    return {
      ...state,
      assetYield: assetYield,
    };
  },
  updateAssetYieldLoading(state: any, { assetYieldLoading }: any) {
    return {
      ...state,
      assetYieldLoading: assetYieldLoading,
    };
  },
  updateCapitalAbundanceRatio(state: any, { capitalAbundanceRatio }: any) {
    return {
      ...state,
      capitalAbundanceRatio: capitalAbundanceRatio,
    };
  },
  updateCapitalAbundanceRatioLoading(state: any, { capitalAbundanceRatioLoading }: any) {
    return {
      ...state,
      capitalAbundanceRatioLoading: capitalAbundanceRatioLoading,
    };
  },
  updateCoreCapitalAbundanceRatio(state: any, { coreCapitalAbundanceRatio }: any) {
    return {
      ...state,
      coreCapitalAbundanceRatio: coreCapitalAbundanceRatio,
    };
  },
  updateCoreCapitalAbundanceRatioLoading(state: any, { coreCapitalAbundanceRatioLoading }: any) {
    return {
      ...state,
      coreCapitalAbundanceRatioLoading: coreCapitalAbundanceRatioLoading,
    };
  },
  updatePlRatio(state: any, { nplRatio }: any) {
    return {
      ...state,
      nplRatio: nplRatio,
    };
  },
  updatePlRatioLoading(state: any, { nplRatioLoading }: any) {
    return {
      ...state,
      nplRatioLoading: nplRatioLoading,
    };
  },
};

export default { initialState, effects, reducers };