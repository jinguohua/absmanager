import { FilterSelectListType } from './enum';

export interface IFilterSectionConfig {
  key: number;
  value: IFilterItemConfig[];
  title: string;
  type: FilterSelectListType;
  relatedkey: number | null;
}

export interface IFilterItemConfig {
  key: number;
  value: string;
  selected: boolean;
  relatedkey: number[] | null;
  hide: boolean;
}