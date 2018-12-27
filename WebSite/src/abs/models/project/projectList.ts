import Request from '../../../utils/http/request';
import { ProjectAPI } from '../../config/api';

const initState = {
  projectList: []
};

const effects = {
  *getProjectList({ payload }: any, { put, call }: any) {
    const response = yield call(Request.post, ProjectAPI.list, payload);
    yield put({
      type: 'updateProjectList',
      payload: response
    });
  }
};

const reducers = {
  updateProjectList(state: any, { payload }: any) {
    return {
      ...state,
      projectList: payload
    };
  }
};

export default { initState, effects, reducers };
