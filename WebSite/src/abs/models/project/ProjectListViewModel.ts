export default interface IProjectListItem {
  id: number;
  fullName: string;
  shortName: string;
  status: string;
  type: string;
  totalOffer?: number;
  assetTotal?: number;
  isserDate?: Date;
  endDate?: Date;
}
