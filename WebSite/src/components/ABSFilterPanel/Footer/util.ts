export function intValue(value: string) {
  const v = parseInt(value, 0);
  if (isNaN(v)) {
    return 0;
  }
  return v;
}

export function floatValue(value: string) {
  const v = parseFloat(value);
  if (isNaN(v)) {
    return 0;
  }
  return v;
}