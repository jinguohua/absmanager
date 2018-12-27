import projectList from './projectList';
import IProjectListItem from './ProjectListViewModel';
import detail from './detail';

export default {
  namespace: 'project',

  state: {
    current: { fullName: 'test', shortName: 'test' } as IProjectListItem,
    ...projectList.initState,
    ...detail.initState
  },

  effects: {
    ...projectList.effects,
    ...detail.effects
  },

  reducers: {
    ...projectList.reducers,
    ...detail.reducers
  }
};
