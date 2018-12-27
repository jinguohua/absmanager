const defaultAvatar = require('../assets/images/default-avatar.png');
import { chartColors } from '../components/ABSChart/chartTheme';
import qs from 'qs';
import moment from 'moment';
import ABSMessage from '../components/ABSMessage';

class CommonUtils {

  private isPlus = 0;

  /**
   * 数据格式综合处理
   * @param contents 内容
   * @param isTable 是否为null是，数据来源类型是否是table (默认：false) null=false
   * @param isPercent 是否*100（百分位） (默认：false) null=false
   * @param isThousandth 是否有千分位+‘,’  (默认：true) null=true
   * @param num 保留位数 （默认：不填时为2；如果不保留位数请填0）null=2
   * @param unit 数值转换,单位：（默认：0=【元】；4=【万】；6=【百万】；8=【亿】）null=0
   * @param showPercentSign 是否显示百分号 （默认: true)
   */
  formatContent = (
    contents: any,
    isTable?: boolean | null,
    isPercent?: boolean | null,
    isThousandth?: boolean | null,
    num?: number | null,
    unit?: 0 | 4 | 6 | 8 | null,
    showPercentage?: boolean | null,
  ) => {
    if ((contents || contents === 0) && contents.toString().length > 0) {
      let contentsNum = this.isNumber(contents);
      if (contentsNum || contentsNum === 0) {
        if (isPercent) {
          contentsNum = this.absMul(contentsNum, 100);
        }
        if (unit) {
          const pow = Math.pow(10, unit);
          contentsNum = this.absDiv(contentsNum, pow);
        }
        contentsNum = this.toFixed(contentsNum, num);
        if (isThousandth !== false ? true : isThousandth) {
          contentsNum = this.formatCurrency(contentsNum);
        }

        // 不传默认值为true
        const shouldShowPercentage = showPercentage == null ? true : showPercentage;
        // 没有传num 保留%  传了num 且 num <=2 保留%
        const showPercent = !num || num <= 2;
        if (isPercent && showPercent && shouldShowPercentage) {
          return `${contentsNum}%`;
        }
        return contentsNum;
      }
      return contents;
    }
    const nullText = '--';
    return nullText;
  }

  /**
   * 获取时间格式
   * @param value 传入的时间值
   * @param type 时间的类型，默认时候YYYY-MM-DD
   */
  formatDate(value: any, type?: '-' | '.' | null) {
    if (!value) {
      return '--';
    }
    const date = new Date(value);
    if (type) {
      return moment(date).format(`YYYY${type}MM${type}DD`);
    }
    return moment(date).format('YYYY-MM-DD');
  }

  /**
   * 内容是否为数字处理
   * @param contents 内容
   */
  isNumber(contents: any) {
    if (!isNaN(contents) && (typeof contents === 'number')) {
      return contents;
    } else if (contents === parseFloat(contents).toString()) {
      return parseFloat(contents);
    }
    return false;
  }

  /**
   * 精确度的控制
   * @param num 转换的数字
   * @param position 显示几位小数，默认的是两位小数，可以不传
   */
  toFixed(num: any, position?: number | null) {
    const positions = (!position && position !== 0) ? 2 : position;
    return num.toFixed(positions);
  }

  /**
   * 千分位加逗号
   */
  formatCurrency = (num) => {
    if (num) {
      num = num.toString().replace(/\$|\,/g, '');
      // if ('' === num || isNaN(num) || '-' === num) { return '-'; }
      var sign = num.indexOf('-') > 0 ? '-' : '';
      var cents = num.indexOf('.') > 0 ? num.substr(num.indexOf('.')) : '';
      cents = cents.length > 1 ? cents : '';
      num = num.indexOf('.') > 0 ? num.substring(0, (num.indexOf('.'))) : num;
      if ('' === cents) {
        if (num.length > 1 && '0' === num.substr(0, 1)) {
          return '-';
        }
      } else {
        if (num.length > 1 && '0' === num.substr(0, 1)) {
          return '-';
        }
      }
      for (var i = 0; i < Math.floor((num.length - (1 + i)) / 3); i++) {
        num = num.substring(0, num.length - (4 * i + 3)) + ',' + num.substring(num.length - (4 * i + 3));
      }
      return (sign + num + cents);
    }
    return num;
  }

