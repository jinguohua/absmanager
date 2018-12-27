import axios, { AxiosRequestConfig } from 'axios';
import helper from './helper';
import AbortBus from './abort';
import qs from 'qs';
import { RequestMethod, MIMEType } from './enum';

export default class Request {

  /**
   * 请求默认超时时间
   */
  private static readonly defaultTimeout = 100 * 1000;

  /**
   * 发送一个GET异步请求
   * 
   * @param {string} url 请求地址
   * @param {*} params 键值对(url查询参数)
   * @param {(data: any) => Promise} [successCallback] 成功时的回调
   * @param {(error: string) => Promise} [errorCallback] 错误时的回调
   * @memberof Request
   */
  static async get(url: string, params?: any) {
    const config = {
      url,
      method: RequestMethod.get,
      params,
      headers: { 'Accept': MIMEType.json },
    };

    return Request.request(config);
  }

  /**
   * 发送一个POST异步请求
   * 
   * @param {string} url 请求地址
   * @param {*} params request url的请求数据, 键值对
   * @param {*} data request body的请求数据, 键值对
   * @memberof Request
   */
  static async post(url: string, data?: any, params?: any) {
    const config = {
      url,
      method: RequestMethod.post,
      params,
      data: qs.stringify(data),
      headers: {
        'Accept': MIMEType.json,
        'Content-Type': MIMEType.form,
      },
    };

    return Request.request(config);
  }

  static async postJson(url: string, data?: any, params?: any) {
    const config = {
      url,
      method: RequestMethod.post,
      params,
      data: data,
      headers: {
        'Accept': MIMEType.json,
        'Content-Type': MIMEType.json,
      },
    };

    return Request.request(config);
  }

  /**
   * 发送一个POST异步请求
   * 
   * @param {string} url 请求地址
   * @param {*} params request url的请求数据, 键值对
   * @param {*} data request body的请求数据, 键值对
   * @memberof Request
   */
  static async postFile(url: string, params?: any, data?: any) {
    const defaultConfig = {
      url: url,
      method: RequestMethod.post,
      params: params,
      data: data,
      headers: {
        'Accept': MIMEType.json,
        'Content-Type': MIMEType.json,
      },
    };

    return Request.request(defaultConfig);
  }

  /**
   * 发送一个异步请求
   * 
   * @param {AxiosRequestConfig} config Axios请求配置
   */
  private static async request(config: AxiosRequestConfig) {
    const defaultConfig: AxiosRequestConfig = {
      timeout: Request.defaultTimeout,
      withCredentials: true,
      cancelToken: AbortBus.cancelToken,
      ...config,
    };

    const timeout = defaultConfig.timeout || Request.defaultTimeout;
    let asyncResult = axios.request(defaultConfig)
      .then(helper.checkStatus)
      .then(response => helper.parseResponse(response));

    return Promise.race([helper.timeout(timeout), asyncResult]);
  }
}