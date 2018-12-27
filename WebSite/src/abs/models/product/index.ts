import { ProductApi } from '../../config/api';
import Request from '../../../utils/http/request';
import security from './security';
import basicInfo from './basicInfo';
import file from './file';
import originators from './originators';
import basicAnalysis from './basicAnalysis';
import assetPool from './assetPool';
import dealList from './dealList';
import structure from './structure';
import dealListIssuing from './dealListIssuing';
import commonUtils from '../../../utils/commonUtils';

export default {
  namespace: 'product',
  // 初始状态
  state: {
    // 产品全称
    fullname: null,
    // 产品简称
    shortname: null,
    // 产品ID
    dealID: null,
    // 组织ID
    organizationID: null,
    // 证券id，默认为全部证券
    securityID: 12069,
    // 情景ID
    scenarioID: null,
    // 是否关注该产品
    isFollow: false,
    // 产品列表
    dealListObject: {},
    // 是否有下载按钮
    hasDownloadButton: false,
    // 是否显示IC,OC和资产池利息分布图
    showExtraChart: false,
    // 是否显示资产池 为false显示ABSNotSupport
    isHasAssetPool: false,
    // 是否显示资产池偿付 如果isHasAssetPool为false则 就算isHasAssetChart为true也不显示资产池偿付
    isHasAssetChart: false,
    ...security.initialState,
    ...basicInfo.initialState,
    ...file.initialState,
    ...originators.initialState,
    ...basicAnalysis.initialState,
    ...assetPool.initialState,
    ...structure.initialState,
    ...dealList.initialState,
    ...dealListIssuing.initialState,
  },

  effects: {
    *initParams({ payload }: any, { call, put }: any) {
      const params = commonUtils.getParams();
      if (params) {
        yield put({
          type: 'updateParams',
          payload: params,
        });
      }
    },
    *getDealInfo({ payload }: any, { call, put }: any) {
      const response = yield call(Request.post, ProductApi.dealCore, payload);

      yield put({
        type: 'updateDealInfo',
        payload: response,
      });
    },
    *followDeal({ payload }: any, { call, put }: any) {
      const params = {
        deal_id: payload.dealID,
      };
      const response = yield call(Request.post, ProductApi.dealFollow, params);

      yield put({
        type: 'updateDealFollow',
        payload: response,
      });
    },
    *exportDeal({ payload }: any, { call, put }: any) {
      const params = {
        deal_id: payload.dealID,
      };
      const response = yield call(Request.post, ProductApi.dealExport, params);
      commonUtils.downloadIsSuccess(response, () => {
        const url = `${ProductApi.globalDownload}?guid=${response.data}`;
        commonUtils.downloadFile(url);
      });
    },
    *getDealList({ payload }: any, { call, put }: any) {
      const params = {
        page_size: payload.pageSize,
        page_index: payload.current,
      };
      const response = yield call(Request.get, ProductApi.dealList, params);

      yield put({
        type: 'updateDealList',
        payload: response,
      });
    },
    ...security.effects,
    ...basicInfo.effects,
    ...file.effects,
    ...originators.effects,
    ...basicAnalysis.effects,
    ...structure.effects,
    ...assetPool.effects,
    ...dealList.effects,
    ...dealListIssuing.effects,
  },

  reducers: {
    updateDealFollow(state: any, { payload }: any) {
      const { isFollow } = state;

      return {
        ...state,
        isFollow: !isFollow,
        followStatus: !isFollow ? '已关注' : '关注',
      };
    },
    updateDealInfo(state: any, { payload }: any) {
      const { deal_id, originator_id, deal_full_name, originator_name, is_follow, is_has_doc, is_csrc, is_has_assetpool, is_has_assetchart } = payload;

      return {
        ...state,
        fullname: deal_full_name,
        shortname: originator_name,
        hasDownloadButton: is_has_doc,
        isFollow: is_follow,
        dealID: deal_id,
        organizationID: originator_id,
        showExtraChart: !is_csrc,
        isHasAssetPool: is_has_assetpool,
        isHasAssetChart: is_has_assetchart,
      };
    },
    updateDealList(state: any, { payload }: any) {
      return {
        ...state,
        dealListObject: payload,
      };
    },
    updateDealID(state: any, { payload }: any) {
      const { dealID } = payload;
      return {
        ...state,
        dealID: +dealID,
      };
    },
    updateOrganizationID(state: any, { payload }: any) {
      const { organizationID } = payload;
      return {
        ...state,
        organizationID: +organizationID,
      };
    },
    resetProductInfo(state: any, { payload }: any) {
      return {
        ...state,
        organizationID: null,
        dealID: null,
      };
    },
    resetParams(state: any, { payload }: any) {
      const { scenarioID } = payload;
      return {
        ...state,
        scenarioID: scenarioID,
      };
    },
    updateParams(state: any, { payload }: any) {
      return {
        ...state,
        scenarioID: payload.scenario_id ? payload.scenario_id : null,
      };
    },
    ...security.reducers,
    ...basicInfo.reducers,
    ...file.reducers,
    ...originators.reducers,
    ...basicAnalysis.reducers,
    ...assetPool.reducers,
    ...structure.reducers,
    ...dealList.reducers,
    ...dealListIssuing.reducers,
  },

  subscriptions: {
    setup({ dispatch, history }: any) {
      return history.listen(({ pathname, search }) => {
        // 基本分析子页面切换
        if (pathname === '/detail/basic-analysis') {
          const { scenario_id } = commonUtils.getParams();
          // const pattern = new RegExp('[[\\]/?=]');
          // const arr = search.split(pattern);
          // const dealID = arr[2];
          // const scenarioID = arr[4];
          dispatch({
            type: 'product/resetParams',
            payload: {
              scenarioID: scenario_id,
            },
          });
        }
      });
    },
  },
};
