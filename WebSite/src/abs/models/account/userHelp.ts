import { GlobalApi } from '../../config/api';

const initialState = {
  uploadUrl: ''
};

const effects = {
  // 获取上传路径
  *getUploadUrl({  }: any, { call, put }: any) {
    const url = GlobalApi.uploadUrl;
    yield put({
      type: 'returnUploadUrl',
      uploadUrl: url
    });
  }
};

const reducers = {
  returnUploadUrl(state: any, { uploadUrl }: any) {
    return {
      ...state,
      uploadUrl: uploadUrl
    };
  }
};

export default { initialState, effects, reducers };
