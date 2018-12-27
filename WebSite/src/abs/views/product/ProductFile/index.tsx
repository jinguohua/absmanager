import React from 'react';
import './index.less';
import { connect } from 'dva';
import ABSPanel from '../../../../components/ABSPanel';
import { Row } from 'antd';
import ABSContainer from '../../../../components/ABSContainer';
import ABSNoContent from '../../../../components/ABSNoContent';
import ABSMinContainer from '../../../../components/ABSMinContainer';
import ABSLoading from '../../../../components/ABSLoading';
import CommonUtils from '../../../../utils/commonUtils';

export interface IABSProductFileProps {
  dispatch: any;
  fileList: Array<any>;
  dealID: string;
  loading?: boolean;
}

export interface IABSProductFileState {
}

class ProductDocuMenTation extends React.Component<IABSProductFileProps, IABSProductFileState> {
  constructor(props: IABSProductFileProps) {
    super(props);
    this.state = {
    };
  }

  componentDidMount() {
    const { dealID } = this.props;
    this.props.dispatch({
      type: 'product/getFileList',
      payload: { deal_id: dealID },
    });
  }

  onClick = (docid) => {
    this.props.dispatch({
      type: 'product/getFileListDownload',
      payload: { doc_id: docid },
    }).then((res) => {
      CommonUtils.downloadIsSuccess(res, () => {
          CommonUtils.downloadFile(res.data);
        });
      });
  }

  renderlist() {
    const { fileList, loading } = this.props;
    if (loading) {  
      return <ABSLoading />;
    }
    if (!fileList || fileList.length === 0) {
      return <ABSNoContent />;
    }

    return fileList.map((textitem, index) => {
      return (
        <ABSPanel title={textitem.doc_type} key={index}>
          {
            textitem.doc_list.map((doclist, indexs) => {
              return (
                <Row className="file-row" key={indexs} >
                  <div onClick={() => this.onClick(doclist.doc_id)}>
                    <p className="text-style">{doclist.name}</p>
                  </div>
                  {/* <ABSLink to={`${ProductApi.fileDownload}${doclist.file_code}`}>{doclist.name}</ABSLink>; */}
                </Row>
              );
            })
          }
        </ABSPanel>
      );
    });
  }

  render() {
    return (
      <ABSContainer>
        <ABSMinContainer>
          {this.renderlist()}
        </ABSMinContainer>
      </ABSContainer>
    );
  }
}

function mapStateToProps({ global, product, loading }: any) {
  return { fileList: product.fileList, dealID: global.params.dealID, loading: loading.models.product, fileurl: product.fileurl };
}

export default connect(mapStateToProps)(ProductDocuMenTation);