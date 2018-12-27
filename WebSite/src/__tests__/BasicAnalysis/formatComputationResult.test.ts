import { formatComputationResult } from '../../abs/views/product/BasicAnalysis/util';

test('should null', () => {
  const i1 = null;
  const o = null;
  expect(formatComputationResult(i1)).toBe(o);

  const i2 = undefined;
  expect(formatComputationResult(i2)).toBe(o);

  const i3 = '';
  expect(formatComputationResult(i3)).toBe(o);

  const i4 = 4;
  expect(formatComputationResult(i4)).toBe(o);
});

test('should right array', () => {
  const i = [
    {
      security_id: 1,
      short_name: '证券1',
      loss: 0,
      irr: 0,
      npv: 0,
      wal_for_principal: 0,
      payback_period: 0,
      loss_threshold: 0,
    },
    {
      security_id: 2,
      short_name: '证券2',
      loss: 0,
      irr: 0,
      npv: 0,
      wal_for_principal: 0,
      payback_period: 0,
      loss_threshold: 0,
    }
  ];
  const o = [
    {
      key: 1,
      name1: '证券1',
      name2: 0,
      name3: 0,
      name4: 0,
      name5: 0,
      name6: 0,
      name7: 0,
    },
    {
      key: 2,
      name1: '证券2',
      name2: 0,
      name3: 0,
      name4: 0,
      name5: 0,
      name6: 0,
      name7: 0,
    }
  ];
  expect(formatComputationResult(i)).toEqual(o);
});
