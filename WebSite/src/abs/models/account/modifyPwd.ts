import Request from '../../../utils/http/request';
import { AccountApi } from '../../config/api';
const initialState = {
};

const effects = {
  *getModifyPwd({ payload }: any, { call, put }: any) {
    const response = yield call(Request.post, AccountApi.modifypwd, payload);
    return response;

  },
};

const reducers = {

};

export default { initialState, effects, reducers };