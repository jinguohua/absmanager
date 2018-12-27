import ISelectData from './ISelectData';
import ISelectOption from './ISelectOption';

export default class ProductDataConverter {
  static convertSelectData(response: any): ISelectData {
    const empty = {
      roleOptions: [], // 参与角色
      dutyOptions: [], // 个人职责
    };
    if (!Array.isArray(response) || response.length <= 0) { return empty; }
    const roleKey = 23;
    const dutyKey = 24;
    let roleSection, dutySection;
    for (const section of response) {
      if (!section) { continue; }
      const { key } = section;
      if (key === roleKey) {
        roleSection = section;
        continue;
      }
      if (key === dutyKey) {
        dutySection = section;
        continue;
      }
    }
    let roleOptions: ISelectOption[] = [];
    if (roleSection) {
      const roleValue = roleSection.value;
      roleOptions = this.convertSelectOptions(roleValue);
    }
    let dutyOptions: ISelectOption[] = [];
    if (dutySection) {
      const dutyValue = dutySection.value;
      dutyOptions = this.convertSelectOptions(dutyValue);
    }
    const selectData: ISelectData = {
      roleOptions,
      dutyOptions,
    };
    return selectData;
  }

  static convertSelectOptions(data: any): ISelectOption[] {
    const selectOptions: ISelectOption[] = [];
    if (!Array.isArray(data) || data.length <= 0) {
      return selectOptions;
    }
    for (const item of data) {
      if (!item) { continue; }
      const { key, value } = item;
      const roleOption: ISelectOption = {
        value: key.toString(),
        labelName: value,
      };
      selectOptions.push(roleOption);
    }
    return selectOptions;
  }

  // static convertABSProject(dealInfo: any, baseInfo: any): IABSProject | null {
  //   if (!dealInfo || !baseInfo) { return null; }
  //   const { deal_id: dealId } = dealInfo;
  //   const { deal_name: dealName } = baseInfo;
  //   if (!dealId || !dealName) { return null; }
    
  //   const project = new ABSProjectImpl();
  //   project.productId = dealId;
  //   project.product = dealName;
  //   return project;
  // }
}