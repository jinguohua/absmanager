import { IFilterSectionConfig } from '../ABSFilterPanel/interface';
import { FilterSelectListType } from '../ABSFilterPanel/enum';

export function updateFilterDataState(filterData: IFilterSectionConfig[], sectionKey: number, itemIndex: number): any[] {
  if (!Array.isArray(filterData)) {
    return filterData;
  }
  for (const item of filterData) {
    if (!item || item.key !== sectionKey) {
      continue;
    }
    const listData = item.value;
    if (!Array.isArray(listData)) {
      continue;
    }
    if (listData.length - 1 < itemIndex) {
      continue;
    }
    for (const listitem of listData) {
      listitem.selected = false;
    }
    const prevState = listData[itemIndex].selected;
    listData[itemIndex].selected = !prevState;
  }
  return filterData.slice();
}

export function getSelectedFilterData(filterData: any[]|null|undefined): any[]|null {
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
    const selectedItems: any[] = [];
    for (const itemConfig of listData) {
      if (!itemConfig) {
        continue;
      }
      const selected = itemConfig.selected;
      if (!selected) {
        continue;
      }
      selectedItems.push(itemConfig);
    }
    const sectionConfigCopy = { ...sectionConfig };
    sectionConfigCopy.value = selectedItems[0];
    if (selectedItems.length <= 0 && type === FilterSelectListType.select && listData.length > 0) {
      const firstItem = listData[0];
      firstItem.selected = true;
      selectedItems.push(firstItem);
    }
    result.push(sectionConfigCopy);
  }
  return result;
}