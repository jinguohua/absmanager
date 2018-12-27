import { AxiosResponse } from 'axios';
import ABSMessage from '../../../components/ABSMessage';
import routeConfig from '../../../abs/config/routeConfig';
import authUtil from '../../authUtil';
class HttpHelpers {
  /**
   * 请求超时设置
   * 
   * @param {number} requestTimeout 请求超时时间
   * @returns {Promise<any>} Promise
   */
  static timeout(requestTimeout: number): Promise<any> {
    requestTimeout = requestTimeout;
    return new Promise((resolve, reject) => {
      setTimeout(() => reject(new Error('网络请求超时')), requestTimeout);
    });
  }

  // TODO:
  static checkStatus(response: any) {
    if (response.status >= 200 && response.status < 300) {
      if (response.data.isAuthorized === 1) {
        let data = response.data;
        if (data.code === 0) {
          return data.data;
        } else {
          ABSMessage.error(`处理失败： ${data.message}`);
        }
      } else {
        authUtil.removeAllCache();
        window.location.href = routeConfig.login + '?return_url=' + window.location.href;
      }
    } else {
      ABSMessage.error('服务器或网络异常，请稍后再试');
      return null;
    }

    return null;
  }

  /**
   * 处理响应数据
   * 
   * @param {AxiosResponse<any>} response 请求响应
   * @returns {Promise<any>} Promise
   */
  static parseResponse(response: AxiosResponse<any>): Promise<any> {

    return new Promise((resolve: any, reject: any): void => {
      resolve(response);
    });
  }
}

export default HttpHelpers;