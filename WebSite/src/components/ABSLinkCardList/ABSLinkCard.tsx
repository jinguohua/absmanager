import React from 'react';
import './ABSLinkCard.less';
import classnames from 'classnames';
import commonUtils from '../../utils/commonUtils';
import authUtil from '../../utils/authUtil';

// tslint:disable-next-line:interface-name
export interface LinkCard {
  name: string | null;
  url: string | null;
  key: string;
}

export const LinkCardBG = [
  'abs-link-list-item-flag-bg-blue-19',
  'abs-link-list-item-flag-bg-green-5',
  'abs-link-list-item-flag-bg-orange-1',
  'abs-link-list-item-flag-bg-blue-20',
  'abs-link-list-item-flag-bg-red-10',
  'abs-link-list-item-flag-bg-violet-1',
];

export interface IABSLinkCardProps {
  style?: React.CSSProperties;
  fontStyle?: React.CSSProperties;
  className?: string;
  item: LinkCard;
  currentColor: string;
  onShowMenuModal: () => void;
}

class ABSLinkCard extends React.Component<IABSLinkCardProps> {
  modal;
  goFunctionUrl = () => {
    const { item, onShowMenuModal } = this.props;
    const url = item.url ? item.url : '';
    if (item) {
      if (authUtil.hasPermission(item.key)) {
        location.href = commonUtils.parseUrl(url);
      } else {
        if (onShowMenuModal) {
          this.props.onShowMenuModal();
        }
      }
    }
  }
  render() {
    const { style, fontStyle, className, item, currentColor } = this.props;
    const currentColors = classnames('abs-link-list-item-flag', currentColor);
    const classNames = classnames('abs-link-list-item-content', className);
    return (
      <div className="abs-link-list-item" style={style} onClick={this.goFunctionUrl}>
        {
          item.url ?
            <>
              <div className={currentColors} />
              <p className={classNames} style={fontStyle}>{item.name}</p>
            </>
            : null
        }
      </div>
    );
  }
}

export default ABSLinkCard;