import Request from '../../../utils/http/request';
import { ProductApi, GlobalApi } from '../../config/api';
import { formatProductCycleData, formatExpertListData, formatOrganizationData, getLineChartConfig } from '../../views/product/BasicInfo/util';
import ProductDataConverter from '../../views/product/BasicInfo/Tab/utils/ProductDataConverter';

const initialState = {
  detail: {},
  paymentChart: {},
  paymentChartLoading: true,
  structureChart: {},
  structureList: {},
  productCycleList: [],
  expertList: [],
  organizationList: [],
  activeTabKey: '1',
  selectOptions: {
    roleOptions: [], // 参与角色
    dutyOptions: [], // 个人职责
  },
  project: null, // abs 项目
};

const effects = {
  // deal_id
  *getDetail({ dealInfo }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.detail, dealInfo);
    yield put({
      type: 'updateDetail',
      detail: response,
    });
    const productCycle = formatProductCycleData(response);
    yield put({
      type: 'updateProductCycleList',
      payload: productCycle,
    });
    const expertList = formatExpertListData(response);
    yield put({
      type: 'updateExpertList',
      payload: expertList,
    });
    const organizations = formatOrganizationData(response);
    yield put({
      type: 'updateOrganizationList',
      payload: organizations,
    });
    // const resBaseInfo = response ? response.basic_info : null;
    const project = null; // ProductDataConverter.convertABSProject(dealInfo, resBaseInfo);
    if (!project) { return; }
    yield put({
      type: 'updateProject',
      payload: project,
    });
  },
  // deal_id 
  *getPaymentChart({ dealInfo }: any, { call, put }: any) {
    yield put({
      type: 'updatePaymentChartLoading',
      paymentChartLoading: true,
    });
    const response = yield call(Request.post, ProductApi.paymentChart, dealInfo);
    const paymentChartData = getLineChartConfig(response);
    yield put({
      type: 'updatePaymentChart',
      paymentChart: paymentChartData,
    });
    yield put({
      type: 'updatePaymentChartLoading',
      paymentChartLoading: false,
    });
  },
  // deal_id  security_id
  *getStructureChart({ dealInfo }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.structureChart, dealInfo);
    yield put({
      type: 'updateStructureChart',
      structureChart: response,
    });
  },
  // deal_id
  *getStructureList({ dealInfo }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.securityList, dealInfo);
    yield put({
      type: 'updateStructureList',
      structureList: response,
    });
  },
  // 获取参与角色、个人职责选项
  *getABSProjectSelectData({ payload }: any, { call, put }: any) {
    const ABSProjectType = 8;
    const response = yield call(Request.post, GlobalApi.filter, { filter_type_id: ABSProjectType });
    const selectOptions =  ProductDataConverter.convertSelectData(response);
    yield put({
      type: 'updateSelectOptions',
      payload: selectOptions,
    });
  },
};

const reducers = {
  updateDetail(state: any, { detail }: any) {
    return {
      ...state,
      detail: detail,
    };
  },
  updatePaymentChart(state: any, { paymentChart }: any) {
    return {
      ...state,
      paymentChart: paymentChart,
    };
  },
  updatePaymentChartLoading(state: any, { paymentChartLoading }: any) {
    return {
      ...state,
      paymentChartLoading: paymentChartLoading,
    };
  },
  updateStructureChart(state: any, { structureChart }: any) {
    return {
      ...state,
      structureChart: structureChart,
    };
  },
  updateStructureList(state: any, { structureList }: any) {
    return {
      ...state,
      structureList: structureList,
    };
  },
  updateProductCycleList(state: any, { payload }: any) {
    return {
      ...state,
      productCycleList: payload,
    };
  },
  updateExpertList(state: any, { payload }: any) {
    return {
      ...state,
      expertList: payload,
    };
  },
  updateOrganizationList(state: any, { payload }: any) {
    return {
      ...state,
      organizationList: payload,
    };
  },
  changeTab(state: any, { payload }: any) {
    return {
      ...state,
      activeTabKey: payload,
    };
  },

  updateSelectOptions(state: any, { payload }: any) {
    return {
      ...state,
      selectOptions: payload,
    };
  },
  updateProject(state: any, { payload }: any) {
    return {
      ...state,
      project: payload,
    };
  }
};

export default { initialState, effects, reducers };