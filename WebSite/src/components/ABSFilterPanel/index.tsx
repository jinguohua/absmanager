import React from 'react';
import TagList from './TagList';
import { IFilterSectionConfig } from './interface';
import { getSelectedFilterData, updateFilterDataState, calcuConfig } from './util';
import { FilterSelectListType, FilterPanelType } from './enum';
import Footer from './Footer';
import Header from './Header';
import Container from './Container';
import Converter from './Converter';
import SelectField from './SelectField';
import ExtraType from './enums/ExtraType';

export interface IABSFilterPanelProps {
  config: IFilterSectionConfig[];
  onClickItem: (value: IFilterSectionConfig[]) => void;
  onConfirm: (value: any) => void;
  onClose?: (flag: boolean, value: IFilterSectionConfig[]) => void;
  type?: FilterPanelType;
}

export interface IABSFilterPanelState {

}

class ABSFilterPanel extends React.PureComponent<IABSFilterPanelProps, IABSFilterPanelState> {
  extraView: Footer | null;
  header: Header | null;

  onConfirm = () => {
    const { config, onConfirm } = this.props;
    const selectedValue = getSelectedFilterData(config);
    let result: any = { filter_query_list: selectedValue };
    if (this.header) {
      const headerValue = this.header.extraValue();
      result = { ...result, ...headerValue };
    }
    if (this.extraView) {
      const extraValue = this.extraView.extraValue();
      result = { ...result, ...extraValue };
    }
    onConfirm(result);
  }

  onClose = (flag: boolean) => {
    if (this.extraView) {
      this.extraView.resetBuildDate();
    }
    const { onClose, config } = this.props;
    const result = Converter.resetState(config);
    if (!result) { return; }
    if (onClose) { onClose(flag, result); }
  }

  onClickItem = (sectionConfig: IFilterSectionConfig, itemIndex: number) => {
    if (!sectionConfig) { return; }
    const { config, onClickItem } = this.props;
    const sectionKey = sectionConfig.key;
    const updatedData = updateFilterDataState(config, sectionKey, itemIndex);
    const newConfig = calcuConfig(sectionConfig, updatedData);
    onClickItem(newConfig);
  }

  /**
   * 获取附加视图（Header、Footer）数据
   * @param type 附加视图类型，例：机构排名页面Header、投资>一级、二级市场Footer
   */
  getExtraValue(type: ExtraType): any {
    if (type === ExtraType.organizationHeader && this.header) {
      const value = this.header.extraValue();
      return value;
    } else if (type === ExtraType.investmentFooter) {
      // Todo
    }
    return null;
  }

  renderHeader() {
    const { type } = this.props;
    if (type !== FilterPanelType.organization) { return null; }
    return (
      <Header ref={view => this.header = view} />
    );
  }

  renderContent() {
    const { config } = this.props;
    if (!Array.isArray(config)) { return null; }
    return config.map((value, index, array) => {
      if (!value) { return null; }
      const type = value.type;
      if (type === FilterSelectListType.select) {
        return (
          <SelectField
            key={index}
            config={value}
            onClick={this.onClickItem}
          />
        );
      } else if (type === FilterSelectListType.checkbox) {
        return (
          <TagList
            key={index}
            config={value}
            onClick={this.onClickItem}
          />
        );
      }
      return null;
    });
  }

  renderFooter() {
    const { type, config } = this.props;
    if (type !== FilterPanelType.investment) { return null; }
    if (!Array.isArray(config) || config.length <= 0) { return null; }
    return (
      <Footer ref={view => this.extraView = view} />
    );
  }

  render() {
    return (
      <Container onConfirm={this.onConfirm} onClose={this.onClose}>
        {this.renderHeader()}
        {this.renderContent()}
        {this.renderFooter()}
      </Container>
    );
  }
}

export default ABSFilterPanel;