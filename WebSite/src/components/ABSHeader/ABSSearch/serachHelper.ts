
import routeConfig from '../../../abs/config/routeConfig';

interface IResult {
  title: string;
  description: string;
  url: string;
}

interface ISearchResult {
  type: string;
  results: Array<IResult>;
}

/**
 * 菜单帮助类
 */
class Menuhelper {
  /**
   * 搜索结果格式转换
   * @param {any} data 数据源
   * @returns {Array<ISearchResult>} 转换结果
   */
  static parseResults = (data: any) => {
    let results: Array<ISearchResult> = [];

    if (!data) {
      return results;
    }

    if (data.securities && data.securities.length > 0) {
      let resultItem: ISearchResult = {
        type: '证券',
        results: data.securities.map((item) => {
          return {
            title: item.deal_short_name + ' ' + item.short_name,
            description: item.code ? '证券代码：' + item.code : '暂无代码',
            url: `${routeConfig.investmentSecurityInfo}${item.id}`
          };
        })
      };
      results.push(resultItem);
    }

    if (data.deals && data.deals.length > 0) {
      let resultItem: ISearchResult = {
        type: '产品',
        results: data.deals.map((item) => {
          return {
            title: item.short_name,
            description: item.full_name,
            url: `${routeConfig.productDealInfo}?deal_id=${item.id}`
          };
        })
      };

      results.push(resultItem);
    }

    if (data.organizations && data.organizations.length > 0) {
      let resultItem: ISearchResult = {
        type: '机构',
        results: data.organizations.map((item) => {
          return {
            title: item.short_name,
            description: item.full_name,
            url: `${routeConfig.organizationDetail}${item.id}`
          };
        })
      };

      results.push(resultItem);
    }

    // if (data.functions && data.functions.length > 0) {
    //   let resultItem: ISearchResult = {
    //     type: '功能',
    //     results: data.functions.map((item) => {
    //       return {
    //         title: item.name,
    //         url: item.url
    //       };
    //     })
    //   };

    //   results.push(resultItem);
    // }

    return results;
  }
}

export default Menuhelper;
