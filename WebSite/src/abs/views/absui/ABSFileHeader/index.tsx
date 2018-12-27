import React, { Component } from 'react';
import '../../../../styles/coverAnt.less';
import ABSDescription from '../../../../components/ABSDescription';
import { connect } from 'dva';
import { ABSFileData } from '../../../../components/ABSFileList/ABSFile';
import ABSFileHeader from '../../../../components/ABSFileHeader';

interface IABSUIFileHeaderProps {
}

interface IABSUIFileHeaderState {
    isRead: boolean;
}

class ABSUIFileHeader extends Component<IABSUIFileHeaderProps, IABSUIFileHeaderState> {
    constructor(props: any) {
        super(props);
        this.state = {
            isRead: true
        };
    }
    onHeaderRead = () => {
        const { isRead } = this.state;
        this.setState({ isRead: !isRead });
    }

    onHeaderDelete = () => {
        alert('删除');
    }

    render() {
        const { isRead } = this.state;
        const fileData: ABSFileData = {
            title: '捷信成功发行2018年第四期个人消费贷款资产支持证券',
            url: '/market.html#/detail/news-info?id=12157',
            source: '中国网财经',
            publishDate: '2018-12-05'
        };
        const notice = {
            title: '捷信成功发行2018年第四期个人消费贷款资产支持证券',
            publishDate: '2018-12-05',
            source: '中国网财经',
            avatar: 'http://10.1.1.35:8000/filestore/common/downloadimg/cnabs/8a7faeef-84c2-c11b-f999-08d56deefc53/s'
        };
        return (
            <div>
                <ABSDescription style={{ marginTop: 12 }}>内容标题：</ABSDescription>
                <ABSFileHeader title={fileData.title} publishDate={fileData.publishDate} source={fileData.source} />
                <ABSDescription style={{ marginTop: 12 }}>站内公告标题：</ABSDescription>
                <ABSFileHeader
                    title={notice.title}
                    publishDate={notice.publishDate}
                    source={notice.source}
                    avatar={notice.avatar}
                    onRead={this.onHeaderRead}
                    onDelete={this.onHeaderDelete}
                    isRead={isRead}
                />
            </div>
        );
    }
}

function mapStateToProps({ account }: any) {
    return {
    };
}

export default connect(mapStateToProps)(ABSUIFileHeader);