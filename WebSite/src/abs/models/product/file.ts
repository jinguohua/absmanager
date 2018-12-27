import Request from '../../../utils/http/request';
import { ProductApi } from '../../config/api';

const initialState = {
  fileList: [],
  fileurl: {},
};

const effects = {
  *getFileList({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.fileList, payload);
    yield put({
      type: 'updateFileList',
      payload: response,
    });
  },
  *getFileListDownload({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, ProductApi.fileListDownload, payload);
    return response;
    // yield put({
    //   type: 'updateFileListDownload',
    //   payload: response,
    // });
  },
};

const reducers = {
  updateFileList(state: any, { payload }: any) {
    return {
      ...state,
      fileList: payload,
    };
  },
  // updateFileListDownload(state: any, { payload }: any) {
  //   return {
  //     ...state,
  //     fileurl: payload,
  //   };
  // },
};

export default { initialState, effects, reducers };