  /**
   * 加法函数，用来得到精确的加法结果
   * 说明：javascript的加法结果会有误差，在两个浮点数相加的时候会比较明显。这个函数返回较为精确的加法结果。
   * 调用：accAdd(arg1,arg2)
   * 返回值：arg1加上arg2的精确结果
   */
  absAdd(arg1: any, arg2: any) {
    if (isNaN(arg1)) {
      arg1 = 0;
    }
    if (isNaN(arg2)) {
      arg2 = 0;
    }
    arg1 = Number(arg1);
    arg2 = Number(arg2);
    var r1, r2, m, c;
    try {
      r1 = arg1.toString().split('.')[1].length;
    } catch (e) {
      r1 = 0;
    }
    try {
      r2 = arg2.toString().split('.')[1].length;
    } catch (e) {
      r2 = 0;
    }
    c = Math.abs(r1 - r2);
    m = Math.pow(10, Math.max(r1, r2));
    if (c > 0) {
      var cm = Math.pow(10, c);
      if (r1 > r2) {
        arg1 = Number(arg1.toString().replace('.', ''));
        arg2 = Number(arg2.toString().replace('.', '')) * cm;
      } else {
        arg1 = Number(arg1.toString().replace('.', '')) * cm;
        arg2 = Number(arg2.toString().replace('.', ''));
      }
    } else {
      arg1 = Number(arg1.toString().replace('.', ''));
      arg2 = Number(arg2.toString().replace('.', ''));
    }
    return (arg1 + arg2) / m;
  }
  /**
   * 减法函数，用来得到精确的减法结果
   * 说明：javascript的减法结果会有误差，在两个浮点数相减的时候会比较明显。这个函数返回较为精确的减法结果。
   * 调用：accSub(arg1,arg2)
   * 返回值：arg1加上arg2的精确结果
   */
  absSub(arg1: any, arg2: any) {
    if (isNaN(arg1)) {
      arg1 = 0;
    }
    if (isNaN(arg2)) {
      arg2 = 0;
    }
    arg1 = Number(arg1);
    arg2 = Number(arg2);

    var r1, r2, m, n;
    try {
      r1 = arg1.toString().split('.')[1].length;
    } catch (e) {
      r1 = 0;
    }
    try {
      r2 = arg2.toString().split('.')[1].length;
    } catch (e) {
      r2 = 0;
    }
    m = Math.pow(10, Math.max(r1, r2)); // last modify by deeka //动态控制精度长度
    n = (r1 >= r2) ? r1 : r2;
    return ((arg1 * m - arg2 * m) / m).toFixed(n);
  }
  /**
   * 乘法函数，用来得到精确的乘法结果
   * 说明：javascript的乘法结果会有误差，在两个浮点数相乘的时候会比较明显。这个函数返回较为精确的乘法结果。
   * 调用：accMul(arg1,arg2)
   * 返回值：arg1乘以 arg2的精确结果
   */
  absMul(arg1: any, arg2: number) {
    if (isNaN(arg1)) {
      arg1 = 0;
    }
    if (isNaN(arg2)) {
      arg2 = 0;
    }
    arg1 = Number(arg1);
    arg2 = Number(arg2);

    var m = 0, s1 = arg1.toString(), s2 = arg2.toString();
    try {
      m += s1.split('.')[1].length;
    } catch (e) {
      // 
    }
    try {
      m += s2.split('.')[1].length;
    } catch (e) {
      // 
    }
    return Number(s1.replace('.', '')) * Number(s2.replace('.', '')) / Math.pow(10, m);
  }
  /** 
   * 除法函数，用来得到精确的除法结果
   * 说明：javascript的除法结果会有误差，在两个浮点数相除的时候会比较明显。这个函数返回较为精确的除法结果。
   * 调用：accDiv(arg1,arg2)
   * 返回值：arg1除以arg2的精确结果
   */
  absDiv(arg1: any, arg2: number) {
    if (isNaN(arg1)) {
      arg1 = 0;
    }
    if (isNaN(arg2)) {
      arg2 = 0;
    }
    arg1 = Number(arg1);
    arg2 = Number(arg2);

    let t1 = 0,
      t2 = 0,
      r1: any,
      r2: any;
    try {
      t1 = arg1.toString().split('.')[1].length;
    } catch (e) {
      // 
    }
    try {
      t2 = arg2.toString().split('.')[1].length;
    } catch (e) {
      // 
    }
    // with (Math) {
    r1 = Number(arg1.toString().replace('.', ''));
    r2 = Number(arg2.toString().replace('.', ''));
    return (r1 / r2) * Math.pow(10, t2 - t1);
    // }
  }

