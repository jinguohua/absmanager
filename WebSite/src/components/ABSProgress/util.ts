export function getBottomInactiveRatio(currentTime: number, nextTime: number): number {
  const now = Date.now();
  if (currentTime >= now) {
    return 0;
  }
  const ratio = (now - currentTime) / (nextTime - currentTime);
  if (ratio >= 0.5) {
    return 1;
  }
  return 2 * ratio;
}

export function getTopInactiveRatio(currentTime: number, prevTime: number): number {
  const now = Date.now();
  if (currentTime <= now) {
    return 1;
  }
  const ratio = (now - prevTime) / (currentTime - prevTime);
  if (ratio <= 0.5) {
    return 0;
  }
  return (ratio - 0.5) * 2;
}