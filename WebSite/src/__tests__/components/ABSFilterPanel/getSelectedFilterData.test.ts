import { getSelectedFilterData } from '../../../components/ABSFilterPanel/util';

test('should null', () => {
  const i = null;
  const o = null;
  expect(getSelectedFilterData(i)).toEqual(o);
});

test('should null', () => {
  const i = undefined;
  const o = null;
  expect(getSelectedFilterData(i)).toEqual(o);
});

test('should return selected value', () => {
  const i = [
    {
      key: 1,
      value: [{
        key: 0,
        value: '全部',
        selected: true
        },
        {
          key: 97,
          value: '中原证券股份有限公司',
          selected: false
        },
        {
          key: 695,
          value: '重庆国际信托股份有限公司',
          selected: false
        }
      ],
      title: '发行/管理人',
      type: 'select'
    },
    {
      key: 7,
      value: [{
          key: 2018,
          value: '2018',
          selected: true
        },
        {
          key: 2017,
          value: '2017',
          selected: true
        }
      ],
      title: '产品发行时间',
      type: 'checkbox'
    }
  ];

  const o = [
    {
      key: 1,
      value: [
        {
          key: 0,
          value: '全部',
          selected: true
        },
      ],
      title: '发行/管理人',
      type: 'select'
    },
    {
      key: 7,
      value: [
        {
          key: 2018,
          value: '2018',
          selected: true
        },
        {
          key: 2017,
          value: '2017',
          selected: true
        }
      ],
      title: '产品发行时间',
      type: 'checkbox'
    }
  ];
  expect(getSelectedFilterData(i)).toEqual(o);
});

test('should return selected value', () => {
  const i = [
    {
      key: 1,
      value: [{
        key: 0,
        value: '全部',
        selected: false
        },
        {
          key: 97,
          value: '中原证券股份有限公司',
          selected: false
        }
      ],
      title: '发行/管理人',
      type: 'select'
    },
    {
      key: 7,
      value: [{
          key: 2018,
          value: '2018',
          selected: false
        },
        {
          key: 2017,
          value: '2017',
          selected: true
        }
      ],
      title: '产品发行时间',
      type: 'checkbox'
    }
  ];

  const o = [
    {
      key: 1,
      value: [
        {
          key: 0,
          value: '全部',
          selected: true
        },
      ],
      title: '发行/管理人',
      type: 'select'
    },
    {
      key: 7,
      value: [
        {
          key: 2017,
          value: '2017',
          selected: true
        }
      ],
      title: '产品发行时间',
      type: 'checkbox'
    }
  ];
  expect(getSelectedFilterData(i)).toEqual(o);
});