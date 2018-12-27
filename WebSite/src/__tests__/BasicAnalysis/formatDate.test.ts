import { formatDate } from './../../abs/views/product/BasicAnalysis/util';

test('should empty string', () => {
  const a = '';
  expect(formatDate(a)).toBe(a);
});

test('should return 2018-11-01', () => {
  const i = '2018-07-01 00:00:00';
  const o = '2018-07-01';
  expect(formatDate(i)).toBe(o);
});

test('should return 2018-10-30', () => {
  const i = '2018-10-30 11:54:57';
  const o = '2018-10-30';
  expect(formatDate(i)).toBe(o);
});

test('should return 2018-10-31', () => {
  const i = '2018-10-31';
  const o = '2018-10-31';
  expect(formatDate(i)).toBe(o);
});