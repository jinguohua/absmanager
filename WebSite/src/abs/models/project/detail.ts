import Request from '../../../utils/http/request';
import { ProjectAPI } from '../../config/api';

const initState = {
  projectId: 0
};

const effects = {
  *getProjectBaseInfo({ payload: { id } }: any, { put, call }: any) {
    const response = yield call(Request.get, ProjectAPI.baseInfo, { id: id });
    yield put({
      type: 'updateProject',
      payload: response
    });
  }
};

const reducers = {
  updateProject(state: any, { payload }: any) {
    const { id, name, shortName } = payload;

    return {
      ...state,
      projectId: id,
      current: { fullName: name, shortName: shortName }
    };
  }
};

export default { initState, effects, reducers };
