import request from '@/utils/request';

export async function fetchProjects(query) {
  const option = {
    method: 'post',
    body: {
      pageIndex: query.pageindex != null ? query.pageindex : 1,
      pageSize: query.pagesize != null ? query.pagesize : 5,
      sortField: query.sortField != null ? query.sortField : '',
      sortType: query.sortType != null ? query.sortField : '',
      filters: query.filters,
    },
  };
  return request('/api/project/list', option);
}

export function remove(id) {
  return request(`api/project/delete/${id}`, { method: 'post' });
}
