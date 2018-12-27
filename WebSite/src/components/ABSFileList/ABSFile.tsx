import React from 'react';
import './ABSFile.less';
import '../../components/ABSBubble/index.less';
import ABSLink from '../../components/ABSLink';

// tslint:disable-next-line:interface-name
export interface ABSFileData {
    title?: string;
    source?: string;
    publishDate?: string;
    url?: string;
}

export interface IABSFileProps {
    data: ABSFileData;
    key: any;
    isLast?: boolean;
}

class ABSFile extends React.Component<IABSFileProps> {
    static defaultProps = {
        isLast: false,
    };

    render() {
        const { data, key, isLast } = this.props;
        const url = data.url ? data.url : '#';
        if (isLast) {
            return (
                <div className="abs-file">
                    <div className="list-view">
                        <ABSLink to={url} target="_blank" key={key}>
                            <p className="content-text">{data.title === null ? '-' : data.title}</p>
                        </ABSLink>
                        <p className="adress-time-last">{`来源 ：${data.source === null ? '-' : data.source} | ${data.publishDate === null ? '-' : data.publishDate}`}</p>
                    </div>
                </div>
            );
        } else {
            return (
                <div className="abs-file">
                    <div className="list-view">
                        <ABSLink to={url} target="_blank" key={key}>
                            <p className="content-text">{data.title === null ? '-' : data.title}</p>
                        </ABSLink>
                        <p className="adress-time-last">{`来源 ：${data.source === null ? '-' : data.source} | ${data.publishDate === null ? '-' : data.publishDate}`}</p>
                    </div>
                    <div className="line" />
                </div>
            );
        }

    }
}

export default ABSFile;