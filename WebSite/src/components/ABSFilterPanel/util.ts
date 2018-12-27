import { IFilterSectionConfig, IFilterItemConfig } from './interface';
import { FilterSelectListType } from './enum';

export function calcuConfig(sectionConfig: IFilterSectionConfig, filterData: IFilterSectionConfig[]): IFilterSectionConfig[] {
  const marketCategoryKey = 4;
  const productCategoryKey = 5;
  if (sectionConfig && sectionConfig.key !== marketCategoryKey) {
    return filterData;
  }
  const marketKeys = getMarketKeys(sectionConfig);
  const productValue = getProductValue(filterData, productCategoryKey);
  updateFilterProductOptions(marketKeys, productValue);
  return filterData;
}

export function updateFilterDataState(
  filterData: IFilterSectionConfig[],
  sectionKey: number,
  itemIndex: number,
): any[] {
  if (!Array.isArray(filterData)) {
    return filterData;
  }
  for (const item of filterData) {
    if (!item || item.key !== sectionKey) {
      continue;
    }
    const listData = item.value;
    const type = item.type;
    if (!Array.isArray(listData)) {
      continue;
    }
    if (type === FilterSelectListType.select) {
      listData.map((value, index, array) => {
        if (index === itemIndex) {
          value.selected = true;
        } else {
          value.selected = false;
        }
      });
    } else if (type === FilterSelectListType.checkbox) {
      if (listData.length - 1 < itemIndex) {
        continue;
      }
      const prevState = listData[itemIndex].selected;
      listData[itemIndex].selected = !prevState;
    }
  }
  return filterData.slice();
}

export function getSelectedFilterData(filterData: any[] | null | undefined): any[] | null {
  if (!Array.isArray(filterData)) {
    return null;
  }
  const result: any[] = [];
  for (const sectionConfig of filterData) {
    if (!sectionConfig) {
      continue;
    }
    const type = sectionConfig.type;
    const listData = sectionConfig.value;
    if (!Array.isArray(listData)) {
      continue;
    }
    const selectedItemValue: number[] = [];
    for (const itemConfig of listData) {
      if (!itemConfig) {
        continue;
      }
      const selected = itemConfig.selected;
      const hide = itemConfig.hide;
      if (!selected || hide) {
        continue;
      }
      selectedItemValue.push(itemConfig.key);
    }
    const resultItem: any = {};
    resultItem.key = sectionConfig.key;
    resultItem.value = selectedItemValue;

    if (selectedItemValue.length <= 0 && type === FilterSelectListType.select && listData.length > 0) {
      const firstItem = listData[0];
      firstItem.selected = true;
      const key = firstItem.key;
      selectedItemValue.push(key);
    }
    if (selectedItemValue.length <= 0) {
      continue;
    }
    result.push(resultItem);
  }
  return result;
}

function getMarketKeys(sectionConfig: IFilterSectionConfig): number[] {
  if (!sectionConfig) {
    return [];
  }
  const value = sectionConfig.value;
  if (!Array.isArray(value)) {
    return [];
  }
  const keys: number[] = [];
  value.map(item => {
    if (item && item.selected) {
      keys.push(item.key);
    }
  });
  return keys;
}

function getProductValue(filterData: IFilterSectionConfig[], productCategoryKey: number): IFilterItemConfig[] {
  for (const secConfig of filterData) {
    if (!secConfig) {
      continue;
    }
    if (secConfig.key === productCategoryKey && Array.isArray(secConfig.value)) {
      return secConfig.value;
    }
  }
  return [];
}

function updateFilterProductOptions(marketKeys: number[], productValue: IFilterItemConfig[]) {
  if (!Array.isArray(marketKeys) || !Array.isArray(productValue)) {
    return;
  }
  if (marketKeys.length <= 0) {
    productValue.map(pv => pv.hide = false);
    return;
  }
  productValue.map(pv => {
    let hide = true;
    const relateKeys = pv && Array.isArray(pv.relatedkey) ? pv.relatedkey : [];
    for (const mv of marketKeys) {
      for (const rv of relateKeys) {
        if (rv === mv) {
          hide = false;
          break;
        }
      }
      if (!hide) {
        break;
      }
    }
    pv.hide = hide;
  });
}