  getMaxHeight(row: HTMLElement[]) {
    let maxHeight = row[0].offsetHeight;
    row.forEach((col) => {
      if (col.offsetHeight > maxHeight) {
        maxHeight = col.offsetHeight;
      }
    });
    return maxHeight;
  }
  /**
   * 获取chart的series，当传送了xAction,会返回{ xList, seriesList }
   * @param data  数据
   * @param xAction x的数据处理
   * @param yAction y的数据处理
   * @param tooltip tooltip的数据处理
   * @param itemStyle 每一个数据item的处理
   */
  getChartSeries(data: any, xAction?: any, yAction?: any, tooltip?: any, itemStyle?: any) {
    if (!data || !data.line_series || data.line_series.length === 0) {
      return [];
    }
    const seriesList: any = [];
    let xList: Array<any> = [];
    this.isPlus = 0;
    if (data.line_series) {
      data.line_series.map((series, index) => {
        let dataList: any = [];
        xList = [];
        series.data.data.forEach((point) => {
          const x = xAction ? xAction(point) : Number.parseFloat(moment(point.x).format('x'));
          const y = yAction ? yAction(point) : Number.parseFloat(String(point.y));
          const tooltips = tooltip ? tooltip(point) : point.tooltip;
          xList.push(x);
          if (tooltips) {
            dataList.push({ x: x, y: y, tooltip: tooltips });
          } else {
            dataList.push([x, y]);
          }
        });
        const content = itemStyle ? itemStyle(series, index) : null;
        let seriesData = {
          name: series.name,
          type: series.type,
          data: dataList,
          step: series.step,
          stacking: series.stacking,
          dashStyle: series.dash_style,
          ...content,
        };
        seriesList.push(seriesData);
      });
    }
    if (xAction) {
      return { xList, seriesList };
    }
    return seriesList;
  }

  getMarkerStyles(series: any, index: number) {
    const colors = chartColors;
    const isAgain = String(series.name).indexOf('说明书') >= 0;
    return {
      marker: { enabled: !1 },
      color: isAgain ? colors[(this.isPlus++) % colors.length] : colors[index % colors.length]
    };
  }

  /**
   * 获取头像地址，头像不存在时使用默认头像
   * @param {string} avatar  原头像
   * @returns {string} 新头像
   */
  getAvatar(avatar: string): string {
    return !avatar ? defaultAvatar : avatar;
  }

