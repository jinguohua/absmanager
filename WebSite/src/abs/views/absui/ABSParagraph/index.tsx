import React, { Component } from 'react';
import './index.less';
import ABSParagraph from '../../../../components/ABSParagraph';
import ABSDescription from '../../../../components/ABSDescription';

class ABSUIParagraph extends Component {
  render () {
    return (
      <div className="absui-paragraph">
        <ABSDescription>段落（行间距）</ABSDescription>
        <ABSParagraph className="absui-paragraph">
          中国资产证券化论坛年会于2017年以“合作创新 百花齐放”为主题，在国家会议中心顺利举行 ，汇集国内外资产证券化与结构性融资领域专家学者、行业领袖，探讨国际结构性融资行业经验，共研中国资产证券化发展，是我国资产证券化行业层次最高、规模最大、影响力最强的年度盛会。
        </ABSParagraph>

        <ABSDescription style={{ marginTop: 32 }}>段落（段间距大于行间距）</ABSDescription>
        <div className="abs-paragraph-group">
          <ABSParagraph className="absui-paragraph">
            中国资产证券化论坛年会于2017年以“合作创新 百花齐放”为主题，在国家会议中心顺利举行,
          </ABSParagraph>
          <ABSParagraph className="absui-paragraph">
            汇集国内外资产证券化与结构性融资领域专家学者、行业领袖，探讨国际结构性融资行业经验，共研中国资产证券化发展，是我国资产证券化行业层次最高、规模最大、影响力最强的年度盛会。 
          </ABSParagraph>
        </div>
      </div>
      
    );
  }
}

export default ABSUIParagraph;