import React from 'react';
import ABSLinkCard, { LinkCard, LinkCardBG } from './ABSLinkCard';
import './index.less';
import { ABSModal } from '../ABSModal';

export interface IABSLinkCardListProps {
  list: Array<LinkCard>;
  numberOflines?: number;
}
const menuModalcontent = (
  <p>该功能仅对专业版用户开放，如果您有兴趣了解更多详情请与我们联系，电话：<a href="tel:021-31156258">021-31156258</a>，邮箱：<a href="mailto:feedback@cn-abs.com">feedback@cn-abs.com</a>，感谢您的关注与支持！</p>
);

class ABSLinkCardList extends React.Component<IABSLinkCardListProps> {
  menuModal;

  onShowMenuModal = () => {
    this.menuModal.setState({ visible: true });
  }

  renderList() {
    const { list } = this.props;
    return list.map((item, index) => {
      // 防止数组越界
      const indes = index % LinkCardBG.length;
      return <ABSLinkCard onShowMenuModal={this.onShowMenuModal} item={item} key={index} currentColor={LinkCardBG[indes]} />;
    });
  }

  render() {
    const { numberOflines } = this.props;
    // 一个item的宽度是300px + 间距是12px, 显示
    const width = 218 * (numberOflines ? numberOflines : 3) + 12;
    return (
      <div className="abs-link-card-list" style={{ width }}>
        {this.renderList()}
        <ABSModal
          content={menuModalcontent}
          width={360}
          footer={null}
          ref={view => this.menuModal = view}
        />
      </div>
    );
  }
}

export default ABSLinkCardList;