import { IFilterSectionConfig, IFilterItemConfig } from './interface';

export default class Converter {
  static resetState(config: IFilterSectionConfig[]): IFilterSectionConfig[] | null {
    if (!Array.isArray(config)) { return null; }
    const result: IFilterSectionConfig[] = [];
    for (const section of config) {
      if (!section) { continue; }
      const sectionVal = section.value;
      if (!Array.isArray(sectionVal) || sectionVal.length <= 0) {
        result.push(section);
        continue;
      }
      const newSectionVal: IFilterItemConfig[] = [];
      for (const item of sectionVal) {
        if (!item) { continue; }
        const val = {
          ...item,
          selected: false,
          hide: false,
        };
        newSectionVal.push(val);
      }
      const newSection = { ...section, value: newSectionVal };
      result.push(newSection);
    }
    return result;
  }
}