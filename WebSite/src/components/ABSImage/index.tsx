import React from 'react';
import classNames from 'classnames';

export interface IABSImageProps {
  logo: string;
  width?: number;
  height?: number;
  style?: React.CSSProperties;
  alt?: string;
  title?: string;
  onClick?: (enent: any) => void;
  className?: string;
}
class ABSImage extends React.Component<IABSImageProps> {
  render() {
    const { logo, width, height, style, alt, title, onClick, className } = this.props;
    const classes = classNames('abs-img', className);
    return (
      <div className={classes}>
        <img
          className="img"
          style={{ ...style }}
          src={logo}
          width={width}
          height={height}
          alt={alt}
          title={title}
          onClick={onClick}
        />
      </div>
    );
  }
}

export default ABSImage;  