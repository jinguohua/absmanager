import { ISelectOption } from '../SelectField';

/**
 * Select数据助手
 */
export default class SelectDataHelper {
  /**
   * 获取选择器默认选中值
   * @param data 
   */
  static getDefaultValue(data: ISelectOption[]): string {
    if (!Array.isArray(data) || data.length <= 0) { return ''; }
    for (const item of data) {
      const { value, selected } = item;
      if (selected) { return value; }
    }
    const firstItem = data[0];
    const { value: fValue } = firstItem;
    return fValue;
  }

  static getFirstElement(data: ISelectOption[]): string {
    if (!Array.isArray(data) || data.length <= 0) { return ''; }
    const item = data[0];
    if (!item) { return ''; }
    const { value } = item;
    return value;
  }
}