  downloadFile(sUrl: string) {
    const isChrome = navigator.userAgent.toLowerCase().indexOf('chrome') > -1;
    const isSafari = navigator.userAgent.toLowerCase().indexOf('safari') > -1;
    // iOS devices do not support downloading. We have to inform user about this.
    if (/(iP)/g.test(navigator.userAgent)) {
      alert('Your device does not support files downloading. Please try again in desktop browser.');
      return false;
    }

    // If in Chrome or Safari - download via virtual link click
    if (isChrome || isSafari) {
      // Creating new link node.
      var link = document.createElement('a');
      link.href = sUrl;

      if (link.download !== undefined) {
        // Set HTML5 download attribute. This will prevent file from opening if supported.
        var fileName = sUrl.substring(sUrl.lastIndexOf('/') + 1, sUrl.length);
        link.download = fileName;
      }

      // Dispatching click event.
      if (document.createEvent) {
        var e = document.createEvent('MouseEvents');
        e.initEvent('click', true, true);
        link.dispatchEvent(e);
        return true;
      }
    }

    // Force file download (whether supported by server).
    if (sUrl.indexOf('?') === -1) {
      sUrl += '?download';
    }

    window.open(sUrl, '_self');
    return true;
  }

  getScenarioID() {
    const pattern = /scenario_id=(.*)/;
    const result = pattern.exec(location.hash);
    if (result) {
      return result[1];
    }
    return result;
  }

  // 电话号码验证
  getPhoneNumber(phonenumber: string, callback: any) {
    if (phonenumber && !(/^1[34578]\d{9}$/.test(phonenumber))) {
      return callback('号码格式不对');
    }
    return callback('');
  }

  getParams() {
    return qs.parse(location.hash.split('?')[1]);
  }

  /**
   * 获取当天日期
   * @returns {string} YYYY-MM-DD
   */
  getNowFormatDate() {
    return moment().format('YYYY-MM-DD');
  }

  /**
   * 旋转文字为竖直方向
   * @param {string} title
   * @param {boolean} direction 方向，true表示表示左边坐标轴，反之右坐标轴
   */
  titleReserver(title: string, direction: boolean) {
    if (direction) {
      title = title.split('').reverse().join('');
    }
    var split = title.split('');
    var deg = direction ? 90 : -90;
    var rlt: any = [];
    split.forEach(function (letter: any) {
      rlt.push('<span style="transform:rotate(' +
        deg +
        'deg);display: inline-block;">' +
        letter + '</span>');
    });
    return rlt.join('');
  }

  numberThousandFormat(val: any) {
    var re = /(?=(?!(\b))(\d{3})+$)/g;
    return val.toString().replace(re, ',');
  }

  /**
   * 转换Url（域名替换）
   * @param {string} url  原链接
   * @returns {string} 新链接
   */
  parseUrl(url: string): string {
    if (!url) { return url; }
    return url.indexOf('http') > -1 ? url : (process.env.REACT_APP_PUBLISH_PATH + url).replace('//', '/');
  }

  /**
   * 获取当前的url
   */
  getSelectedKey() {
    const url = window.location.pathname + window.location.hash;

    if (url.includes('basic-analysis')) {
      return url;
    }
    const selectedKey = url.split('?')[0];

    if (url.includes('user/follow')) {
      return selectedKey + '?type=product';
    }
    return selectedKey;
  }
  /**
   * 限制字数，超出部分以省略号...显示
   * @param {string} txt  原内容
   * @param {number} num  限制字数
   * @returns {string} 新内容
   */
  limitTxtLength(txt: string, num: number) {
    if (txt && txt.length > num) {
      txt = txt.substr(0, num) + '...';
    }
    return txt;
  }

  /**
   * 判断下载是否成功
   * @returns {object} response
   */
  downloadIsSuccess(response: any, onSuccess: (response: any) => void) {
    if (!response.is_success) {
      ABSMessage.error(response.fail_msg);
      return;
    }
    onSuccess(response);
  }

  /**
   * 使滚动容器的滚动条出现
   * @param container 滚动条所在的容器
   */
  setScrollContainerActive(container: HTMLElement) {
    setTimeout(() => {
      container.scrollLeft = 1;
    }, 500);
  }
}

export default new CommonUtils();