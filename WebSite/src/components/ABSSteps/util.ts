import { ReactNode } from 'react';
import { IStepOption } from './interface';

export function getStepContent(steps: IStepOption[], current: number): null|(() => ReactNode) {
  if (!Array.isArray(steps) || steps.length <= current) {
    return null;
  }
  const option = steps[current];
  if (!option) {
    return null;
  }
  const component = option.component;
  if (!component) {
    return null;
  }
  return component;
}