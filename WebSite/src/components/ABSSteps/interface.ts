import { ReactNode } from 'react';

export interface IStepOption {
  title: string;
  component: () => ReactNode;
}